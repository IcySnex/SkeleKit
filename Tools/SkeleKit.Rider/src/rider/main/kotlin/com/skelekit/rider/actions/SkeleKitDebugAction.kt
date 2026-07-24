package com.skelekit.rider.actions

import com.intellij.openapi.actionSystem.AnAction
import com.intellij.openapi.actionSystem.AnActionEvent
import com.intellij.openapi.diagnostic.logger
import com.jetbrains.rd.ide.model.StartBridgeRequest
import com.jetbrains.rd.ide.model.skeleKitModel
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rider.projectView.solution

// Temporary trigger to prove the frontend<->backend rd pipe. Replaced by the real Run/Debug
// executors + device dropdown next.
class SkeleKitDebugAction : AnAction() {
    override fun actionPerformed(e: AnActionEvent) {
        val project = e.project ?: return
        val model = project.solution.skeleKitModel

        model.log.advise(Lifetime.Eternal) { line -> LOG.info("[SkeleKit] $line") }

        val request = StartBridgeRequest(
            assemblyName = "SkeleKit.Gallery",
            deployedDll = "",
            projectDir = project.basePath ?: "",
            cscArgs = "",
        )
        model.startBridge.start(Lifetime.Eternal, request).result.advise(Lifetime.Eternal) { result ->
            val info = result.unwrap()
            LOG.info("[SkeleKit] bridge ports: ide=${info.idePort} app=${info.appPort}")
        }
    }

    companion object {
        private val LOG = logger<SkeleKitDebugAction>()
    }
}
