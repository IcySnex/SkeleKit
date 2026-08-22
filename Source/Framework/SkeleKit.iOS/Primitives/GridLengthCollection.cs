using System.Collections.ObjectModel;

namespace SkeleKit;

/// <summary>
/// An observable list of <see cref="GridLength"/> values used by a <see cref="Grid"/>.
/// Mutations automatically invalidate the owning grid's cached measurement.
/// </summary>
public sealed class GridLengthCollection : Collection<GridLength>
{
	readonly Action changed;


	internal GridLengthCollection(
		Action changed)
	{
		this.changed = changed;
	}


	protected override void InsertItem(
		int index,
		GridLength item)
	{
		base.InsertItem(index, item);
		changed();
	}

	protected override void SetItem(
		int index,
		GridLength item)
	{
		if (this[index] == item)
			return;

		base.SetItem(index, item);
		changed();
	}

	protected override void RemoveItem(
		int index)
	{
		base.RemoveItem(index);
		changed();
	}

	protected override void ClearItems()
	{
		if (Count == 0)
			return;

		base.ClearItems();
		changed();
	}
}
