package com.skelekit.rider.ios

import com.intellij.openapi.diagnostic.logger
import com.intellij.openapi.project.Project
import com.intellij.openapi.startup.ProjectActivity
import net.bytebuddy.agent.ByteBuddyAgent
import net.bytebuddy.agent.builder.AgentBuilder
import net.bytebuddy.asm.Advice
import net.bytebuddy.matcher.ElementMatchers.named
import java.util.concurrent.atomic.AtomicBoolean

// Installs a self-attached ByteBuddy agent and instruments Rider's iOS port preparation. The
// concrete session handlers are final and off our classpath, but the shared logic lives on this
// base type. Retransformation covers the already-loaded case.
class IosPortInstrumenter : ProjectActivity {
    override suspend fun execute(project: Project) {
        if (!installed.compareAndSet(false, true))
            return

        try {
            val instrumentation = ByteBuddyAgent.install()

            AgentBuilder.Default()
                .with(AgentBuilder.RedefinitionStrategy.RETRANSFORMATION)
                .with(AgentBuilder.InitializationStrategy.NoOp.INSTANCE)
                .with(AgentBuilder.TypeStrategy.Default.REDEFINE)
                .disableClassFormatChanges()
                .type(named("com.jetbrains.rider.run.multiPlatform.ios.sessions.IOSSessionHandler"))
                .transform { builder, _, _, _, _ ->
                    builder.visit(Advice.to(PreparePortsAdvice::class.java).on(named("preparePortsForDebugging")))
                }
                .installOn(instrumentation)

            LOG.info("[SkeleKit] iOS port instrumenter installed")
        } catch (throwable: Throwable) {
            LOG.warn("[SkeleKit] iOS port instrumentation failed", throwable)
        }
    }

    companion object {
        private val installed = AtomicBoolean(false)
        private val LOG = logger<IosPortInstrumenter>()
    }
}
