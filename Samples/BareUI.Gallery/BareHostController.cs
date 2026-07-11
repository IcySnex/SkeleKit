using BareUI;
using ObjCRuntime;
using UIKit;

namespace BareUI.Gallery;

/// <summary>
/// Minimal host that drops a BareUI element tree into a view controller. A stand-in until M4 ships
/// <c>ContentView</c>/<c>BareApp</c>.
/// </summary>
public class BareHostController : UIViewController
{
	readonly View? root;
	readonly string? title;

	UITapGestureRecognizer? dismissKeyboard;

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

	// a scrolling root must sit under the nav bar, or the bar has nothing to blur
	UIScrollView? ScrollRoot =>
		root?.IsRealized is true ? root.Native as UIScrollView : null;

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		Title = title;
		View!.BackgroundColor = UIColor.SystemBackground;

		if (root is null)
			return;

		View.AddSubview(root.Realize());

		// numeric keyboards have no return key, so tapping outside is the only way out
		dismissKeyboard = new(() => View.EndEditing(true))
		{
			CancelsTouchesInView = false
		};
		View.AddGestureRecognizer(dismissKeyboard);

		if (ScrollRoot is { } scroll)
			scroll.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Always;
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		if (root is null)
			return;

		// frame set drives measure/arrange via LayoutSubviews
		root.Native.Frame = ScrollRoot is not null
			? View!.Bounds
			: View!.SafeAreaLayoutGuide.LayoutFrame;
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
