using Foundation;
using UIKit;

namespace BareUI.Gallery;

[Register(nameof(SceneDelegate))]
public class SceneDelegate : UIWindowSceneDelegate
{
	public override UIWindow? Window { get; set; }

	// nav retains its VCs natively only — without a managed ref the GC eats the peers. BareApp takes over in M4.
	readonly List<BareHostController> hosts = [];

	// nav.Delegate is weak
	HostKeeper? keeper;

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
			View page)
		{
			BareHostController host = new(page, title);

			hosts.Add(host);
			navigation!.PushViewController(host, true);
		}

		BareHostController menu = new(MenuPage.Build(Push), "BareUI Gallery");
		hosts.Add(menu);

		keeper = new(hosts);

		navigation = new UINavigationController(menu);
		navigation.Delegate = keeper;

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

	// drops refs to popped hosts
	sealed class HostKeeper(
		List<BareHostController> hosts) : UINavigationControllerDelegate
	{
		public override void DidShowViewController(
			UINavigationController navigationController,
			UIViewController viewController,
			bool animated)
		{
			UIViewController[] stack = navigationController.ViewControllers ?? [];

			hosts.RemoveAll(host => !stack.Contains(host));
		}
	}
}
