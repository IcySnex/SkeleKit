using Foundation;
using UIKit;

namespace BareUI.Gallery;

[Register(nameof(SceneDelegate))]
public class SceneDelegate : UIWindowSceneDelegate
{
	public override UIWindow? Window { get; set; }

	public override void WillConnect(
		UIScene scene,
		UISceneSession session,
		UISceneConnectionOptions connectionOptions)
	{
		if (scene is not UIWindowScene windowScene)
			return;

		Window = new(windowScene)
		{
			RootViewController = new BareHostController(MovieInfoPage.Build())
		};
		Window.MakeKeyAndVisible();
	}
}
