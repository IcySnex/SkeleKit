#!/usr/bin/env bash
# Hot reload + breakpoints on the iOS simulator. Builds the app with the debug bridge, launches it on
# the sim pointed at the bridge, and waits until it's ready — then you attach Rider's "Mono Remote"
# once and get breakpoints AND live C# hot reload together.
#
#   Tools/SkeleKit.HotReload/skele-debug.sh <sim-udid> [project-dir]
#
# Use it as the "Before Launch" step of a Rider Mono Remote config (Host 127.0.0.1, Port 9986,
# Listen off) so one Debug press does everything. Find a UDID: xcrun simctl list devices available
set -euo pipefail

UDID="${1:?usage: skele-debug.sh <sim-udid> [project-dir]}"
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
PROJ="${2:-$ROOT/Samples/SkeleKit.Gallery}"
RID="iossimulator-arm64"
OUT="$PROJ/bin/Debug/net10.0-ios/$RID"
LOG="$PROJ/obj/Debug/net10.0-ios/$RID/skelekit-hotreload.log"

echo "==> building with hot reload + debug bridge"
dotnet build "$PROJ" -p:RuntimeIdentifier=$RID -p:EnableHotReload=true -p:EnableHotReloadDebug=true

APP="$(ls -d "$OUT"/*.app | head -1)"
APPID="$(plutil -extract CFBundleIdentifier raw "$APP/Info.plist")"

echo "==> launching $APPID on $UDID (debug endpoint -> bridge)"
xcrun simctl install "$UDID" "$APP"
SIMCTL_CHILD___XAMARIN_DEBUG_HOSTS__=127.0.0.1 \
SIMCTL_CHILD___XAMARIN_DEBUG_PORT__=9987 \
SIMCTL_CHILD___XAMARIN_DEBUG_MODE__=1 \
SIMCTL_CHILD___XAMARIN_DEBUG_CONNECT_TIMEOUT__=120 \
	xcrun simctl launch --terminate-running-process "$UDID" "$APPID"

echo "==> waiting for the app to reach the bridge..."
for _ in $(seq 1 40); do
	grep -q "attach Rider" "$LOG" 2>/dev/null && break
	sleep 0.5
done

echo ""
echo "  Ready. In Rider: Debug the 'Mono Remote' config (Host 127.0.0.1, Port 9986, Listen off)."
echo "  Then set breakpoints and edit any .cs — both work. App console: $LOG"
