@file:Suppress("unused")

package model.rider

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.SolutionModel

object SkeleKitModel : Ext(SolutionModel.Solution) {

    private val StartBridgeRequest = structdef {
        field("assemblyName", string)
        field("deployedDll", string)
        field("projectDir", string)
        field("cscArgs", string)
    }

    private val BridgeInfo = structdef {
        field("idePort", int)
        field("appPort", int)
    }

    init {
        // frontend Debug action -> backend: start the sdb proxy, get its ports back
        call("startBridge", StartBridgeRequest, BridgeInfo)
        // frontend Run/stop -> backend: tear the proxy down
        call("stopBridge", void, void)
        // backend -> frontend: proxy / hot-reload status lines for the tool window
        signal("log", string)
    }
}
