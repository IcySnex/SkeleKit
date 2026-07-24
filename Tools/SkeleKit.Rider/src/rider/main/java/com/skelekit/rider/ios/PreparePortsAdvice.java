package com.skelekit.rider.ios;

import net.bytebuddy.asm.Advice;

// Injected into IOSSessionHandler.preparePortsForDebugging. Increment 1: log the returned ports (via
// System.out so no plugin-classloader type is referenced from the instrumented method). Next: rewrite
// the return to our bridge ports.
public class PreparePortsAdvice {
    @Advice.OnMethodExit
    public static void exit(@Advice.Return Object ret) {
        System.out.println("[SkeleKit] preparePortsForDebugging returned -> " + ret);
    }
}
