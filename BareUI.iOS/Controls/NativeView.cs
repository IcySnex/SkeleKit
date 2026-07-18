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
	/// <param name="view">The UIKit view to embed.</param>
	public NativeView(
		UIView view)
	{
		this.view = view;
	}

	private protected override UIView CreateNative() =>
		view;

	// caller owns it, don't dispose
	private protected override bool OwnsNative =>
		false;
}
