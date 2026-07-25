@file:Suppress("unused")

package model.rider

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rider.model.nova.ide.SolutionModel

object SkeleKitModel : Ext(SolutionModel.Solution) {

    init {
        property("bridgePorts", string.nullable)
    }
}
