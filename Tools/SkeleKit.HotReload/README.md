# SkeleKit.HotReload

C# hot reload **and breakpoints together** on the iOS simulator — edit a `.cs` and the running app
updates in place, while a debugger stays attached. Neither Rider, Visual Studio, nor `dotnet watch`
does this for .NET iOS. The how-and-why is in [`Docs/hot-reload-debugging.md`](../../Docs/hot-reload-debugging.md).

## How it works (short version)

The app can only apply Edit-and-Continue deltas *through the debugger connection* while a debugger is
attached. So the host becomes a **Mono soft-debugger proxy** on the wire between the app and the IDE:

```
app  ⟷  SkeleKit bridge  ⟷  Rider ("Mono Remote" attach)
              │
   • relays sdb traffic          → breakpoints / stepping just work
   • on save: EmitDifference → CREATE_BYTE_ARRAY ×3 + MODULE APPLY_CHANGES → ReloadLive
   • no IDE attached (--self-drive) → the bridge is the debugger; hot reload only
   • the app's second connection → "connect output" → its console prints here
```

Ports: `9987` app debugger, `9986` IDE attach, `9988` reload signal.

## Setup

The app must be built so `MetadataUpdater.IsSupported` is true — `UseInterpreter=true`, a
`MetadataUpdater.IsSupported=true` runtime option with `Trim="true"`, and a baked
`DOTNET_MODIFIABLE_ASSEMBLIES=debug`. The `SkeleKit.iOS.HotReload` NuGet's `build/` targets do all of
that behind `<EnableHotReload>true</EnableHotReload>`.

**Breakpoints + hot reload (Rider):**

```bash
xcrun simctl list devices available            # pick a booted simulator UDID
Tools/SkeleKit.HotReload/skele-debug.sh <udid> # builds, starts the bridge, launches the app at it
```

Then in Rider: **Debug** a **Mono Remote** config (Host `127.0.0.1`, Port `9986`, Listen off). Set
breakpoints, edit any `.cs`, save — both work. Make it one press by setting `skele-debug.sh` as that
config's **Before Launch** step.

**Hot reload only (no IDE):** build with `-p:EnableHotReload=true` (without `EnableHotReloadDebug`) —
the build auto-starts the bridge in `--self-drive` and it drives the app itself.

## Scope

Reloads **method / constructor / property-accessor body** edits (covers view-tree changes in a ctor).
Adding or removing types/members is a rude edit and needs a restart; the host prints it. One assembly
(the app head); library edits need the app rebuilt.

## Packaging

`dotnet pack Tools/SkeleKit.HotReload -c Release` → `SkeleKit.iOS.HotReload.nupkg` (targets in
`build/`, the bridge + Roslyn + `skele-debug.sh` in `tools/hotreload/`).
