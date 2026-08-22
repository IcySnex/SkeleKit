# SkeleKit Rider plugin

The plugin adds C# hot reload to Rider's normal .NET iOS simulator debug session. The user selects a
simulator and presses **Debug**. Rider still owns the build, launch, debugger, breakpoints, stepping,
and console. The plugin only inserts a transparent soft-debugger proxy that can apply Roslyn
Edit-and-Continue deltas.

## Supported scope

| Target | Rider debugging | SkeleKit hot reload |
| --- | --- | --- |
| Local iOS simulator | through the plugin bridge | yes |
| Physical device over USB | Rider's stock debugger (when available) | no |
| Physical device over Wi-Fi | unmodified/unsupported by this plugin | no |

Physical devices are deliberately not instrumented. Device builds use a different debugger topology,
and device hot reload also requires interpreter/AOT decisions that this plugin does not own. Selecting
a physical device must behave exactly as it does without the plugin.

The simulator path is verified with Rider 2026.1.4. It supports repeated sessions, breakpoints,
stepping, Debug output, app termination, and cumulative method-body edits in the app and referenced
projects such as `SkeleKit.iOS`.

The plugin is intentionally pinned to Rider build line `261.*` (2026.1.x). Its debugger-worker APIs
and bytecode hook are Rider internals, so each later Rider line must be inspected and live-tested
before the compatibility range is widened.

## Architecture

There are three moving parts.

### 1. Simulator port interception

Rider computes `IOSDebuggingPorts(portForDebugger, portForDevice)` inside
`IOSSessionHandler.preparePortsForDebugging`. Its concrete macOS handlers are final and live in a
module that plugin code cannot subclass, so `IosPortInstrumenter` installs a small ByteBuddy advice
on that method.

`PreparePortsAdvice` has two gates:

1. `IOSAppInfo.isSimulator()` must be true.
2. The backend must have published a valid `appPort:riderPort` pair.

If either gate fails, the advice leaves Rider's return value untouched. This is the complete transport
policy. There are no USB or Wi-Fi overrides in MSBuild.

The advice is inlined into a Rider-owned classloader and cannot call plugin classes. The backend ports
therefore cross the frontend boundary over RD as one string, and `BridgePortPublisher` mirrors that
string into the `skelekit.ios.bridgePorts` JVM property. A missing property is the fail-safe.

### 2. Native debug bridge

`SkeleKitHost` starts one `NativeBridge` for a solution containing a .NET iOS project. The bridge:

- accepts the simulator's debugger/output connections on a dynamic app port;
- connects each one to the dynamic port where Rider's debugger worker is listening;
- identifies the soft-debugger connection by its `DWP-Handshake`, never by connection order;
- relays Rider traffic unchanged;
- reserves command IDs at `0x40000000` for injected commands; and
- listens on port 9988 for the SkeleKit in-app UI reload channel.

The bridge publishes its ports only after both startup and solution discovery succeed. Closing the
solution clears the matching JVM property. Ports are JVM-global, so hot reload currently assumes one
active SkeleKit solution per Rider process.

### 3. Delta engine and debugger symbol sync

When the soft-debugger connection appears, the backend discovers the app Rider just deployed and its
referenced projects. For each assembly it:

1. asks MSBuild for the real `CscCommandLineArgs`;
2. reconstructs the Roslyn compilation, including generators and analyzer config;
3. verifies its declaration shape against the deployed assembly;
4. creates an EnC baseline from the deployed DLL; and
5. starts watching the assembly's source files.

On save, `ReloadEngine` emits a delta. Before applying it to Mono, the backend sends the IL,
metadata, PDB, updated method/type tokens, and source-line mappings to the plugin component running
inside `JetBrains.Debugger.Worker`.

That component stages Rider's own `EnCDelta`. `SdbConnection` then sends the runtime update:

```text
APPDOMAIN CREATE_BYTE_ARRAY  (metadata)
APPDOMAIN CREATE_BYTE_ARRAY  (IL)
APPDOMAIN CREATE_BYTE_ARRAY  (PDB)
MODULE APPLY_CHANGES
```

