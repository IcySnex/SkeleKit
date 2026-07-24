@file:Suppress("unused")

package model.rider

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.SolutionModel

object SkeleKitModel : Ext(SolutionModel.Solution) {

    private val BridgePorts = structdef {
        field("appPort", int)
        field("riderPort", int)
    }

    init {
        // backend -> frontend: the loopback ports the bridge bound. The frontend publishes them as
        // system properties so the port-rerouting advice can send an iOS debug session through the
        // bridge. Stays null when the solution has no hot-reloadable .NET iOS project, which is what
        // keeps us out of the way of unrelated iOS runs.
        property("bridgePorts", BridgePorts.nullable)

        // backend -> frontend: bridge / hot-reload status lines
        signal("log", string)
    }
}
