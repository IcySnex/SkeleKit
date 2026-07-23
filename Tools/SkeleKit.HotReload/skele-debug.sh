#!/usr/bin/env bash
# Hot reload + breakpoints on the iOS simulator. Builds the app, starts the SkeleKit debug bridge,
# and launches the app on the booted simulator pointed at the bridge. Then attach Rider's "Mono
# Remote" (Host 127.0.0.1, Port 9986, Listen off) once and get breakpoints AND live C# hot reload.
#
#   skele-debug.sh [project-dir]
#
# The booted simulator is auto-detected — just have one running (Rider's iOS run, or Simulator.app).
set -euo pipefail

HERE="$(cd "$(dirname "$0")" && pwd)"
if [ -f "$HERE/skele-hotreload.dll" ]; then
	# installed from the NuGet package: the host sits next to this script
	HOST="$HERE/skele-hotreload.dll"
	PROJ="${1:?usage: skele-debug.sh <project-dir>}"
else
	# running from the SkeleKit repo
	ROOT="$(cd "$HERE/../.." && pwd)"
	HOST="$ROOT/Tools/SkeleKit.HotReload/bin/Release/net10.0/skele-hotreload.dll"
	PROJ="${1:-$ROOT/Samples/SkeleKit.Gallery}"
fi

RID="iossimulator-arm64"
OBJ="$PROJ/obj/Debug/net10.0-ios/$RID"
OUT="$PROJ/bin/Debug/net10.0-ios/$RID"
LOG="$OBJ/skelekit-hotreload.log"
PID="$OBJ/skelekit-hotreload.pid"

UDID="$(xcrun simctl list devices booted 2>/dev/null | grep -oE '[0-9A-Fa-f-]{36}' | head -1 || true)"
[ -n "$UDID" ] || { echo "no booted simulator — boot one first (Rider iOS run, or Simulator.app)"; exit 1; }

echo "==> building $(basename "$PROJ") with hot reload + debug bridge"
dotnet build "$PROJ" -p:RuntimeIdentifier=$RID -p:EnableHotReload=true -p:EnableHotReloadDebug=true

APP="$(ls -d "$OUT"/*.app 2>/dev/null | head -1)"
[ -n "$APP" ] || { echo "no .app in $OUT"; exit 1; }
APPID="$(plutil -extract CFBundleIdentifier raw "$APP/Info.plist")"
DLL="$APP/$(basename "$APP" .app).dll"

echo "==> starting the bridge"
[ -f "$PID" ] && kill "$(cat "$PID")" 2>/dev/null || true
sleep 0.3
nohup dotnet "$HOST" bridge "$OBJ/skelekit-hotreload.args" "$DLL" "$PROJ" >"$LOG" 2>&1 &
echo $! >"$PID"
for _ in $(seq 1 40); do grep -q "waiting for the app" "$LOG" 2>/dev/null && break; sleep 0.25; done

echo "==> launching $APPID on $UDID"
xcrun simctl install "$UDID" "$APP"
SIMCTL_CHILD___XAMARIN_DEBUG_HOSTS__=127.0.0.1 \
SIMCTL_CHILD___XAMARIN_DEBUG_PORT__=9987 \
SIMCTL_CHILD___XAMARIN_DEBUG_MODE__=1 \
SIMCTL_CHILD___XAMARIN_DEBUG_CONNECT_TIMEOUT__=120 \
	xcrun simctl launch --terminate-running-process "$UDID" "$APPID" >/dev/null

echo "==> waiting for the app to reach the bridge..."
for _ in $(seq 1 60); do grep -q "attach Rider" "$LOG" 2>/dev/null && break; sleep 0.25; done

if grep -q "attach Rider" "$LOG" 2>/dev/null; then
	echo ""
	echo "  Ready. In Rider: Debug the 'Mono Remote' config (Host 127.0.0.1, Port 9986, Listen off)."
	echo "  Then set breakpoints and edit any .cs — both work. App console: $LOG"
else
	echo "  the app did not reach the bridge — check $LOG"
	exit 1
fi
