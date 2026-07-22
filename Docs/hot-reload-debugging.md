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

- **A Rider plugin that calls `ApplyChanges` on the session.** Rider runs the debug session in an
  isolated worker process (`JetBrains.Debugger.Worker`); the `ApplyChanges` primitive exists in
  `debugger-libs` but lives in that worker, and Rider exposes no Mono-EnC operation. A plugin can't
  reach into the worker or add protocol operations. Not an effort problem — the operation isn't wired.
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

## Production architecture — the unified bridge

`EnableHotReload` builds bake the app's debug endpoint (`__XAMARIN_DEBUG_HOSTS__/PORT`) at our bridge
and auto-start it. The bridge:

```
app  ⟷  SkeleKit bridge  ⟷  IDE (Rider "Mono Remote" attach)
              │
   • relays all sdb traffic  → breakpoints/stepping/inspection just work
   • on save: Roslyn EmitDifference → CREATE_BYTE_ARRAY ×3 + APPLY_CHANGES → ReloadLive
   • if no IDE attaches: the bridge is the debugger itself → hot reload only
```

Unified by design: opting into hot reload means running under the bridge, so you get breakpoints for
free; opting out (a plain build) is the performance path with neither.

The one non-obvious engineering piece is **command-id multiplexing**: the IDE and our injector both
send commands on the one connection, so the bridge parses the sdb packet stream, gives injected
commands a reserved high id range, and swallows their replies (and the resulting `ENC_UPDATE` event)
so the IDE never sees them.

IDE-agnostic on purpose: anything that attaches to a Mono soft-debugger by host/port (Rider today; a
VS Code mono-attach or plain `dotnet` later) rides the same bridge.

## Key references

- `dotnet/macios` `runtime/monotouch-debug.m` — the `start debugger: sdb` control protocol.
- `dotnet/runtime` `src/mono/mono/component/debugger-agent.c` + `debugger-protocol.h` — the sdb
  command sets, `MODULE_APPLY_CHANGES`, `APPDOMAIN_CREATE_BYTE_ARRAY`, `decode_objid`.
- `mono/debugger-libs` `Mono.Debugging.Soft` — the client-side wire encodings (ids, byte arrays,
  packet framing: `len[4] id[4] flags[1] set[1] cmd[1]`; reply `… flags[1] errorcode[2]`).
