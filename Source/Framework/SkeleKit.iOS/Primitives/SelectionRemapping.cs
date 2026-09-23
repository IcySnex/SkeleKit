namespace SkeleKit;

internal static class SelectionRemapping
{
	internal static bool CanReplace<TItem>(
		IReadOnlyList<TItem>? selectedItems,
		TItem oldItem)
		where TItem : class
	{
		if (selectedItems is IList<TItem> { IsReadOnly: false })
			return true;

		if (selectedItems is null)
			return true;

		foreach (TItem selected in selectedItems)
		{
			if (ReferenceEquals(selected, oldItem))
				return false;
		}

		return true;
	}
}
