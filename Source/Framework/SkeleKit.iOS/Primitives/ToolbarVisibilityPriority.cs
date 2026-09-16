namespace SkeleKit;

/// <summary>
/// How long a toolbar item remains visible when space is constrained.
/// </summary>
public enum ToolbarVisibilityPriority
{
	/// <summary>
	/// Uses the system's normal visibility priority.
	/// </summary>
	Standard,

	/// <summary>
	/// Lets the item yield space before standard-priority items.
	/// </summary>
	Low,

	/// <summary>
	/// Keeps the item visible longer than standard-priority items.
	/// </summary>
	High
}
