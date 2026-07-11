using BareUI;
using UIKit;

namespace BareUI.Gallery;

/// <summary>
/// Minimal host that drops a BareUI element tree into a view controller's safe area. A stand-in
/// until M4 ships <c>ContentView</c>/<c>BareApp</c>.
/// </summary>
public class BareHostController : UIViewController
{
	readonly View root;

	public BareHostController(
		View root)
	{
		this.root = root;
	}

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		View!.BackgroundColor = UIColor.SystemBackground;
		View.AddSubview(root.Realize());
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		// Setting the host frame drives the measure/arrange engine via LayoutSubviews.
		root.Native.Frame = View!.SafeAreaLayoutGuide.LayoutFrame;
	}
}
