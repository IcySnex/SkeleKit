using UIKit;

namespace BareUI;

public abstract partial class Panel
{
	private protected override UIView CreateNative() =>
		new LayoutHost(this);

	private protected override void OnRealized() =>
		RealizeChildren();

	private protected override void OnUnrealized()
	{
		foreach (View child in Children)
			child.Unrealize();
	}

	partial void OnChildrenChanged()
	{
		if (IsRealized)
			RealizeChildren();
	}

	// rebuild native subviews from Children
	void RealizeChildren()
	{
		UIView host = Native;

		foreach (UIView existing in host.Subviews)
			existing.RemoveFromSuperview();

		foreach (View child in Children)
			host.AddSubview(child.Realize());

		host.SetNeedsLayout();
	}
}
