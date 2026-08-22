# Hot reload **while debugging** on .NET iOS

How SkeleKit does C# hot reload *and* breakpoints at the same time on the iOS simulator — something
neither Rider, Visual Studio, nor `dotnet watch` do for .NET iOS. This is the full reasoning: what
was tried, why the obvious paths are dead, and the mechanism that actually works.

> TL;DR — the runtime **can** apply Edit-and-Continue deltas, but only *through the debugger
> connection*, and only the IDE normally drives that (and none do it for Mono/iOS). So we became a
> **Mono soft-debugger proxy**: sit on the wire between the app and the IDE, relay everything (so
> breakpoints work), and **inject** the apply-changes commands ourselves.

## The wall

Plain hot reload (Run mode) applies deltas with `MetadataUpdater.ApplyUpdate`. But:

```
[SkeleKit] hot reload failed: Cannot use MetadataUpdater.ApplyChanges while debugger is attached
```

The runtime **forbids the managed apply while a debugger is attached** — EnC can have one owner, and
under a debugger that owner is the debugger. On desktop, Visual Studio applies the delta *through the
debugger* (so you get both). Rider does that only for CoreCLR/Windows; for Mono/iOS its hot reload is
XAML-only. So: attach a debugger → no managed hot reload, and the IDE won't do it for us. Mutually
exclusive — unacceptable.

## Dead ends (and why)

- **A normal Rider backend call to `ApplyChanges`.** Rider runs the debug session in an isolated
  `JetBrains.Debugger.Worker` process, and its public plugin model exposes no Mono-EnC operation.
  SkeleKit therefore still applies the runtime delta through the wire proxy. A small debugger-worker
  plugin is used only to keep Rider's PDB/sequence-point state synchronized.
- **A standalone client / proxy via Rider's generic "Mono Remote".** Microsoft.iOS apps don't speak
  raw soft-debugger on connect; they use a proprietary control protocol first. Generic Mono Remote
  connecting straight to the app **deadlocks** (both sides wait) — confirmed on the wire: Rider sent 0
  bytes and errored, the app sent 0 bytes.

## The breakthrough — the transport is open

`dotnet/macios` is open source. `runtime/monotouch-debug.m` shows the app's side:

1. The app connects out to `__XAMARIN_DEBUG_HOSTS__:__XAMARIN_DEBUG_PORT__` (the "IDE").
2. The IDE sends **length-prefixed text commands** (1 byte length + string): `connect stdout`,
   `start profiler: …`, `start debugger: sdb`, …
3. On **`start debugger: sdb`** the app hands that same socket to the Mono soft-debugger agent, which
   does the standard `DWP-Handshake` and then speaks the Mono debugger wire protocol.

Verified live: after sending length-prefixed `start debugger: sdb`, the app replied `DWP-Handshake`
then a real sdb `VM_START` event. **The proprietary part is just that one preamble.**

## Applying a delta over the debugger

The apply command (from the Mono runtime source, `debugger-agent.c`):

- Command set `MDBGPROT_CMD_SET_MODULE = 24`, command `MDBGPROT_CMD_MODULE_APPLY_CHANGES = 2`.
- Args: a **module id** (`decode_moduleid`, an sdb object id — not an MVID) then **three object ids**
  for the `dmeta` / `dil` / `dpdb` byte arrays (`decode_objid`).
- The deltas aren't raw bytes on this command — they must already be `byte[]` **objects in the app's
  runtime**. You create them first with `MDBGPROT_CMD_APPDOMAIN_CREATE_BYTE_ARRAY = 8` (args: domain
  id, int length, raw bytes → returns the new array's object id).

So the full sequence a debugger runs to hot-reload:

```
start debugger: sdb          (length-prefixed preamble; unblocks the app)
DWP-Handshake                (both ways)
VM VERSION / SET_PROTOCOL_VERSION / RESUME
APPDOMAIN GET_ROOT_DOMAIN → GET_ASSEMBLIES → (per assembly) GET_NAME → find SkeleKit.Gallery
ASSEMBLY GET_MANIFEST_MODULE → module id
APPDOMAIN CREATE_BYTE_ARRAY × 3   (dmeta, dil, dpdb → object ids)
MODULE APPLY_CHANGES (module id, dmeta_id, dil_id, dpdb_id) → err 0
```

`MetadataUpdater` handlers are **not** invoked by Mono on a debugger-applied delta, so after the apply
we tell the app to rebuild its live pages (`PageHost.ReloadLive`). Proven on the sim: the page title
changed **while a debugger was attached**.

## Production architecture — Rider simulator bridge

For a local simulator only, the Rider plugin changes the two ports in Rider's normal iOS debug start
info. The app connects to the SkeleKit bridge and the bridge connects to the port already owned by
Rider's debugger worker:

```
simulator app  ⟷  SkeleKit bridge  ⟷  Rider debugger worker
                        │
             Roslyn EmitDifference
                        │
      stage PDB delta in Rider's worker
                        │
       CREATE_BYTE_ARRAY ×3 + APPLY_CHANGES
                        │
                    ReloadLive
```

Rider still owns the session, breakpoints, stepping, inspection, output, and lifecycle. Physical
devices are not rerouted; USB and Wi-Fi behavior remains Rider's stock behavior and SkeleKit hot
reload is off.

The one non-obvious engineering piece is **command-id multiplexing**: the IDE and our injector both
send commands on the one connection, so the bridge parses the sdb packet stream, gives injected
commands a reserved high id range, and consumes only their replies. Every real runtime event
continues to Rider.

Before `APPLY_CHANGES`, the backend sends the matching metadata/IL/PDB delta and line mappings to the
plugin assembly loaded inside `JetBrains.Debugger.Worker`. Mono accepts the injected delta but does
not emit `ENC_UPDATE`, so after a successful apply the bridge sends that standard event to Rider.
Rider's existing EnC event processor consumes the staged delta, updates its symbol reader, and
rebinds the module's breakpoints. This is what keeps newly inserted lines and **Step Over** aligned
with the running method.

## Key references

- `dotnet/macios` `runtime/monotouch-debug.m` — the `start debugger: sdb` control protocol.
- `dotnet/runtime` `src/mono/mono/component/debugger-agent.c` + `debugger-protocol.h` — the sdb
  command sets, `MODULE_APPLY_CHANGES`, `APPDOMAIN_CREATE_BYTE_ARRAY`, `decode_objid`.
- `mono/debugger-libs` `Mono.Debugging.Soft` — the client-side wire encodings (ids, byte arrays,
  packet framing: `len[4] id[4] flags[1] set[1] cmd[1]`; reply `… flags[1] errorcode[2]`).
