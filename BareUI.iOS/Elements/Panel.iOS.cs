using UIKit;

namespace BareUI;

public abstract partial class Panel
{
	private protected override UIView CreateNative() =>
		new LayoutHost(this);

	private protected override void OnRealized() =>
		SyncNativeChildren();

	private protected override void OnUnrealized()
	{
		foreach (View child in Children)
			child.Unrealize();
	}

	partial void OnChildrenChanged()
	{
		if (IsRealized)
			SyncNativeChildren();
	}

	// diff the host's subviews against Children: keep what is still there, only add/remove/move
	void SyncNativeChildren()
	{
		UIView host = Native;

		HashSet<UIView> wanted = [];
		foreach (View child in Children)
			if (child.IsRealized)
				wanted.Add(child.Native);

		foreach (UIView existing in host.Subviews)
			if (!wanted.Contains(existing) && !ReferenceEquals(existing, BackgroundView))
				existing.RemoveFromSuperview();

		UIView[] subviews = host.Subviews;

		// a material background holds subview 0, so the children start one slot in
		int offset = BackgroundView is null ? 0 : 1;

		for (int index = 0; index < Children.Count; index++)
		{
			UIView native = Children[index].Realize();

			// already in the right slot: leave it alone. Re-inserting a UITextField would make it
			// resign first responder, so never touch a subview that has not moved
			if (index + offset < subviews.Length && subviews[index + offset].Equals(native))
				continue;

			// InsertSubview moves a view that is already a subview, so this fixes order too
			host.InsertSubview(native, index + offset);
			subviews = host.Subviews;
		}

		host.SetNeedsLayout();
	}
}
