namespace SkeleKit;

/// <summary>
/// The UIKit scene delegate SkeleKit registers; it builds the app's window and shell.
/// </summary>
[Register(nameof(SkeleWindowSceneDelegate))]
public class SkeleWindowSceneDelegate : UIWindowSceneDelegate
{
	public override UIWindow? Window { get; set; }

	
	public override void WillConnect(
		UIScene scene,
		UISceneSession session,
		UISceneConnectionOptions connectionOptions)
	{
		if (scene is not UIWindowScene windowScene || SkeleApplication.Current is not SkeleApplication app)
			return;

		Window = new(windowScene)
		{
			RootViewController = app.BuildShell()
		};

		if (View.AppAccent is Color accent)
			Window.TintColor = accent.ToUIColor();

		Window.MakeKeyAndVisible();
	}

	public override void DidEnterBackground(
		UIScene scene) =>
		SkeleApplication.Current?.NotifyBackground();

	public override void WillEnterForeground(
		UIScene scene) =>
		SkeleApplication.Current?.NotifyForeground();
}
