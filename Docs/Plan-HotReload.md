# Plan: SkeleKit Rider Plugin — Hot Reload + Breakpoints, "Just Works"

Sibling to [`hot-reload-debugging.md`](hot-reload-debugging.md), which explains the working proxy this
plan builds a first-class Rider UI on top of. Read that first for the wire-level mechanism.

## Context

SkeleKit already has C# hot reload **and** breakpoints working together on the iOS simulator via a
Mono soft-debugger proxy (`Tools/SkeleKit.HotReload/Sdb/DebugBridge.cs` + `SdbConnection.cs`). But the
UX is a shell script (`skele-debug.sh`) plus a hand-configured Rider "Mono Remote" run config the user
attaches manually. Rider, VS, and `dotnet watch` do none of this for non-MAUI .NET iOS.

Goal: replace the script/manual-config workflow with a first-class **Rider plugin** using official
frontend/backend APIs. Experience target:

- Device selection like any normal iOS run.
- **Run** → plain launch, nothing extra.
- **Debug** → breakpoints **and** live C# hot reload, both baked into one Mono Remote flow,
  transparent. No scripts, no manual "Mono Remote" config.

### Why the current proxy is 70% of the work already

- `SdbConnection` is a working Mono soft-debugger client: DWP handshake, big-endian command/reply
  framing, `APPLY_CHANGES` injection via `CREATE_BYTE_ARRAY ×3 + MODULE APPLY_CHANGES`.
- Relay mode already sidesteps ID collisions by carving injected commands into a high ID range
  (`InjectedIdBase = 0x40000000`) — no mapping table needed.
- `DebugBridge.Watch` already does Roslyn `EmitDifference` on file save against a deployed-dll baseline.

### Prior art that shapes the plugin design

From `hot-reload-debugging.md` "Dead ends":

- A Rider plugin **cannot** reach into the debug session to call `ApplyChanges` — the session runs in
  an isolated `JetBrains.Debugger.Worker` and Rider exposes no Mono-EnC operation. So the plugin never
  drives EnC through Rider; the **proxy** applies deltas on the side, exactly as today.
- A generic "Mono Remote" attaching **straight to the app** deadlocks on the `start debugger: sdb`
  preamble. So Rider must attach to the **bridge**, never the app. The bridge handles the preamble.

Net: the plugin's only debugger job is to auto-start a Mono Remote attach **to the bridge's port**.
That is narrow and does not require touching the worker.

### The one unsolved gap (Phase 2)

`SdbConnection.Read` → `IsEncEvent` **swallows** the runtime's `ENC_UPDATE`/`METHOD_UPDATE` events so
Rider never sees them. Consequence: after an edit, **Rider's symbols go stale** — a breakpoint on a
just-edited method maps to the old line table until the app restarts. Everything else stays exact.
Closing this is the "serve corrected debug info to Rider" work.

## Decisions (locked)

1. **Sequencing:** Plugin first; symbol repair is a later phase.
2. **Fallback:** Symbol accuracy on edited methods is a **hard requirement** for the *final* product.
   Phase 1 (plugin, drift present) is dev-usable but NOT done. If symbol repair proves impossible via
   the proxy, stop and rethink (own the debugger, or Run-mode-only reload) rather than ship drift.
