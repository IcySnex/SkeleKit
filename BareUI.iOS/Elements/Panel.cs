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
}
