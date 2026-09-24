using System.Collections.Specialized;

namespace SkeleKit;

internal readonly record struct IndexedSourceChange(
	bool IsSectionChange,
	int Section,
	NotifyCollectionChangedAction Action,
	int OldIndex,
	int OldCount,
	int NewIndex,
	int NewCount)
{
	public static IndexedSourceChange Items(
		int section,
		NotifyCollectionChangedEventArgs change) =>
		new(
			false,
			section,
			change.Action,
			change.OldStartingIndex,
			change.OldItems?.Count ?? 0,
			change.NewStartingIndex,
			change.NewItems?.Count ?? 0);

	public static IndexedSourceChange Sections(
		NotifyCollectionChangedEventArgs change) =>
		new(
			true,
			-1,
			change.Action,
			change.OldStartingIndex,
			change.OldItems?.Count ?? 0,
			change.NewStartingIndex,
			change.NewItems?.Count ?? 0);
}
