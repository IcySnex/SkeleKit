# SkeleKit.HotReload

C# hot reload for SkeleKit on the iOS **simulator** — edit a `.cs` file and the running app updates in
place, no reinstall. Works without an IDE (Rider/VS never wired code hot reload for non-MAUI .NET iOS,
and `dotnet watch` crashes on the sim — its named-pipe delta transport can't cross the sandbox).

## How it works

```
edit .cs ──▶ host tool (Roslyn EmitDifference) ──▶ TCP 127.0.0.1:9988 ──▶ app
                                                                            │
                     MetadataUpdater.ApplyUpdate(IL delta) ◀───────────────┘
                                     │
                     PageHost.ReloadLive() rebuilds every live page
```

- **Host** (`Tools/SkeleKit.HotReload`, plain `net10.0`): reconstructs the app's Roslyn `Compilation`
  from its exact compiler command line (references, defines, **source generators**), uses the deployed
  dll as the `EmitBaseline`, and on each save computes a metadata/IL delta with `EmitDifference`, then
  ships it over TCP.
- **App** (`SkeleKit.iOS/App/HotReload.cs`, dev-only): a background TCP client applies the delta with
  `MetadataUpdater.ApplyUpdate` and calls `PageHost.ReloadLive` (Mono does **not** invoke
  `[MetadataUpdateHandler]`s itself). The page is reconstructed from its registry factory reusing the
  same ViewModel, so a changed ctor / method body shows live with ViewModel state preserved.

## Requirements (already wired in the Gallery)

The app must be built so `MetadataUpdater.IsSupported` is true — the undocumented combo:

1. `<UseInterpreter>true</UseInterpreter>` — ships `libmono-component-hot_reload.dylib`.
2. `<RuntimeHostConfigurationOption Include="System.Reflection.Metadata.MetadataUpdater.IsSupported"
   Value="true" Trim="true" />` — **`Trim="true"` is essential**, or the linker bakes `IsSupported`
   to a constant `false`.
3. launched with env `DOTNET_MODIFIABLE_ASSEMBLIES=debug`.

All three are behind `-p:EnableHotReload=true` in the Gallery csproj, so a normal Debug build stays on
the faster JIT with zero hot-reload cost. Release (AOT) can never hot-reload; the app-side code trims
away entirely.

## Use

```bash
xcrun simctl list devices available          # pick a booted simulator UDID
Tools/SkeleKit.HotReload/run.sh <sim-udid>   # build, launch, and start the host
```

Then edit any `.cs` under `Samples/SkeleKit.Gallery` and save.

## Scope

Reloads **method / constructor / property-accessor body** edits (the common case — includes view-tree
changes in a ctor). Adding or removing types/members is a rude edit and needs a restart; the host
prints it and keeps going. One assembly (the app head); library edits need the app rebuilt.
