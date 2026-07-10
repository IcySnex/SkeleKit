using BareUI.Primitives;
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

		Thickness probe = new(16, 8);

		UIViewController rootViewController = new();
		rootViewController.View!.BackgroundColor = UIColor.SystemBackground;
		rootViewController.View.AddSubview(new UILabel(windowScene.CoordinateSpace.Bounds)
		{
			Text = $"BareUI Gallery — wired up (Thickness.Horizontal = {probe.Horizontal})",
			TextAlignment = UITextAlignment.Center,
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
			Lines = 0
		});

		Window = new(windowScene)
		{
			RootViewController = rootViewController
		};
		Window.MakeKeyAndVisible();
	}
}
