#if IOS
using UIKit;

namespace BareUI;

/// <summary>
/// Escape hatch to embed any UIKit view in a BareUI tree.
/// </summary>
public class NativeView : Control
{
	readonly UIView view;

	/// <summary>
	/// Creates a wrapper for the given UIKit view.
	/// </summary>
	public NativeView(
		UIView view)
	{
		this.view = view;
	}

	private protected override UIView CreateNative() =>
		view;

	// The wrapped view is caller-owned; Unrealize must not dispose it.
	private protected override bool OwnsNative =>
		false;
}
#endif
