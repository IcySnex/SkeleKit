namespace BareUI;

/// <summary>
/// Describes a completed drag-to-reorder: which item moved and where it went.
/// </summary>
/// <typeparam name="TItem">The item type of the collection the move happened in.</typeparam>
/// <param name="Item">The item the user moved.</param>
/// <param name="FromSection">The section the item left. 0 for a flat list.</param>
/// <param name="FromIndex">The item's index within its old section.</param>
/// <param name="ToSection">The section the item landed in. 0 for a flat list.</param>
/// <param name="ToIndex">The item's index within its new section.</param>
public sealed record ItemMove<TItem>(
	TItem Item,
	int FromSection,
	int FromIndex,
	int ToSection,
	int ToIndex);
