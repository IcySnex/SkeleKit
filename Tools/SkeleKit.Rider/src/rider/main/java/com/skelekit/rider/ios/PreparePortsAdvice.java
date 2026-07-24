package com.skelekit.rider.ios;

import com.jetbrains.rider.run.multiPlatform.ios.sessions.IOSSessionHandler;
import net.bytebuddy.asm.Advice;
import net.bytebuddy.implementation.bytecode.assign.Assigner;

// Injected into IOSSessionHandler.preparePortsForDebugging. Rewrites the debug ports so the app
// connects to our bridge (portForDevice) while Rider listens on a fixed port our bridge forwards to
// (portForDebugger). Only touches IOSDebuggingPorts, which is visible to the instrumented classloader,
// so no callback into plugin code is needed here; the forwarder/bridge runs independently.
public class PreparePortsAdvice {
    // app (mlaunch -monodevelop-port) connects here; our forwarder listens
    public static final int BRIDGE_APP_PORT = 10098;
    // Rider's debugger worker listens here; our forwarder connects out to it
    public static final int RIDER_PORT = 10099;

    @Advice.OnMethodExit
    public static void exit(
        @Advice.Return(readOnly = false, typing = Assigner.Typing.DYNAMIC) Object ret) {
        if (ret instanceof IOSSessionHandler.IOSDebuggingPorts) {
            System.out.println("[SkeleKit] reroute ports: was " + ret
                + " -> device=" + BRIDGE_APP_PORT + " debugger=" + RIDER_PORT);
            ret = new IOSSessionHandler.IOSDebuggingPorts(RIDER_PORT, BRIDGE_APP_PORT);
        }
    }
}
