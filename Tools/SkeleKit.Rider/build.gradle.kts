import com.jetbrains.plugin.structure.base.utils.isFile
import org.jetbrains.intellij.platform.gradle.Constants

plugins {
    id("java")
    alias(libs.plugins.kotlinJvm)
    id("org.jetbrains.intellij.platform") version "2.10.5"
}

val DotnetSolution: String by project
val BuildConfiguration: String by project
val DotnetPluginId: String by project
val ProductVersion: String by project

allprojects {
    repositories {
        maven { setUrl("https://cache-redirector.jetbrains.com/maven-central") }
    }
}

repositories {
    intellijPlatform {
        defaultRepositories()
        jetbrainsRuntime()
    }
}

version = property("PluginVersion") as String

sourceSets {
    main {
        java.srcDir("src/rider/main/java")
        kotlin.srcDir("src/rider/main/kotlin")
        resources.srcDir("src/rider/main/resources")
    }
}

dependencies {
    intellijPlatform {
        rider(ProductVersion) {
            useInstaller = false
        }
        jetbrainsRuntime()
    }

    // bundled with the plugin: bytecode instrumentation to reroute the iOS debug ports through our
    // bridge (the session handlers are final + in an off-classpath module, so we patch the base method)
    implementation("net.bytebuddy:byte-buddy:1.15.11")
    implementation("net.bytebuddy:byte-buddy-agent:1.15.11")
}

intellijPlatform {
    // it starts a headless IDE to index settings pages, which fails while Rider is open and indexes
    // nothing anyway: the plugin contributes no settings UI
    buildSearchableOptions = false

    pluginConfiguration {
        ideaVersion {
            sinceBuild = "261"
            untilBuild = provider { null }
        }
    }
}

tasks.processResources {
    from("dependencies.json") { into("META-INF") }
}

// Gradle (launched from Rider or a non-login shell) may not have brew's dotnet on PATH.
fun dotnetExecutable(): String {
    providers.environmentVariable("DOTNET_PATH").orNull?.let { if (file(it).exists()) return it }
    return listOf(
        "/opt/homebrew/bin/dotnet",
        "/usr/local/bin/dotnet",
    ).firstOrNull { file(it).exists() } ?: "dotnet"
}

// Build the .NET backend (uses the generated model, so it needs rdgen first).
val compileDotNet by tasks.registering {
    dependsOn(":protocol:rdgen")
    doLast {
        providers.exec {
            executable(dotnetExecutable())
            args("build", DotnetSolution, "-c", BuildConfiguration, "/p:HostFullIdentifier=")
            workingDir(rootDir)
        }.result.get()
    }
}

// Frontend Kotlin consumes the generated model too.
tasks.named("compileKotlin") {
    dependsOn(":protocol:rdgen")
}

tasks.prepareSandbox {
    dependsOn(compileDotNet)

    val outputFolder = "${rootDir}/src/dotnet/${DotnetPluginId}/bin/${DotnetPluginId}/${BuildConfiguration}"

    // only our dll — the host provides Roslyn + BCL (we compile against its bundled Microsoft.CodeAnalysis)
    val dllFiles = listOf("$outputFolder/${DotnetPluginId}.dll", "$outputFolder/${DotnetPluginId}.pdb")
    dllFiles.forEach { f -> from(file(f)) { into("${rootProject.name}/dotnet") } }

    doLast {
        if (!file("$outputFolder/${DotnetPluginId}.dll").exists())
            throw RuntimeException("backend dll missing at $outputFolder")
    }
}

tasks.runIde {
    maxHeapSize = "1500m"
    // sandbox: auto-trust the opened solution so it loads without the "Trust Project?" dialog
    systemProperty("idea.trust.all.projects", "true")
}

// Expose rider-model.jar so :protocol can generate against the IDE model.
val riderModel: Configuration by configurations.creating {
    isCanBeConsumed = true
    isCanBeResolved = false
}

artifacts {
    add(riderModel.name, provider {
        intellijPlatform.platformPath.resolve("lib/rd/rider-model.jar").also {
            check(it.isFile) { "rider-model.jar is not found at $it" }
        }
    }) {
        builtBy(Constants.Tasks.INITIALIZE_INTELLIJ_PLATFORM_PLUGIN)
    }
}
