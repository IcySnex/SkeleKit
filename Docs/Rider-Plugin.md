# SkeleKit Rider Plugin — Status & Handoff

Transparent native-session hot reload for SkeleKit.iOS in Rider: the user presses the **normal
Debug** on the stock **Multi Platform > iOS** run config (native device picker, deploy, breakpoints,
console) and **also gets live C# hot reload** — one button, zero extra steps, nothing new to learn.

**Status: WORKING, sim-verified** (Rider 2026.1.4, local iOS simulator). Build, deploy, breakpoints,
pause/step, console, app-close-ends-session, multi-session, and live hot reload of method-body edits
all work. Remaining items are polish + hardening (see Next steps).

Companion docs: [`hot-reload-debugging.md`](hot-reload-debugging.md) (wire-level sdb mechanism),
[`Plan-HotReload.md`](Plan-HotReload.md) (original plan). Plugin lives at `Tools/SkeleKit.Rider/`.

---

## How it works (the whole trick)

Rider does **not** support C# hot reload for Mono/iOS (its debugger runs a `DummyEncManager`). So we
sit transparently inside Rider's own native debug session and inject Edit-and-Continue deltas ourselves.

1. **Reroute the debug ports (frontend, ByteBuddy).** `preparePortsForDebugging` on the base class
   `IOSSessionHandler` returns `IOSDebuggingPorts(portForDebugger, portForDevice)` (both = a single
   port normally). The concrete handlers (`IOSLocalSessionHandler`) are `final` and in the
   off-classpath `intellij.rider.macos` module, so we can't subclass. Instead a **self-attached
   ByteBuddy agent** (`IosPortInstrumenter` postStartupActivity → `PreparePortsAdvice`) rewrites that
   method's return to fixed ports: **`portForDevice = 10098`** (app connects here) and
   **`portForDebugger = 10099`** (Rider's debugger listens here). Sandbox JVM already has
   `-Djdk.attach.allowAttachSelf=true`.

2. **Sit in the middle (backend, `NativeBridge`).** A `[SolutionComponent]` starts `NativeBridge` on
   solution load. It listens on **10098** (where the app connects) and **9988** (the in-app reload
   channel), and for each app connection opens one to Rider's **10099** (with retry — the worker may
   start listening a beat late). Every connection is relayed transparently, so breakpoints / stepping /
   console are untouched.

3. **Topology (confirmed by lsof):** Rider **LISTENS** on `portForDebugger`; the app **CONNECTS** to
   `portForDevice` (3 connections: sdb-debug + stdout + stderr).

