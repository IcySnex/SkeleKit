using System.Windows.Input;

namespace BareUI;

/// <summary>
/// Which edge of a row a swipe action lives on.
/// </summary>
public enum SwipeSide
{
	/// <summary>
	/// Revealed by swiping from the trailing edge (the usual place for Delete).
	/// </summary>
	Trailing,

	/// <summary>
	/// Revealed by swiping from the leading edge.
	/// </summary>
	Leading
}

/// <summary>
/// An action revealed by swiping a row.
/// </summary>
public sealed class SwipeAction
{
	/// <summary>
	/// The action's title.
	/// </summary>
	public string? Text { get; set; }

	/// <summary>
	/// An SF Symbol name shown on the action.
	/// </summary>
	public string? Icon { get; set; }

	/// <summary>
	/// Which edge reveals the action.
	/// </summary>
	public SwipeSide Side { get; set; } = SwipeSide.Trailing;

	/// <summary>
	/// Whether the action is styled as destructive, and runs on a full swipe.
	/// </summary>
	public bool IsDestructive { get; set; }

	/// <summary>
	/// The action's background color, or null for the system default.
	/// </summary>
	public Color? Background { get; set; }

	/// <summary>
	/// Invoked with the row's item.
	/// </summary>
	public ICommand? Command { get; set; }
}

/// <summary>
/// An entry in a row's long-press context menu.
/// </summary>
public sealed class MenuAction
{
	/// <summary>
	/// The entry's title.
	/// </summary>
	public string Text { get; set; } = "";

	/// <summary>
	/// An SF Symbol name shown beside the title.
	/// </summary>
	public string? Icon { get; set; }

	/// <summary>
	/// Whether the entry is styled as destructive.
	/// </summary>
	public bool IsDestructive { get; set; }

	/// <summary>
	/// Invoked with the row's item.
	/// </summary>
	public ICommand? Command { get; set; }
}
