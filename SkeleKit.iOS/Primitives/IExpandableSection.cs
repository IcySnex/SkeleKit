namespace SkeleKit;

/// <summary>
/// A <see cref="ISection{TItem}"/> whose items collapse behind its header.
/// </summary>
/// <remarks>
/// The header shows a chevron and tapping it toggles <see cref="IsExpanded"/>.
/// </remarks>
/// <typeparam name="TItem">The element type of the group.</typeparam>
public interface IExpandableSection<out TItem> : ISection<TItem>
{
	/// <summary>
	/// Whether the group's items are shown.
	/// </summary>
	bool IsExpanded { get; set; }
}