4. **Identify + inject on the sdb connection.** Each connection self-identifies: the sdb one starts
   with the `DWP-Handshake` (`SdbConnection.Mitm` → `ReadMitm`). On the sdb connection we frame-parse
   app→Rider (forwarding everything, incl. the runtime's ENC/METHOD_UPDATE events) and can inject sdb
   commands with a reserved high id range (`0x40000000`), swallowing our own replies. On a source
   save the engine builds an EnC delta with the **host's Roslyn** (`EmitDifference`) and injects
   `CREATE_BYTE_ARRAY ×3 + MODULE APPLY_CHANGES`, then signals the app on 9988 to rebuild the live UI.

5. **The app side.** The SkeleKit.iOS app must be built with `<EnableHotReload>true</EnableHotReload>`
   (Debug) — this ships the Mono interpreter + `MetadataUpdater` support + writes `cscargs` + includes
   the in-app `HotReload.cs` agent (connects to 9988; on an empty-module signal it calls
   `PageHost.ReloadLive`, which works even under the debugger). The Gallery csproj already sets it.

---

## Key files

- `Tools/SkeleKit.Rider/src/rider/main/java/.../PreparePortsAdvice.java` — ByteBuddy advice rewriting the ports.
- `.../kotlin/com/skelekit/rider/ios/IosPortInstrumenter.kt` — installs the agent (postStartupActivity).
- `.../dotnet/SkeleKit.Rider.Backend/SkeleKitHost.cs` — `[SolutionComponent]`, starts `NativeBridge`.
  **Gallery paths are HARDCODED here** (TODO: derive from the project model).
- `.../HotReload/NativeBridge.cs` — the MITM orchestrator + engine (watch → EmitDifference → inject → reload notice).
- `.../HotReload/SdbConnection.cs` — sdb wire client; `Mitm` mode (self-identify sdb via handshake, inject, VM_DEATH on drop, locked `SendToIde` for console notices).
- `.../HotReload/{CscInvocation,Project,Baseline,Differ,Pe,Polyfills}.cs` — the ported Roslyn delta engine (net472).
- `SkeleKit.iOS/App/HotReload.cs` — the in-app agent (unchanged by the plugin; reload + ReloadLive).
- `protocol/…/SkeleKitModel.kt` — rd model (currently UNUSED; NativeBridge doesn't use rd — safe to remove later).

---

## Build / run / test loop

- **Backend only** (fast error check): `dotnet build Tools/SkeleKit.Rider/SkeleKit.Rider.Backend.sln -c Debug /p:HostFullIdentifier=` (use `/opt/homebrew/bin/dotnet`).
- **Launch sandbox Rider:** `cd Tools/SkeleKit.Rider && JAVA_HOME="$(/usr/libexec/java_home -v 21)" ./gradlew runIde --args="/Users/kevin/Repos/SkeleKit/SkeleKit.slnx"`. Opens the real SkeleKit solution (has the iOS Gallery). Rebuilds rdgen → compileDotNet → prepareSandbox and **detaches** (gradle exits 0, Rider keeps running).
- **Verify from logs:** backend `build/idea-sandbox/RD-2026.1.4/log/backend.*.log` — grep `[native]` for engine + inject/reload lines. Frontend `idea.log` for the agent install + advice. Clear old `backend.*.log` before each launch.
- **GUI test needs the user:** iOS Debug + edit a `.cs` can't be driven headlessly. lsof on 10098/10099 shows the relay topology.
- Ports 10098/10099/9988 are loopback; `lsof -nP -iTCP:10098 -iTCP:10099` shows app↔bridge↔Rider.

---

## Load-bearing gotchas (don't relearn)

- **Rider 2026.1.4 version pins:** Kotlin **2.3.0** (rider-model.jar metadata is 2.3.0), IntelliJ-platform-gradle-plugin 2.10.5, rdGen 2026.1.1, `JetBrains.Rider.SDK` 2026.1.4, ByteBuddy 1.15.11.
- **Use the host's Roslyn, ship none.** No `Microsoft.CodeAnalysis` PackageReference — the NuGet drags Immutable 10.x and shipping it FileLoadExceptions the SolutionComponent. Rider bundles Roslyn (`42.42.42.42`) exposed by the SDK; just `using Microsoft.CodeAnalysis…`.
- **net472 port:** the engine is net10 code; needs `Polyfills.cs` (init/required) + net472 rewrites (`Split`, ranges collide with host `System.Index/Range`, `DistinctBy`, KeyValuePair deconstruction).
- **`compileDotNet` needs brew dotnet** at `/opt/homebrew/bin/dotnet` (Gradle's PATH lacks it).
- **Component must be eager:** `[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]`. Ride Rider's active `IRiderModelZone` via `[ZoneMarker] IRequire<IRiderModelZone>` — don't define a custom zone (stays inactive).
- **iOS session seam:** `IOSDefaultSessionHandlerProvider` (open service, macOS override = `RiderLocalIOSSessionProvider`, both `final`). Port logic is on the base `preparePortsForDebugging` → instrument it, don't subclass.
- **sdb connection id must be by handshake, not order** — the "first connection is sdb" guess broke on later sessions (APPLY_CHANGES to a stdout socket → timeout → crash).
- **Never let a hot-reload error crash the backend** — the apply is wrapped; a TypeLoad/timeout logs + skips.
- **Per-session reset** (`EndSession` on sdb drop) + **build the baseline on app-connect** (matches the just-deployed dll's MVID).
- **VM_DEATH on app drop** so a crashing/closing app ends the session (iOS *suspend*, i.e. swipe-away, legitimately keeps it alive).
- **Conservative apply:** skip structural edits (added/removed members) AND deltas that add a new `TypeRef`/`AssemblyRef` (Mono EnC can't resolve a newly-referenced type live, e.g. first `Debug.WriteLine` → crash). Both notify "restart to apply".

---

## Known limitations

- **Debugger symbol desync on complex edits.** After an edit, Rider warns "source may have changed
  since building the module" and stepping in edited methods can drift. Root cause: Rider's Mono
  debugger uses a **`DummyEncManager`** and its symbol store lives in the **debugger-worker process**
  (`JetBrains.Debugger.Worker`) — a third process our JVM agent + ReSharper backend can't reach.
  Forwarding the ENC/METHOD_UPDATE events didn't fix it (it has `SoftHotReloadManager`/
  `SoftModuleDeltaStorage`/`SoftRuntimeSequencePointsProvider` but only populates them when Rider
  itself applies). Simple body edits step fine. **Fully fixing needs injecting a CLR profiler/agent
  into the worker process** — big, fragile, uncertain payoff. Accepted for v1.
- **Structural + new-type-reference edits are skipped**, not applied (restart to pick them up).
- **Fixed ports 10098/10099/9988** — a second Rider instance or parallel sessions would collide.

---

## Next steps (priority order)

1. **Derive the app paths from the project model** — `SkeleKitHost` hardcodes the Gallery's cscargs +
   dll. Use the ReSharper project model to find the iOS runnable project's Debug/iossimulator build
   output (cscargs = `obj/…/skelekit-hotreload.args`, dll = `bin/…/<App>.dll`). Until then it only
   hot-reloads *this* Gallery. This is the #1 blocker for real use.
2. **Gate the reroute** on hot-reload-enabled projects — the advice currently reroutes *every* iOS
   Debug. Harmless (transparent relay) but should no-op / not spin the engine when not a SkeleKit app.
3. **Dynamic ports** instead of fixed — avoid multi-instance/parallel-session conflicts. Tricky because
   the advice (frontend) must communicate the port to the backend bridge; or keep the app port fixed
   and let the bridge discover Rider's real `portForDebugger` (the advice knows it pre-rewrite).
4. **Verify the console notices** actually display (stdout-connection injection — confirm in Rider's
   Debug/console tab). If not, fall back to crafting a `USER_LOG` sdb event
   (`UserLogEvent(req_id, thread_id, level, category, message)`; EventType.UserLog = 0x10).
5. **Delete the dead code / old host** once confident: `Tools/SkeleKit.HotReload` (the old standalone
   bridge this replaces), the unused `HotReload/Bridge.cs` (old relay + SelfTest/SmokeEmit scaffold),
   the unused `SdbConnection` legacy path (`Adopt`/`Relay`/`Read`/`SelfDrive`/`PipeOutput`), the unused
   rd `:protocol` model, and the leftover `connections`/`domain` fields in NativeBridge.
6. **Package/distribute** the plugin (`buildPlugin`), and confirm it works in the user's *real* Rider
   (not just the sandbox) — same iOS support is present.
7. Optional stretch: the worker-process profiler injection for full symbol sync (limitation above).

---

## The git trail (feat/rider-plugin branch)

Milestones are committed so you can bisect/rollback:
`feat(rider): plugin + in-proc hot-reload engine` → `transparent reroute` → `working native hot reload
+ breakpoints` → robustness (`session end`, `retry`, `sdb id`, `crash guard`, `new type refs`) →
`plugin-side console notices`. HEAD is a working build.
