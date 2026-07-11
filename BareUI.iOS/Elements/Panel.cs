#if IOS
using UIKit;
#endif

namespace BareUI;

/// <summary>
/// A <see cref="View"/> that lays out one or more children, hosted by a native LayoutHost.
/// </summary>
public abstract class Panel : View
{
	/// <summary>
	/// The panel's children. Collection-initializer friendly.
	/// </summary>
	public ViewCollection Children { get; }

	protected Panel()
	{
		Children = new(SyncChildren);
	}

#if IOS
	private protected override UIView CreateNative() =>
		new LayoutHost(this);

	private protected override void OnRealized() =>
		RealizeChildren();

	private protected override void OnUnrealized()
	{
		foreach (View child in Children)
			child.Unrealize();
	}

	void SyncChildren()
	{
		if (IsRealized)
			RealizeChildren();
	}

	// Rebuilds the host's native subviews from the current children.
	void RealizeChildren()
	{
		UIView host = Native;

		foreach (UIView existing in host.Subviews)
			existing.RemoveFromSuperview();

		foreach (View child in Children)
			host.AddSubview(child.Realize());

		host.SetNeedsLayout();
	}
#else
	void SyncChildren()
	{ }
#endif
}
