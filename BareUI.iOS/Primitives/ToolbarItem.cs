using System.Windows.Input;

namespace BareUI;

/// <summary>
/// Which side of the navigation bar a toolbar item sits on.
/// </summary>
public enum ToolbarSide
{
	/// <summary>
	/// Trailing edge (the right, in a left-to-right layout).
	/// </summary>
	Trailing,

	/// <summary>
	/// Leading edge.
	/// </summary>
	Leading
}

/// <summary>
/// A button in the page's navigation bar.
/// </summary>
public sealed class ToolbarItem
{
	/// <summary>
	/// The item's text, or null when it shows only an icon.
	/// </summary>
	public string? Text { get; set; }

	/// <summary>
	/// An SF Symbol name, or null for a text-only item.
	/// </summary>
	public string? Icon { get; set; }

	/// <summary>
	/// Which side of the bar the item sits on.
	/// </summary>
	public ToolbarSide Side { get; set; } = ToolbarSide.Trailing;

	/// <summary>
	/// Whether the item is rendered as the prominent action.
	/// </summary>
	public bool IsPrimary { get; set; }

	/// <summary>
	/// Menu entries shown on tap instead of invoking <see cref="Command"/>. Empty for a plain item.
	/// </summary>
	public IList<MenuAction> Menu { get; } = [];

	/// <summary>
	/// Invoked when the item is tapped; its CanExecute drives the enabled state.
	/// </summary>
	public ICommand? Command { get; set; }

	/// <summary>
	/// The parameter passed to <see cref="Command"/>.
	/// </summary>
	public object? CommandParameter { get; set; }
}
