namespace BareUI;

/// <summary>
/// <inheritdoc/>
/// </summary>
[Register(nameof(BareWindowSceneDelegate))]
public class BareWindowSceneDelegate : UIWindowSceneDelegate
{
	public override UIWindow? Window { get; set; }

	public override void WillConnect(
		UIScene scene,
		UISceneSession session,
		UISceneConnectionOptions connectionOptions)
	{
		if (scene is not UIWindowScene windowScene || BareApplication.Current is not BareApplication app)
			return;

		Window = new(windowScene)
		{
			RootViewController = app.BuildShell()
		};

		if (BareApplication.Accent is { } accent)
			Window.TintColor = accent.ToUIColor();

		Window.MakeKeyAndVisible();
	}

	public override void DidEnterBackground(
		UIScene scene) =>
		BareApplication.Current?.NotifyBackground();

	public override void WillEnterForeground(
		UIScene scene) =>
		BareApplication.Current?.NotifyForeground();
}