3. **Architecture:** Reimplement the sdb proxy **inside the ReSharper backend** (C#, in-process),
   porting `DebugBridge`/`SdbConnection` logic. One process; owning the sdb stream in-backend is the
   enabler for Phase 2 symbol repair.

## Architecture

```
Rider plugin
  ├── Frontend (Kotlin, IntelliJ Platform / rd)
  │     ├── device dropdown (reuse Rider's iOS device list; simctl fallback)
  │     ├── Run action   → plain build + launch
  │     └── Debug action → triggers backend bridge + Mono Remote attach
  │
  └── Backend (C#, ReSharper backend process, rd protocol)
        ├── in-process sdb proxy   (ported DebugBridge + SdbConnection)
        ├── Roslyn EmitDifference  (Microsoft.CodeAnalysis, bundled)
        ├── FileSystemWatcher / save hook → apply deltas over sdb
        └── Phase 2: symbol-repair layer serving corrected debug info to Rider
```

Ports become dynamic (backend picks free ports; no fixed 9986/9987/9988). The plugin owns app launch,
so it sets `__XAMARIN_DEBUG_HOSTS__/PORT__/MODE__` env to point the app at the in-backend proxy.

## Phase 0 — De-risking spikes (do FIRST)

Both unknowns are in JetBrains' APIs, not our code. Resolve before building UI.

- **Spike A — Programmatic Mono Remote attach.** Can the plugin start a Rider debug session against an
  arbitrary `host:port` (the in-backend proxy) from a Debug action, without the user picking a "Mono
  Remote" config? Investigate Rider run-configuration extension points and the Mono/Unity remote-attach
  launch path. Fallback: register a custom run-configuration type wrapping Mono Remote with our
  host:port pre-filled.
- **Spike B — In-backend proxy hosting.** Confirm a long-lived TCP proxy + `Microsoft.CodeAnalysis`
  `EmitDifference` can run inside the ReSharper backend process (threading, assembly-load, lifecycle on
  solution close). Fallback: keep the proxy as a backend-*spawned* child .NET process (still
  plugin-owned, no user script).

Deliverable: a spike note (fold into this doc / `hot-reload-debugging.md`) recording which APIs exist
and which fallback, if any, we take.

### Spike A findings — RESOLVED, feasible

Reference implementation: JetBrains' own open-source **resharper-unity**, which auto-attaches a Mono
debug session to the Unity editor by host/port with no manual "Mono Remote" config. The exact path
(source: `rider/.../run/configurations/UnityAttachProfileState.kt`,
`run/attach/UnityLocalAttachDebugger.kt`):

- Subclass **`AttachDebugProfileStateBase`** (`com.jetbrains.rider.run`) — the public Rider frontend
  base for an attach debug session, driven through the standard `RunProfileState.execute()`.
- Override `createModelStartInfo` to return a Mono **`DebuggerStartInfoBase`** carrying **`host`,
  `port`, `listenForConnections`** (Unity's `UnityMonoStartInfo`; Rider has the generic base in
  `com.jetbrains.rider.model.debuggerWorker`). `host:port` = our bridge's IDE port;
  `listenForConnections=false` makes Rider the client that connects to the bridge.
- Rider's debugger worker performs the Mono soft-debugger attach from that start-info over rd. The
  plugin never reaches into the worker process (that was the ruled-out dead end) — it only supplies a
  start-info model. Consistent with our design: Rider attaches to the **bridge**, which handles the
  `start debugger: sdb` preamble the app requires.

Conclusion: the single **Debug** button is achievable with public SDK. No custom-run-config fallback
needed.

**Phase 2 lead (not yet confirmed):** resharper-unity ships its own debugger-worker component
(`debugger/debugger-worker/src/UnityDebuggerWorkerHost.cs`) plugging into Rider's worker. The worker
is where the Mono client (`Mono.Debugging.Soft`) — the thing that caches/fetches sequence points —
runs. So symbol repair may have a **worker-side extension hook** rather than requiring on-the-wire
symbol spoofing. Investigate before the Phase 2 probe.

## Phase 1 — Orchestration plugin (dev-usable, drift present)

1. **Plugin skeleton.** Gradle + IntelliJ Platform plugin targeting Rider; ReSharper backend half over
   rd. New top-level dir `Tools/SkeleKit.Rider/` (frontend `plugin/`, backend `backend/`).
2. **Device selection.** Booted/available simulators (reuse Rider's iOS device provider if exposed;
   else `xcrun simctl list devices`).
3. **Run action.** Build `-p:RuntimeIdentifier=iossimulator-arm64` (no hot reload), install, launch.
4. **Debug action.**
   - Build `-p:EnableHotReload=true` (+ debug bridge flag) — same MSBuild gates that make
     `MetadataUpdater.IsSupported` true (`UseInterpreter`, runtime option w/ `Trim="true"`,
     `DOTNET_MODIFIABLE_ASSEMBLIES=debug`), already in the `SkeleKit.iOS.HotReload` targets.
   - Backend starts the in-process sdb proxy on a free port.
   - Launch the app with `__XAMARIN_DEBUG_*` env pointing at the proxy.
   - Auto-start the Mono Remote attach to the proxy's IDE port (per Spike A).
   - Backend watches source; on save → `EmitDifference` → `ApplyChanges` over sdb → reload signal.
5. **Port the proxy.** Lift `DebugBridge`/`SdbConnection` logic into the backend. **Keep**
   `Tools/SkeleKit.HotReload` console host for CI / `--self-drive` / no-IDE use and the packaged NuGet.

End state: one Debug button gives breakpoints + hot reload, no scripts. Known limit: symbol drift on
edited methods until restart (Phase 2 closes it).

## Phase 2 — Symbol repair (hard requirement for final done)

Replace the event-swallowing in the ported `Read`/`IsEncEvent` with a symbol-serving layer.

1. **Probe (cheap, uses the working proxy).** Stop swallowing `ENC_UPDATE`/`METHOD_UPDATE`; log every
   command Rider sends after an edit. Answer: **does Rider's Mono debugger ever re-fetch method debug
   info (sequence points / locals) after a method update, or cache at first touch and never look
   again?**
2. **If Rider re-fetches:** serve the *new* sequence points/locals for updated methods from the proxy
   (we hold the fresh PDB delta from `EmitDifference`); forward the runtime's method-update events so
   Rider invalidates. Breakpoints on edited methods follow the new lines.
3. **If Rider caches-and-never-refetches:** proxy symbol repair is impossible → STOP and escalate
   (own the debugger session, or restrict hot reload to Run-mode). Do not ship permanent drift.

## Key risks

- **Spike A is existential** to the single-button promise. If no attach API and the custom-run-config
  fallback still needs a user gesture, revisit UX with the user.
- **Phase 2 feasibility is genuinely unknown**, gated on Rider-internal caching we don't control. The
  probe fails cheap and early by design.
- **JetBrains SDK is new to this repo** (Gradle + Kotlin + rd). Budget toolchain ramp into Phase 1.
- Do not regress the headless host — CI and the NuGet depend on it.

## Files

- **New:** `Tools/SkeleKit.Rider/` — plugin frontend (Kotlin/Gradle) + backend (C#/rd).
- **Ported:** `Tools/SkeleKit.HotReload/Sdb/{DebugBridge,SdbConnection}.cs`, `Differ.cs`,
  `Baseline.cs`, `Project.cs`, `CscInvocation.cs` → logic lifted into the backend (originals kept).
- **Reused as-is:** app-side reload signaling (`SkeleKit.iOS/App/HotReload.cs`, `PageHost.ReloadLive`),
  the `EnableHotReload` MSBuild gates in `build/`.

## Verification

- **Spikes:** written note confirming attach API (A) + in-backend hosting (B) or the chosen fallback,
  before UI work.
- **Phase 1 end-to-end:** boot a sim, pick it, press Debug → app launches, breakpoint in an unedited
  method hits; edit a ctor/method body + save → view tree updates live, VM state kept, no restart.
  Press Run → plain launch, no bridge. Zero scripts, no manual Mono Remote config.
- **Phase 2 end-to-end:** breakpoint on a method, edit that method's body (shift its lines), save →
  breakpoint follows the new line and hits without restart.
- **No regression:** `Tools/SkeleKit.HotReload` `--self-drive` + packaged NuGet still work headless.
