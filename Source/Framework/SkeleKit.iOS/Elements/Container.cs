namespace SkeleKit;

/// <summary>
/// Base for views that lay out content and can follow the containing page's UIKit margins.
/// </summary>
public abstract partial class Container : View
{
	internal ViewCollection LogicalChildren { get; }

	/// <summary>
	/// Empty space between the container's edge and its content.
	/// </summary>
	public Thickness Padding
	{
		get;
		set => Set(ref field, value);
	} = Thickness.Zero;

	/// <summary>
	/// Edges where the containing page's UIKit system margins are added to <see cref="Padding"/>.
	/// </summary>
	/// <remarks>
	/// This insets the container's content without changing the container's own bounds. The default is <see cref="LayoutEdges.None"/>.
	/// </remarks>
	public LayoutEdges SystemInsetEdges
	{
		get;
		set => Set(ref field, value);
	} = LayoutEdges.None;

	/// <summary>
	/// The fixed padding plus the selected system insets resolved from the containing page.
	/// </summary>
	protected Thickness ContentInsets
	{
		get
		{
			Thickness padding = Padding;
			LayoutEdges edges = SystemInsetEdges;
			if (edges == LayoutEdges.None)
				return padding;

			ContentView? page = null;
			for (View? view = this; view is not null; view = view.Parent)
			{
				if (view is ContentView found)
				{
					page = found;
					break;
				}
			}

			Thickness system = page?.PageSystemInsets ?? Thickness.Zero;
			bool rightToLeft = page?.PageIsRightToLeft ?? false;
			LayoutEdges leftEdge = rightToLeft ? LayoutEdges.Trailing : LayoutEdges.Leading;
			LayoutEdges rightEdge = rightToLeft ? LayoutEdges.Leading : LayoutEdges.Trailing;
			double leftInset = rightToLeft ? system.Right : system.Left;
			double rightInset = rightToLeft ? system.Left : system.Right;
			return new(
				padding.Left + (edges.HasFlag(leftEdge) ? leftInset : 0),
				padding.Top + (edges.HasFlag(LayoutEdges.Top) ? system.Top : 0),
				padding.Right + (edges.HasFlag(rightEdge) ? rightInset : 0),
				padding.Bottom + (edges.HasFlag(LayoutEdges.Bottom) ? system.Bottom : 0));
		}
	}

	/// <summary>
	/// Creates the container and its logical-child collection.
	/// </summary>
	protected Container()
	{
		LogicalChildren = new(this, SyncChildren);
	}


	void SyncChildren()
	{
		InvalidateMeasure();
		OnChildrenChanged();
	}

	partial void OnChildrenChanged();

	private protected override void PropagateBindingContext()
	{
		foreach (View child in LogicalChildren)
			child.OnBindingContextChanged();
	}

	private protected override void InvalidateChildren()
	{
		foreach (View child in LogicalChildren)
			child.InvalidateSubtree();
	}

	internal override void ReapplyVisuals()
	{
		base.ReapplyVisuals();

		foreach (View child in LogicalChildren)
			child.ReapplyVisuals();
	}

	internal override void PageWillAppear()
	{
		foreach (View child in LogicalChildren)
			child.PageWillAppear();
	}

	internal override void TintChanged()
	{
		foreach (View child in LogicalChildren)
		{
			if (child.LocalTint is null)
				child.TintChanged();
		}
	}
}
