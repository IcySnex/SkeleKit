using System.Collections.ObjectModel;

namespace SkeleKit;

/// <summary>
/// An observable <see cref="GridLength"/> list that invalidates its owning <see cref="Grid"/> when changed.
/// </summary>
public sealed class GridLengthCollection : Collection<GridLength>
{
	readonly Action changed;


	internal GridLengthCollection(
		Action changed)
	{
		this.changed = changed;
	}


	/// <inheritdoc/>
	protected override void InsertItem(
		int index,
		GridLength item)
	{
		base.InsertItem(index, item);
		changed();
	}

	/// <inheritdoc/>
	protected override void SetItem(
		int index,
		GridLength item)
	{
		if (this[index] == item)
			return;

		base.SetItem(index, item);
		changed();
	}

	/// <inheritdoc/>
	protected override void RemoveItem(
		int index)
	{
		base.RemoveItem(index);
		changed();
	}

	/// <inheritdoc/>
	protected override void ClearItems()
	{
		if (Count == 0)
			return;

		base.ClearItems();
		changed();
	}
}
