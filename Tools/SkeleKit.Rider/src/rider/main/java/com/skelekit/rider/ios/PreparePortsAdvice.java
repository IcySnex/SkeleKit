package com.skelekit.rider.ios;

import com.jetbrains.rider.run.configurations.multiPlatform.ios.IOSConnectionSessionHost;
import com.jetbrains.rider.run.multiPlatform.ios.IOSProfileState;
import com.jetbrains.rider.run.multiPlatform.ios.sessions.IOSSessionHandler;
import net.bytebuddy.asm.Advice;
import net.bytebuddy.implementation.bytecode.assign.Assigner;

// Injected into IOSSessionHandler.preparePortsForDebugging. Simulator sessions are routed through
// the bridge. Physical devices retain Rider's native USB or Wi-Fi transport.
public class PreparePortsAdvice {
    public static final String BRIDGE_PORTS_PROPERTY_PREFIX = "skelekit.ios.bridgePorts.";

    public static String bridgePortsProperty(String locationHash) {
        return BRIDGE_PORTS_PROPERTY_PREFIX + locationHash;
    }

    @Advice.OnMethodExit(suppress = Throwable.class)
    public static void exit(
        @Advice.Argument(1) IOSProfileState.IOSAppInfo appInfo,
        @Advice.FieldValue("connectionHost") IOSConnectionSessionHost connectionHost,
        @Advice.Return(readOnly = false, typing = Assigner.Typing.DYNAMIC) Object ret) {
        if (!appInfo.isSimulator() || !(ret instanceof IOSSessionHandler.IOSDebuggingPorts))
            return;

        String property = BRIDGE_PORTS_PROPERTY_PREFIX + connectionHost.getProject().getLocationHash();
        String ports = null;
        for (int attempt = 0; attempt < 50 && ports == null; attempt++) {
            ports = System.getProperty(property);
            if (ports == null) {
                try {
                    Thread.sleep(50);
                } catch (InterruptedException ignored) {
                    Thread.currentThread().interrupt();
                    return;
                }
            }
        }

        if (ports == null)
            return;
        int firstSeparator = ports.indexOf(':');
        if (firstSeparator < 1)
            return;

        int secondSeparator = ports.indexOf(':', firstSeparator + 1);

        try {
            int appPort = Integer.parseInt(ports.substring(0, firstSeparator));
            int riderPort = Integer.parseInt(secondSeparator < 0
                ? ports.substring(firstSeparator + 1)
                : ports.substring(firstSeparator + 1, secondSeparator));
            if (appPort > 0 && appPort <= 65535 && riderPort > 0 && riderPort <= 65535)
                //noinspection ReassignedVariable
                ret = new IOSSessionHandler.IOSDebuggingPorts(riderPort, appPort);
        } catch (NumberFormatException ignored) {
        }
    }
}
