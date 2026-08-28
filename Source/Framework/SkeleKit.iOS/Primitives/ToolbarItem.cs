using System.Windows.Input;

namespace SkeleKit;

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
	public string? Text
	{
		get;
		set
		{
			field = value;
			Changed?.Invoke();
		}
	}

	/// <summary>
	/// The local icon, or null for a text-only item.
	/// </summary>
	public ImageSource? Icon
	{
		get;
		set
		{
			field = value;
			Changed?.Invoke();
		}
	}

	/// <summary>
	/// Whether the item is in the bar at all.
	/// </summary>
	/// <remarks>
	/// Contextual actions toggle it live, like a Delete that only exists in edit mode.
	/// </remarks>
	public bool IsVisible
	{
		get;
		set
		{
			field = value;
			Changed?.Invoke();
		}
	} = true;

	internal event Action? Changed;

	/// <summary>
	/// Which side of the bar the item sits on.
	/// </summary>
	public ToolbarSide Side { get; set; } = ToolbarSide.Trailing;

	/// <summary>
	/// Whether the item is rendered as the prominent action.
	/// </summary>
	public bool IsPrimary { get; set; }

	/// <summary>
	/// The item's tint, or null to follow the page or app tint.
	/// </summary>
	public Color? Tint
	{
		get;
		set
		{
			field = value;
			Changed?.Invoke();
		}
	}

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
