package com.skelekit.rider.ios

import com.intellij.openapi.diagnostic.logger
import com.intellij.openapi.project.Project
import com.intellij.openapi.startup.ProjectActivity
import com.jetbrains.rd.platform.util.lifetime
import com.jetbrains.rd.ide.model.skeleKitModel
import com.jetbrains.rider.projectView.hasSolution
import com.jetbrains.rider.projectView.solution

// Mirrors the backend bridge's ports into system properties, which is how PreparePortsAdvice reads
// them (its inlined body cannot see plugin classes). Null ports means the solution has nothing to hot
// reload, and the properties are cleared so iOS debug sessions run untouched.
//
// The properties are JVM-global, so with several solutions open in one Rider the last one loaded owns
// them. That only costs the other solution its hot reload: its debug session still relays through the
// bridge untouched, and the engine declines to apply deltas to an assembly it does not know.
class BridgePortPublisher : ProjectActivity {
    override suspend fun execute(project: Project) {
        if (!project.hasSolution)
            return

        project.solution.skeleKitModel.bridgePorts.advise(project.lifetime) { ports ->
            if (ports == null) {
                System.clearProperty(PreparePortsAdvice.APP_PORT_PROPERTY)
                System.clearProperty(PreparePortsAdvice.RIDER_PORT_PROPERTY)
                LOG.info("[SkeleKit] bridge down; iOS debug ports left alone")
            } else {
                System.setProperty(PreparePortsAdvice.APP_PORT_PROPERTY, ports.appPort.toString())
                System.setProperty(PreparePortsAdvice.RIDER_PORT_PROPERTY, ports.riderPort.toString())
                LOG.info("[SkeleKit] bridge ports: app=${ports.appPort} rider=${ports.riderPort}")
            }
        }
    }

    companion object {
        private val LOG = logger<BridgePortPublisher>()
    }
}