Mono accepts the injected update but does not emit `ENC_UPDATE` for this path. After a successful
apply, the bridge therefore emits that standard event on the Rider-facing connection. Rider's
built-in event processor consumes the already-staged delta, applies its PDB to the loaded symbol
reader, updates line mappings for unchanged methods, and rebinds the module's breakpoints.

After a successful apply, the bridge sends a one-byte signal on port 9988.
`Source/Framework/SkeleKit.iOS/App/HotReload.cs` then rebuilds the live page tree while retaining its view models. The
app-side component does not compile or apply deltas; it is only the UI refresh hook.

## Simulator build requirements

`Source/Framework/SkeleKit.iOS/build/SkeleKit.iOS.targets` enables the runtime prerequisites only for a Debug
simulator build with `EnableHotReload=true`:

- `UseInterpreter=true`;
- `MetadataUpdater.IsSupported=true` with `Trim="true"`; and
- `DOTNET_MODIFIABLE_ASSEMBLIES=debug`.

The target is shipped as a `buildTransitive` asset in the `SkeleKit.iOS` package. It also records the
compiler arguments Rider needs to reproduce the deployed assemblies. It does not start a process,
select a device, or alter a physical-device transport.

## Safety rules

These checks prevent known Mono EnC crashes:

- The reconstructed compilation must match the deployed assembly's declaration shape.
- The running module MVID must match the DLL used for the baseline.
- Structural edits and signature changes are skipped.
- Edits that introduce a runtime type absent from the baseline are skipped.
- Referenced projects baseline against the copy inside the deployed app bundle when available.
- Each assembly is initialized before its watcher starts, so the first edit is not mistaken for the
  baseline.
- A failed build, delta, socket command, or UI signal must not terminate Rider's backend.

Skipped edits produce a restart notice in Rider's Debug output.

## Build and verification

Package the plugin:

```sh
cd Tools/SkeleKit.Rider
./gradlew buildPlugin
```

The archive is written to `build/distributions/`.

Stop any running `runIde` sandbox before invoking `buildPlugin`, `prepareSandbox`, or another
`runIde`. Those tasks replace the sandbox's .NET backend assembly; replacing it while Rider still
has it loaded can make a later lazy JIT read fail with `Bad IL range`.

Launch the sandbox Rider:

```sh
cd Tools/SkeleKit.Rider
JAVA_HOME="$(/usr/libexec/java_home -v 21)" ./gradlew runIde \
  --args="/Users/kevin/Repos/SkeleKit/Source/SkeleKit.slnx"
```

For an end-to-end check:

1. Select a local simulator in the stock **Multi Platform > iOS** configuration.
2. Press **Debug**.
3. Wait for `Hot reload ready.` in Debug output.
4. Make a method-body edit and save.
5. Confirm `Hot reloaded <file>; Rider symbols updated.` and the live UI update.
6. Set a breakpoint on a newly added line, hit it, and use **Step Over** across the edited method.
7. Confirm Rider does not show the “source file may have changed since building the module” warning.
8. Stop, start another Debug session, and repeat once.
9. Select a physical device and confirm the frontend log contains no simulator reroute.

Useful logs:

- frontend: `build/idea-sandbox/RD-2026.1.4/log/idea.log`;
- backend: `build/idea-sandbox/RD-2026.1.4/log/backend.*.log`, filtered by `[native]`.

## Known limitations

- Structural edits and new runtime type references require a restart.
- Active-statement remapping is not synthesized. Edit while the app is running, then enter the edited
  method. Mono may reject an update saved while Rider is stopped at a breakpoint; resume execution
  and save again. The compilation baseline and pending Rider delta are left recoverable after that
  rejection.
- Port 9988 is fixed because the in-app UI refresh hook dials it directly. If it is busy,
  code deltas still apply but the automatic UI rebuild is unavailable.
- The bridge selects the newest executable iOS build output because Rider does not publish its chosen
  project/RID to the backend extension. The running MVID check prevents applying a delta to a different
  build, but the session then requires a rebuild and redeploy.
- Other .NET iOS apps can receive code deltas if their runtime has EnC enabled. Only SkeleKit apps
  have the port-9988 live-page refresh hook.

The wire-level mechanism is documented in
[`hot-reload-debugging.md`](hot-reload-debugging.md). The original design exploration remains in
[`Plan-HotReload.md`](Plan-HotReload.md).
