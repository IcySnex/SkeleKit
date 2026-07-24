package com.skelekit.rider.ios;

import com.jetbrains.rider.run.multiPlatform.ios.sessions.IOSSessionHandler;
import net.bytebuddy.asm.Advice;
import net.bytebuddy.implementation.bytecode.assign.Assigner;

// Injected into IOSSessionHandler.preparePortsForDebugging. Rewrites the debug ports so the app
// connects to our bridge (portForDevice) while Rider listens on a port the bridge forwards to
// (portForDebugger).
//
// The advice body is inlined into a class we do not own, so it may only touch types that class can
// already see: IOSDebuggingPorts and the JDK. The backend therefore hands the ports over as system
// properties rather than through a plugin class. Both unset means no bridge is listening (no
// hot-reloadable iOS project in this solution, or the bridge failed to bind), and the session is
// left completely alone.
public class PreparePortsAdvice {
    // the app (mlaunch -monodevelop-port) connects here; the bridge accepts
    public static final String APP_PORT_PROPERTY = "skelekit.ios.appPort";
    // Rider's debugger worker listens here; the bridge connects out to it
    public static final String RIDER_PORT_PROPERTY = "skelekit.ios.riderPort";

    @Advice.OnMethodExit
    public static void exit(
        @Advice.Return(readOnly = false, typing = Assigner.Typing.DYNAMIC) Object ret) {
        if (!(ret instanceof IOSSessionHandler.IOSDebuggingPorts))
            return;

        int appPort = Integer.getInteger(APP_PORT_PROPERTY, 0);
        int riderPort = Integer.getInteger(RIDER_PORT_PROPERTY, 0);
        if (appPort <= 0 || riderPort <= 0)
            return;

        System.out.println("[SkeleKit] reroute ports: was " + ret
            + " -> device=" + appPort + " debugger=" + riderPort);
        ret = new IOSSessionHandler.IOSDebuggingPorts(riderPort, appPort);
    }
}
