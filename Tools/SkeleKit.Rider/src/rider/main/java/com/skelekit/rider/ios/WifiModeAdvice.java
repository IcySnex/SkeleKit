package com.skelekit.rider.ios;

import com.jetbrains.rider.run.multiPlatform.ios.devices.IOSDevice;
import net.bytebuddy.asm.Advice;

// IOSDebugOverWiFi defaults to false in the .NET iOS SDK, which makes Rider select its legacy USB
// debugger even when the selected physical device is connected only over the network. The advice is
// inlined into IOSAppInfo.isWiFiMode, so keep it limited to Rider argument types and the JDK.
public class WifiModeAdvice {
    @Advice.OnMethodExit
    public static void exit(
        @Advice.Argument(0) IOSDevice device,
        @Advice.Return(readOnly = false) boolean wifiMode) {
        if (!wifiMode && !device.isConnectedOverUsb()) {
            System.out.println("[SkeleKit] network-only iOS device: enabling Wi-Fi debugging");
            wifiMode = true;
        }
    }
}
