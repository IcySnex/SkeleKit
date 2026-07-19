namespace BareUI;

/// <summary>
/// A <see cref="View"/> that lays out one or more children, hosted by a native LayoutHost.
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

	internal override void PageAppeared()
	{
		foreach (View child in Children)
			child.PageAppeared();
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
