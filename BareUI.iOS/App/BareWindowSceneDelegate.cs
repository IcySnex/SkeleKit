namespace BareUI;

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
		Window.MakeKeyAndVisible();
	}
}
