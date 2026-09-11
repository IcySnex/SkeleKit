package com.skelekit.rider.ios;

import com.jetbrains.rider.run.configurations.multiPlatform.ios.IOSConnectionSessionHost;
import com.jetbrains.rider.run.multiPlatform.ios.IOSProfileState;
import net.bytebuddy.asm.Advice;

import java.util.HashMap;
import java.util.Map;

public class PrepareEnvironmentAdvice {
    @Advice.OnMethodEnter(suppress = Throwable.class)
    public static void enter(
        @Advice.Argument(2) IOSProfileState.IOSAppInfo appInfo,
        @Advice.FieldValue("connectionHost") IOSConnectionSessionHost connectionHost,
        @Advice.Argument(value = 4, readOnly = false) Map<String, String> environment) {
        if (!appInfo.isSimulator())
            return;

        String property = PreparePortsAdvice.BRIDGE_PORTS_PROPERTY_PREFIX +
            connectionHost.getProject().getLocationHash();
        String ports = System.getProperty(property);
        if (ports == null)
            return;

        int first = ports.indexOf(':');
        int second = first < 0 ? -1 : ports.indexOf(':', first + 1);
        if (second < 0 || second == ports.length() - 1)
            return;

        try {
            int reloadPort = Integer.parseInt(ports.substring(second + 1));
            if (reloadPort <= 0 || reloadPort > 65535)
                return;

            environment = new HashMap<>(environment);
            environment.put("SKELEKIT_HOT_RELOAD_PORT", Integer.toString(reloadPort));
        } catch (NumberFormatException ignored) {
        }
    }
}
