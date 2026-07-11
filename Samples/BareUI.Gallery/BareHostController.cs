using BareUI;
using ObjCRuntime;
using UIKit;

namespace BareUI.Gallery;

/// <summary>
/// Minimal host that drops a BareUI element tree into a view controller's safe area. A stand-in
/// until M4 ships <c>ContentView</c>/<c>BareApp</c>.
/// </summary>
public class BareHostController : UIViewController
{
	readonly View? root;
	readonly string? title;

	public BareHostController(
		View root,
		string? title = null)
	{
		this.root = root;
		this.title = title;
	}

	// marshaller needs this; SceneDelegate keeps the managed ref so it stays unused
	public BareHostController(
		NativeHandle handle) : base(handle)
	{ }

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		Title = title;
		View!.BackgroundColor = UIColor.SystemBackground;

		if (root is not null)
			View.AddSubview(root.Realize());
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		// Setting the host frame drives the measure/arrange engine via LayoutSubviews.
		if (root is not null)
			root.Native.Frame = View!.SafeAreaLayoutGuide.LayoutFrame;
	}

	public override void ViewDidDisappear(
		bool animated)
	{
		base.ViewDidDisappear(animated);

		// popped for good, not just covered
		if (IsMovingFromParentViewController)
			root?.Unrealize();
	}
}
