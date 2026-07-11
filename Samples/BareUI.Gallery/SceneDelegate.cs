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

		UINavigationController? navigation = null;

		// Pushes a demo page onto the nav stack; captured by the pure-BareUI MenuPage tree so its
		// buttons never need to see UIKit.
		void Push(
			string title,
			View page) =>
			navigation!.PushViewController(new BareHostController(page, title), true);

		navigation = new UINavigationController(new BareHostController(MenuPage.Build(Push), "BareUI Gallery"));

		Window = new(windowScene)
		{
			RootViewController = navigation
		};
		Window.MakeKeyAndVisible();

		// Debug convenience: launch straight into a page via `SIMCTL_CHILD_GALLERY_PAGE=<Title>`.
		string? autoPage = Environment.GetEnvironmentVariable("GALLERY_PAGE");
		if (autoPage is not null && MenuPage.TryBuild(autoPage, out View? page))
			Push(autoPage, page);
	}
}
