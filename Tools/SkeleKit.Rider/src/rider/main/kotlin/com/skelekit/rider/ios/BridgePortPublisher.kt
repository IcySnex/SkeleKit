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

class BridgePortPublisher : ProjectActivity {
    override suspend fun execute(project: Project) {
        if (!project.hasSolution)
            return

        var published: String? = null

        fun clearPublished(message: String) {
            if (published != null &&
                System.getProperty(PreparePortsAdvice.BRIDGE_PORTS_PROPERTY) == published) {
                System.clearProperty(PreparePortsAdvice.BRIDGE_PORTS_PROPERTY)
                LOG.info(message)
            }
        }

        project.lifetime.onTermination {
            clearPublished("[SkeleKit] bridge project closed; iOS debug ports left alone")
        }

        withContext(Dispatchers.EDT) {
            val model = project.solution.skeleKitModel

            model.bridgePorts.advise(project.lifetime) { value ->
                if (value == null) {
                    clearPublished("[SkeleKit] bridge down; iOS debug ports left alone")
                } else {
                    published = value
                    System.setProperty(PreparePortsAdvice.BRIDGE_PORTS_PROPERTY, value)
                    LOG.info("[SkeleKit] bridge ports: $value")
                }
            }
        }
    }

    companion object {
        private val LOG = logger<BridgePortPublisher>()
    }
}
