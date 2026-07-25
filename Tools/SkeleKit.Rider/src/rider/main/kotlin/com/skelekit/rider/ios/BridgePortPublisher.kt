package com.skelekit.rider.ios

import com.intellij.openapi.application.EDT
import com.intellij.openapi.diagnostic.logger
import com.intellij.openapi.project.Project
import com.intellij.openapi.rd.util.lifetime
import com.intellij.openapi.startup.ProjectActivity
import com.jetbrains.rd.ide.model.skeleKitModel
import com.jetbrains.rider.projectView.hasSolution
import com.jetbrains.rider.projectView.solution
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext

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

        var publishedAppPort: String? = null
        var publishedRiderPort: String? = null

        fun clearPublished(message: String) {
            // Properties outlive projects. Clear only this project's values: another open solution
            // may have published its own pair after us.
            if (publishedAppPort != null && publishedRiderPort != null &&
                System.getProperty(PreparePortsAdvice.APP_PORT_PROPERTY) == publishedAppPort &&
                System.getProperty(PreparePortsAdvice.RIDER_PORT_PROPERTY) == publishedRiderPort) {
                System.clearProperty(PreparePortsAdvice.APP_PORT_PROPERTY)
                System.clearProperty(PreparePortsAdvice.RIDER_PORT_PROPERTY)
                LOG.info(message)
            }
        }

        project.lifetime.onTermination {
            clearPublished("[SkeleKit] bridge project closed; iOS debug ports left alone")
        }

        // ProjectActivity runs on a coroutine worker. RD model subscriptions must instead be made
        // on the IDE thread (or through the protocol dispatcher), otherwise Rider rejects them with
        // "Wrong thread RdProperty" before the bridge can publish its ports.
        withContext(Dispatchers.EDT) {
            val model = project.solution.skeleKitModel

            model.bridgePorts.advise(project.lifetime) { ports ->
                if (ports == null) {
                    clearPublished("[SkeleKit] bridge down; iOS debug ports left alone")
                } else {
                    val appPort = ports.appPort.toString()
                    val riderPort = ports.riderPort.toString()
                    publishedAppPort = appPort
                    publishedRiderPort = riderPort
                    System.setProperty(PreparePortsAdvice.APP_PORT_PROPERTY, appPort)
                    System.setProperty(PreparePortsAdvice.RIDER_PORT_PROPERTY, riderPort)
                    LOG.info("[SkeleKit] bridge ports: app=${ports.appPort} rider=${ports.riderPort}")
                }
            }

        }
    }

    companion object {
        private val LOG = logger<BridgePortPublisher>()
    }
}
