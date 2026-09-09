namespace SkeleKit;

/// <summary>
/// Base for views that measure and arrange one or more children.
/// </summary>
public abstract partial class Panel : View
{
	/// <summary>
	/// The panel's children. Collection-initializer friendly.
	/// </summary>
	public ViewCollection Children { get; }

	/// <summary>
	/// Empty space between the panel's edge and its children.
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
	/// This insets the panel's children without changing the panel's own bounds. The default is <see cref="LayoutEdges.None"/>.
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
	/// Creates the panel and its <see cref="Children"/> collection.
	/// </summary>
	protected Panel()
	{
		Children = new(this, SyncChildren);
	}


	void SyncChildren()
	{
		InvalidateMeasure();
		OnChildrenChanged();
	}

	partial void OnChildrenChanged();

	private protected override void PropagateBindingContext()
	{
		foreach (View child in Children)
			child.OnBindingContextChanged();
	}

	private protected override void InvalidateChildren()
	{
		foreach (View child in Children)
			child.InvalidateSubtree();
	}

	internal override void ReapplyVisuals()
	{
		base.ReapplyVisuals();

		foreach (View child in Children)
			child.ReapplyVisuals();
	}

	internal override void PageWillAppear()
	{
		foreach (View child in Children)
			child.PageWillAppear();
	}

	internal override void TintChanged()
	{
		foreach (View child in Children)
		{
			if (child.LocalTint is null)
				child.TintChanged();
		}
	}
}
