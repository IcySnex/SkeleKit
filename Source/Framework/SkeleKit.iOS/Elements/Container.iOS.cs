namespace SkeleKit;

public abstract partial class Container
{
	private protected override bool SupportsLayeredBackground => true;

	// Most containers directly mirror LogicalChildren into their native host. Controls such as
	// CollectionView inherit the layout contract but let UIKit own the native child hierarchy.
	private protected virtual bool SynchronizesNativeChildren => true;

	private protected override UIView CreateNative() =>
		new LayoutHost(this);

	private protected override void OnRealized()
	{
		if (SynchronizesNativeChildren)
			SyncNativeChildren();
	}

	private protected override void OnUnrealized()
	{
		foreach (View child in LogicalChildren)
			child.Unrealize();
	}

	partial void OnChildrenChanged()
	{
		if (IsRealized && SynchronizesNativeChildren)
			SyncNativeChildren();
	}

	private protected override void ChildHostChanged()
	{
		if (IsRealized && SynchronizesNativeChildren)
			SyncNativeChildren();
	}

	// diff the host's subviews against logical children: keep what is still there, only add/remove/move
	void SyncNativeChildren()
	{
		UIView host = ChildHost;

		HashSet<UIView> wanted = [];
		foreach (View child in LogicalChildren)
		{
			if (child.IsRealized)
				wanted.Add(child.Native);
		}

		foreach (UIView existing in host.Subviews)
		{
			if (!wanted.Contains(existing) && !ReferenceEquals(existing, BackgroundView))
				existing.RemoveFromSuperview();
		}

		UIView[] subviews = host.Subviews;

		// in the layout host a material background holds subview 0; the effect's content view is clean
		int offset = ReferenceEquals(host, Native) && BackgroundView is not null ? 1 : 0;

		for (int index = 0; index < LogicalChildren.Count; index++)
		{
			UIView native = LogicalChildren[index].Realize();

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
