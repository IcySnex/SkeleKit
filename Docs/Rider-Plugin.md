# SkeleKit Rider Plugin — Status & Handoff

Transparent native-session hot reload in Rider: the user presses the **normal Debug** on the stock
**Multi Platform > iOS** run config (native device picker, deploy, breakpoints, console) and **also
gets live C# hot reload** — one button, zero extra steps, nothing new to learn.

**Status: WORKING, sim-verified** (Rider 2026.1.4, local iOS simulator). Build, deploy, breakpoints,
pause/step, Debug-output feedback, app-close-ends-session, multi-session, and cumulative live hot
reload of method-body edits in both the app and `SkeleKit.iOS` all work.

**It is not SkeleKit-specific.** The delta engine reconstructs the app's compilation from the real
`csc` command line, so any .NET iOS project in the solution works with no MSBuild opt-in. See
[Scope](#scope-what-works-for-which-apps).

Companion docs: [`hot-reload-debugging.md`](hot-reload-debugging.md) (wire-level sdb mechanism),
[`Plan-HotReload.md`](Plan-HotReload.md) (original plan). Plugin lives at `Tools/SkeleKit.Rider/`.

---

## How it works (the whole trick)

Rider does **not** support C# hot reload for Mono/iOS (its debugger runs a `DummyEncManager`). So we
sit transparently inside Rider's own native debug session and inject Edit-and-Continue deltas ourselves.

1. **Reroute the debug ports (frontend, ByteBuddy).** `preparePortsForDebugging` on the base class
   `IOSSessionHandler` returns `IOSDebuggingPorts(portForDebugger, portForDevice)`. The concrete
   handlers (`IOSLocalSessionHandler`) are `final` and in the off-classpath `intellij.rider.macos`
   module, so we can't subclass. Instead a **self-attached ByteBuddy agent**
   (`IosPortInstrumenter` postStartupActivity → `PreparePortsAdvice`) rewrites that method's return.
   Sandbox JVM already has `-Djdk.attach.allowAttachSelf=true`.

2. **The ports are negotiated, not hardcoded.** The backend binds a free pair at solution load and
   publishes them over rd; `BridgePortPublisher` mirrors them into the `skelekit.ios.appPort` /
   `skelekit.ios.riderPort` **system properties**, which is the only channel the advice can read (its
   body is inlined into a class that cannot see plugin types). **Unset means no reroute at all** — that
   is both the gate (a solution with no .NET iOS project is left alone) and the failure-safe.

3. **Sit in the middle (backend, `NativeBridge`).** A `[SolutionComponent]` starts the bridge. It
   listens on the app port and on **9988** (the in-app reload channel), and for each app connection
   opens one to Rider's port (with retry — the worker may start listening a beat late). Every
   connection is relayed transparently, so breakpoints / stepping / console are untouched.

4. **Topology (confirmed by lsof):** Rider **LISTENS** on `portForDebugger`; the app **CONNECTS** to
   `portForDevice` (3 connections: sdb-debug + stdout + stderr).

5. **Identify + inject on the sdb connection.** Each connection self-identifies: the sdb one starts
   with the `DWP-Handshake`. On it we frame-parse app→Rider (forwarding everything, incl. the runtime's
   ENC/METHOD_UPDATE events) and inject sdb commands with a reserved high id range (`0x40000000`),
   swallowing our own replies. On a source save the engine builds an EnC delta with the **host's
   Roslyn** (`EmitDifference`) and injects `CREATE_BYTE_ARRAY ×3 + MODULE APPLY_CHANGES`, then signals
   the app on 9988 to rebuild the live UI.

6. **The app side.** `SkeleKit.iOS/App/HotReload.cs` connects to 9988 and calls `PageHost.ReloadLive`
   on an empty-module signal, which works even under the debugger. An app without that agent still
   gets its **code** updated; it just doesn't redraw.

---

## Scope: what works for which apps

| Piece | SkeleKit app | Any other .NET iOS app |
| --- | --- | --- |
| Breakpoints / stepping / console through the bridge | yes | yes |
| Reconstructing the compilation, building + applying deltas | yes | yes |
| Live UI rebuild after a delta | yes (`HotReload.cs` → `PageHost.ReloadLive`) | no — code updates, UI redraws on its own next pass |
| Runtime prerequisites | `<EnableHotReload>true</EnableHotReload>` | see below |

Nothing in the engine is SkeleKit-aware any more. The two things that used to bind it:

- **The csc command line** used to come from `skelekit-hotreload.args`, written by SkeleKit's MSBuild
  targets. It now comes from `dotnet msbuild -t:Compile -p:ProvideCommandLineArgs=true
  -p:SkipCompilerExecution=true` with the dump target injected via `CustomAfterMicrosoftCommonTargets`
  (`MsBuild.cs`). Stock Roslyn properties, ~1s, nothing in the user's project changes, and
  `SkipCompilerExecution` means the deployed build is not touched.
- **Source generators** used to be an allowlist of two. All of them now run, with the real
  `/analyzerconfig:` and `/additionalfile:` inputs (`AnalyzerConfig.cs`).

**Untested for non-SkeleKit apps:** whether the Mono runtime accepts `APPLY_CHANGES` without the
`EnableHotReload` MSBuild gates (`UseInterpreter=true`, the `MetadataUpdater.IsSupported`
`RuntimeHostConfigurationOption` with `Trim="true"`, `DOTNET_MODIFIABLE_ASSEMBLIES=debug`). The sdb
path does not go through managed `MetadataUpdater`, so a plain Debug simulator build may well work as
is — worth one experiment. On a **device** the interpreter is required regardless (full AOT cannot be
patched).

---

## Load-bearing rules (don't relearn)

- **The compilation must reproduce the deployed assembly, and it is checked.** `MetadataShape` emits
  the rebuilt compilation and compares its declarations to the deployed dll. A mismatch means a
  generator silently didn't run, and every later delta would describe code the app isn't running.
  On mismatch the engine retries without generators, then **disables itself for that assembly** rather
  than applying something dangerous.
- **Compare declarations as a set, ignoring `<`-named members.** Compiler-synthesized plumbing
  (closures, `<>y__InlineArray*`, extension metadata) differs between Roslyn builds, and ours is never
  the exact one that built the app. Row *order* differs too, and that is fine: Roslyn matches an edited
  member to its baseline row by name and signature, not position. An earlier ordered byte-for-byte
  check rejected every real project.
- **Analyzer assemblies demand their own Roslyn version.** Every SDK generator asks for
  `Microsoft.CodeAnalysis, Version=5.x`; the host has a different one, so `LoadFrom` fails and the
  generators quietly produce nothing. `Project.UnifyCompilerAssemblies` installs an `AssemblyResolve`
  hook mapping `Microsoft.CodeAnalysis*` / `System.Collections.Immutable` /
  `System.Reflection.Metadata` onto the loaded copies. Without it the Gallery loaded **1 of 16**
  generators and would not compile.
- **`/langversion:latest` must become `LanguageVersion.Preview`, not `Latest`.** "Latest" means
  whatever the SDK's compiler supported; mapping it to our Roslyn's `Latest` drops `field`, partial
  properties and C# 14 `extension` members that the app already uses. An explicit version is honored.
- **Use the host's Roslyn, ship none.** No `Microsoft.CodeAnalysis` PackageReference — the NuGet drags
  Immutable 10.x and shipping it FileLoadExceptions the SolutionComponent.
- **Rider 2026.1.4 version pins:** Kotlin **2.3.0** (rider-model.jar metadata is 2.3.0),
  IntelliJ-platform-gradle-plugin 2.10.5, rdGen 2026.1.1, `JetBrains.Rider.SDK` 2026.1.4,
  ByteBuddy 1.15.11.
- **net472 port:** the engine is net10 code; needs `Polyfills.cs` (init/required).
- **`compileDotNet` needs brew dotnet** at `/opt/homebrew/bin/dotnet` (Gradle's PATH lacks it).
- **Component must be eager:** `[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]`. Ride
  Rider's active `IRiderModelZone` via `[ZoneMarker] IRequire<IRiderModelZone>` — don't define a custom
  zone (stays inactive). Discovery + binding run on a background thread; the rd write is posted back
  through `IShellLocks.ExecuteOrQueueEx`.
- **iOS session seam:** `IOSDefaultSessionHandlerProvider` (open service, macOS override =
  `RiderLocalIOSSessionProvider`, both `final`). Port logic is on the base `preparePortsForDebugging` →
  instrument it, don't subclass.
- **sdb connection id must be by handshake, not order** — the "first connection is sdb" guess broke on
  later sessions (APPLY_CHANGES to a stdout socket → timeout → crash).
- **Never let a hot-reload error crash the backend** — every apply is wrapped.
- **The watcher thread must not block.** Saves are queued and handled by one worker with a 150 ms
  debounce; compiling on the watcher's callback overflows its buffer and silently drops edits.
- **VM_DEATH on app drop** so a crashing/closing app ends the session (iOS *suspend*, i.e. swipe-away,
  legitimately keeps it alive).
- **Conservative apply:** skip structural edits (added/removed members) AND deltas that add a new
  runtime type dependency (Mono EnC can't resolve a newly-referenced type live, e.g. first
  `Debug.WriteLine` → crash). Dependency checks inspect only changed methods and compare their old
  and new semantic operations; scanning raw delta `TypeRef` rows produces false positives from
  Roslyn-generated metadata. Both cases notify "restart to apply".
- **Referenced libraries use the deployed copy as their baseline.** Linker output has a different
  MVID and may retarget `Microsoft.iOS` from the targeting-pack version to the runtime-pack version.
  The compiler reference is retargeted in memory to that deployed identity while retaining the full
  targeting API; compiling against the trimmed deployed `Microsoft.iOS.dll` does not work.
- **Prewarm every assembly before starting its watcher.** Lazy initialization after the first save
  snapshots already-edited source and silently consumes that edit.
- **Feedback uses Mono `USER_LOG`.** Rider renders readiness, applied, skipped, and failed messages in
  its existing Debug output, on the same presentation path as `Debug.WriteLine`.
- **The app must be running the assembly we baselined.** Before applying, `MODULE_GET_INFO` gives the
  running module's MVID and it is compared to the baseline's.
- **`buildSearchableOptions` is off** — it starts a headless IDE, which fails while Rider is open.

---

## Key files

- `.../rider/main/java/.../PreparePortsAdvice.java` — ByteBuddy advice; reads the two system properties.
- `.../kotlin/com/skelekit/rider/ios/IosPortInstrumenter.kt` — installs the agent.
- `.../kotlin/com/skelekit/rider/ios/BridgePortPublisher.kt` — rd → system properties.
- `.../dotnet/SkeleKit.Rider.Backend/SkeleKitHost.cs` — `[SolutionComponent]`, starts the bridge,
  publishes the ports.
- `.../HotReload/NativeBridge.cs` — ports, MITM, session lifecycle, watcher + debounce worker.
- `.../HotReload/ReloadEngine.cs` — one per assembly: reconstruct, verify, diff, emit, apply.
- `.../HotReload/AppProject.cs` — solution/project discovery and build-output location, by reading the
  solution and project files rather than Rider's project model (no load-order or read-lock coupling,
  and no API that shifts between Rider releases).
- `.../HotReload/{MsBuild,CscInvocation,AnalyzerConfig,Project,Baseline,MetadataShape,Differ}.cs` —
  the delta engine.
- `.../HotReload/SdbConnection.cs` — sdb wire client in MITM mode.
- `SkeleKit.iOS/App/HotReload.cs` — the in-app agent (unchanged by the plugin).

---

## Build / run / test loop

- **Backend only** (fast error check): `dotnet build Tools/SkeleKit.Rider/SkeleKit.Rider.Backend.sln -c Debug /p:HostFullIdentifier=` (use `/opt/homebrew/bin/dotnet`).
- **Launch sandbox Rider:** `cd Tools/SkeleKit.Rider && JAVA_HOME="$(/usr/libexec/java_home -v 21)" ./gradlew runIde --args="/Users/kevin/Repos/SkeleKit/SkeleKit.slnx"`.
- **Package:** `./gradlew buildPlugin` → `build/distributions/SkeleKit.Rider-<version>.zip`.
- **Verify from logs:** backend `build/idea-sandbox/RD-2026.1.4/log/backend.*.log` — grep `[native]`.
  Frontend `idea.log` for the agent install + the published ports. Clear old `backend.*.log` first.
- **The engine is testable without Rider.** It is plain .NET: point a small harness at the solution and
  run discovery → `MsBuild.CscCommandLineArgs` → `Project.Build` → `MetadataShape` → `EmitDifference`.
  That is how the generator, langversion and shape-check problems above were found and fixed; the sim
  was only needed to confirm the wire path.
- **GUI test needs the user:** iOS Debug + edit a `.cs` can't be driven headlessly.

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
- **The reload port 9988 is still fixed** (the in-app agent dials it). If it is taken, deltas still
  apply; only the UI-rebuild nudge is lost.
- **The port system properties are JVM-global.** With several solutions open in one Rider, the last one
  loaded owns them; the others still debug normally through the bridge but don't hot reload.

---

## Next steps

1. **Test breakpoint/sequence-point behavior after several edits**, especially edits that add/remove
   lines before an active breakpoint. This is the largest remaining seamlessness gap.
2. **Add automated engine tests** for structural rejection, semantic new-type rejection, cumulative
   deltas, linked-library reference retargeting, and stale-session isolation.
3. **Run repeated start/stop/restart and app-crash soak tests** to exercise connection replacement
   and debugger-worker timing.
4. **Try a non-SkeleKit iOS app without the `EnableHotReload` gates** to settle whether the runtime
   prerequisites are needed on the simulator.
5. **Retire `Tools/SkeleKit.HotReload`** or keep it deliberately as the no-IDE / CI path — it is the
   only remaining consumer of the `skelekit-hotreload.args` MSBuild target.
6. **Install the packaged zip into the real Rider** and confirm it behaves the same as the sandbox.
7. Optional stretch: make the UI-reload port session-specific; this needs a matching way to tell the
   already-built in-app agent which port to dial.
8. Optional stretch: worker-process profiler injection for full symbol sync (limitation above).
