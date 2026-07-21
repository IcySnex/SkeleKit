#!/usr/bin/env bash
# Builds the Gallery with hot reload enabled, launches it on a simulator, and starts the delta host.
# Edit any .cs under Samples/SkeleKit.Gallery and the change applies live — no reinstall.
#
#   Tools/SkeleKit.HotReload/run.sh <sim-udid>
#
# Find a UDID with: xcrun simctl list devices available
set -euo pipefail

UDID="${1:?usage: run.sh <sim-udid>}"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
PROJ="$ROOT/Samples/SkeleKit.Gallery"
RID="iossimulator-arm64"
OUT="$PROJ/bin/Debug/net10.0-ios/$RID"
OBJ="$PROJ/obj/Debug/net10.0-ios/$RID"

echo "==> building app (interpreter + hot reload)"
dotnet build "$PROJ" -p:RuntimeIdentifier=$RID -p:EnableHotReload=true

echo "==> extracting compiler inputs"
touch "$PROJ/Program.cs"   # force CoreCompile so ProvideCommandLineArgs populates
dotnet build "$PROJ" -p:RuntimeIdentifier=$RID -p:EnableHotReload=true \
	-t:Compile -p:ProvideCommandLineArgs=true -p:SkipCompilerExecution=true \
	--getItem:CscCommandLineArgs 2>/dev/null > "$PROJ/cscargs.json"

echo "==> installing + launching on $UDID"
xcrun simctl install "$UDID" "$OUT/SkeleKit.Gallery.app"
SIMCTL_CHILD_DOTNET_MODIFIABLE_ASSEMBLIES=debug \
	xcrun simctl launch "$UDID" com.skelekit.gallery

echo "==> starting hot reload host (Ctrl+C to stop)"
exec dotnet run --project "$ROOT/Tools/SkeleKit.HotReload" -c Release -- \
	"$PROJ/cscargs.json" "$OBJ/SkeleKit.Gallery.dll"
