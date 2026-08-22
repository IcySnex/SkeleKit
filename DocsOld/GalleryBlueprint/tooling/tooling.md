# Generator, build, and Rider tooling

Classification: **Code-only/non-gallery**. These are consumer-visible compile/build/debug capabilities, not UI controls.

## `[Page]` and generated APIs

`PageAttribute` is applied to a `ContentView` subclass. Its `Singleton` property defaults to `false`; transient registration is generated unless it is set to `true`.

```csharp
[Page(Singleton = true)]
public sealed class HomePage : ContentView
{
	public HomePage() => Content = new Label { Text = "Home" };
}

public static class AppBootstrap
{
	public static SkeleApplication Create() =>
		SkeleApplication.CreateBuilder()
			.SinglePage<HomePage>()
			.Build(); // generated extension
}
```

| Generated API | Exact shape | Behavior |
| --- | --- | --- |
| `UsePages()` | `public static SkeleApplicationBuilder UsePages(this SkeleApplicationBuilder builder)` | Registers every valid `[Page]` as a default; an earlier manual registration takes precedence. A non-generic page uses an accessible parameterless constructor. `ContentView<TViewModel>` uses a constructor whose first parameter is `TViewModel` and whose remaining parameters are optional. |
| `Build()` | `public static SkeleApplication Build(this SkeleApplicationBuilder builder)` | Calls generated `UsePages()` and then `BuildCore()`. With hot reload enabled, a private linker-root method holds public signature type references and is retained through `DynamicDependency`. |

The generator emits fully qualified type references and does not scan assemblies at runtime.

## Diagnostics

| ID | Severity | Trigger | Resolution |
| --- | --- | --- | --- |
| `SKEL001` | Error | A class marked `[Page]` does not inherit `SkeleKit.ContentView`. | Derive from `ContentView`/`ContentView<TViewModel>` or remove the attribute. |
| `SKEL002` | Error | A marked page is abstract. | Mark only a concrete page or register a concrete subclass manually. |
| `SKEL003` | Error | A non-generic page lacks an accessible parameterless constructor, or a ViewModel page lacks an accessible constructor beginning with the ViewModel type and followed only by optional parameters. | Add the required constructor or use manual `UsePages(...)` registration. |

Accessible here means `public`, `internal`, or `protected internal`; a plain `protected` constructor does not satisfy generated construction.

## Build-transitive hot reload

`SkeleKit.iOS.targets` activates only when all three predicates are true:

```text
EnableHotReload == true
Configuration == Debug
RuntimeIdentifier starts with iossimulator
```

The active path sets `UseInterpreter=true`, `ProvideCommandLineArgs=true`, `SKELEKIT_HOT_RELOAD`, a trimmer root for `SkeleKit.iOS`, `MetadataUpdater.IsSupported=true` with `Trim="true"`, and `DOTNET_MODIFIABLE_ASSEMBLIES=debug`. After a real compile it writes `$(IntermediateOutputPath)skelekit-hotreload.args` from `@(CscCommandLineArgs)`. Release and physical-device builds are unchanged.

```xml
<PropertyGroup>
  <EnableHotReload>true</EnableHotReload>
</PropertyGroup>
```

## Rider plugin

The plugin augments Rider's normal local iOS-simulator Debug session. Rider still builds, launches, debugs, steps, owns breakpoints, and shows console output. The plugin reroutes only simulator debug ports through a soft-debugger bridge, reconstructs deployed compilations from captured compiler arguments, emits and applies method-body EnC deltas, updates Rider symbols/line mappings, then signals SkeleKit's live-page rebuild while retaining ViewModels.

Current live packaging configuration is plugin `1.0.2`, Rider build line `262.*` (`since-build="262"`, `until-build="262.*"`). The older `Docs/Rider-Plugin.md` statement naming `261.*` is recorded as a finding.

| Supported | Not supported / restart required |
| --- | --- |
| Local iOS simulator; repeated sessions; app and referenced-project method-body edits; cumulative edits; breakpoints and stepping after symbol sync | Physical-device hot reload; structural edits; signature changes; new runtime type references absent from the baseline; synthesized active-statement remapping; multiple simultaneously active SkeleKit solutions in one Rider process |

Startup or update failures are fail-safe: an invalid declaration shape, MVID mismatch, build failure, delta failure, socket failure, or UI signal failure must not terminate Rider. Skipped edits print a restart notice. Port `9988` is fixed for the UI refresh signal; if occupied, code deltas may still apply but automatic page rebuilding is unavailable.

## Verification lab

1. Build a Debug simulator app with `EnableHotReload=true` and confirm `Hot reload ready.`.
2. Save a method-body edit in the app, then in `SkeleKit.iOS`; confirm the UI refreshes and Rider reports symbol update.
3. Bind and hit a breakpoint on a newly added line, then step over it.
4. Try a structural edit and confirm a restart notice rather than a runtime apply.
5. Start a second Debug session to verify baseline reset.
6. Select a physical device and confirm no simulator reroute or hot-reload build switches.