namespace SkeleKit;

/// <summary>
/// Receives application-wide startup, scene foreground and background, and shutdown notifications.
/// </summary>
/// <remarks>
/// Startup completes before UIKit creates the first window. Foreground, background, and shutdown
/// notifications originate in UIKit callbacks and therefore cannot delay the operating system.
/// Shutdown is best-effort because iOS can terminate a suspended application without notifying it.
/// </remarks>
public interface IApplicationLifecycle
{
	/// <summary>
	/// Runs once after <see cref="SkeleApplication.Current"/> is assigned and before UIKit starts.
	/// </summary>
	/// <remarks>Keep startup short. UIKit objects are not available until this method completes.</remarks>
	Task StartAsync() =>
		Task.CompletedTask;

	/// <summary>
	/// Runs when an application scene returns to the foreground.
	/// </summary>
	Task EnterForegroundAsync() =>
		Task.CompletedTask;

	/// <summary>
	/// Runs when an application scene enters the background.
	/// </summary>
	Task EnterBackgroundAsync() =>
		Task.CompletedTask;

	/// <summary>
	/// Runs at most once when UIKit reports that the application is stopping.
	/// </summary>
	Task StopAsync() =>
		Task.CompletedTask;
}
