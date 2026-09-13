namespace SkeleKit;

/// <summary>
/// The UIKit application delegate SkeleKit registers for you.
/// </summary>
[Register(nameof(SkeleApplicationDelegate))]
public class SkeleApplicationDelegate : UIApplicationDelegate
{
	/// <summary>
	/// The application window managed by UIKit.
	/// </summary>
	public override UIWindow? Window { get; set; }

	/// <summary>
	/// Marks UIKit as ready after application lifecycle services have completed startup.
	/// </summary>
	/// <param name="application">The launched UIKit application.</param>
	/// <param name="launchOptions">The options supplied for this launch.</param>
	/// <returns>True to complete application launch.</returns>
	public override bool FinishedLaunching(
		UIApplication application,
		NSDictionary? launchOptions)
	{
		SkeleApplication.Current?.NotifyNativeApplicationStarted();
		return true;
	}

	/// <summary>
	/// Notifies application lifecycle services that UIKit is terminating the process.
	/// </summary>
	/// <param name="application">The terminating UIKit application.</param>
	public override void WillTerminate(
		UIApplication application) =>
		SkeleApplication.Current?.NotifyStopping();
}
