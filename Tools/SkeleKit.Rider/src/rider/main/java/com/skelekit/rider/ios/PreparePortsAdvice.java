package com.skelekit.rider.ios;

import com.jetbrains.rider.run.multiPlatform.ios.IOSProfileState;
import com.jetbrains.rider.run.multiPlatform.ios.sessions.IOSSessionHandler;
import net.bytebuddy.asm.Advice;
import net.bytebuddy.implementation.bytecode.assign.Assigner;

// Injected into IOSSessionHandler.preparePortsForDebugging. Simulator sessions are routed through
// the bridge. Physical devices retain Rider's native USB or Wi-Fi transport.
public class PreparePortsAdvice {
    public static final String BRIDGE_PORTS_PROPERTY = "skelekit.ios.bridgePorts";

    @Advice.OnMethodExit
    public static void exit(
        @Advice.Argument(1) IOSProfileState.IOSAppInfo appInfo,
        @Advice.Return(readOnly = false, typing = Assigner.Typing.DYNAMIC) Object ret) {
        if (!appInfo.isSimulator() || !(ret instanceof IOSSessionHandler.IOSDebuggingPorts))
            return;

        String ports = System.getProperty(BRIDGE_PORTS_PROPERTY);
        if (ports == null)
            return;

        int separator = ports.indexOf(':');
        if (separator < 1)
            return;

        try {
            int appPort = Integer.parseInt(ports.substring(0, separator));
            int riderPort = Integer.parseInt(ports.substring(separator + 1));
            if (appPort > 0 && riderPort > 0)
                ret = new IOSSessionHandler.IOSDebuggingPorts(riderPort, appPort);
        } catch (NumberFormatException ignored) {
        }
    }
}
