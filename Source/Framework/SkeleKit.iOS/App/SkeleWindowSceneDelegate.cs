namespace SkeleKit;

/// <summary>
/// The UIKit scene delegate SkeleKit registers; it builds the app's window and shell.
/// </summary>
[Register(nameof(SkeleWindowSceneDelegate))]
public class SkeleWindowSceneDelegate : UIWindowSceneDelegate
{
	/// <summary>
	/// The window attached to the active UIKit scene.
	/// </summary>
	public override UIWindow? Window { get; set; }


	/// <summary>
	/// Connects a UIKit scene to the SkeleKit window and application shell.
	/// </summary>
	/// <param name="scene">The scene being connected.</param>
	/// <param name="session">The session associated with the scene.</param>
	/// <param name="connectionOptions">The options supplied while connecting the scene.</param>
	public override void WillConnect(
		UIScene scene,
		UISceneSession session,
		UISceneConnectionOptions connectionOptions)
	{
		if (scene is not UIWindowScene windowScene || SkeleApplication.Current is not SkeleApplication app)
			return;

		Window = new(windowScene)
		{
			RootViewController = app.BuildShell(),
			TintColor = app.Tint?.ToUIColor(),
			OverrideUserInterfaceStyle = app.UserInterfaceStyle
		};

		Window.MakeKeyAndVisible();
	}

	/// <summary>
	/// Notifies the application that its scene entered the background.
	/// </summary>
	/// <param name="scene">The scene entering the background.</param>
	public override void DidEnterBackground(
		UIScene scene) =>
		SkeleApplication.Current?.NotifyBackground();

	/// <summary>
	/// Notifies the application that its scene will enter the foreground.
	/// </summary>
	/// <param name="scene">The scene entering the foreground.</param>
	public override void WillEnterForeground(
		UIScene scene) =>
		SkeleApplication.Current?.NotifyForeground();
}
