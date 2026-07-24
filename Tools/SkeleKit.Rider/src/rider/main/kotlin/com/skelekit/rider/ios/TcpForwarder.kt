package com.skelekit.rider.ios

import com.intellij.openapi.diagnostic.logger
import java.net.InetAddress
import java.net.ServerSocket
import java.net.Socket
import kotlin.concurrent.thread

// Increment 1: a plain TCP forwarder proving the rerouted debug session works end-to-end through us.
// Listens where the app connects (BRIDGE_APP_PORT) and forwards each connection to where Rider's
// debugger listens (RIDER_PORT). No sdb parsing yet; hot-reload injection replaces this next.
object TcpForwarder {
    private val LOG = logger<TcpForwarder>()

    @Volatile
    private var started = false

    @Synchronized
    fun ensureStarted(
        listenPort: Int,
        targetPort: Int,
    ) {
        if (started)
            return
        started = true

        thread(isDaemon = true, name = "skele-forwarder") {
            try {
                val server = ServerSocket(listenPort, 50, InetAddress.getLoopbackAddress())
                LOG.info("[SkeleKit] forwarder listening on $listenPort -> $targetPort")
                while (true) {
                    val client = server.accept()
                    accept(client, targetPort)
                }
            } catch (throwable: Throwable) {
                LOG.warn("[SkeleKit] forwarder failed", throwable)
            }
        }
    }

    private fun accept(
        client: Socket,
        targetPort: Int,
    ) {
        thread(isDaemon = true, name = "skele-forward-conn") {
            try {
                val target = connectWithRetry(targetPort)
                pump(client, target)
                pump(target, client)
            } catch (throwable: Throwable) {
                LOG.warn("[SkeleKit] forward connection failed", throwable)
                runCatching { client.close() }
            }
        }
    }

    private fun connectWithRetry(
        port: Int,
    ): Socket {
        repeat(200) {
            try {
                return Socket(InetAddress.getLoopbackAddress(), port)
            } catch (_: Exception) {
                Thread.sleep(50)
            }
        }
        return Socket(InetAddress.getLoopbackAddress(), port)
    }

    private fun pump(
        from: Socket,
        to: Socket,
    ) {
        thread(isDaemon = true, name = "skele-pump") {
            try {
                from.getInputStream().copyTo(to.getOutputStream())
            } catch (_: Exception) {
            } finally {
                runCatching { to.shutdownOutput() }
            }
        }
    }
}
