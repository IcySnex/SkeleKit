using System.Collections.ObjectModel;
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
/// An entry in a page's navigation bar or bottom toolbar.
/// </summary>
/// <remarks>
/// Use <see cref="ToolbarButton"/>, <see cref="ToolbarMenu"/>, or <see cref="ToolbarGroup"/>.
/// </remarks>
public abstract class ToolbarItem
{
	internal ToolbarItem()
	{
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
			OnChanged();
		}
	} = true;

	internal event Action<ToolbarItem>? Changed;

	private protected void OnChanged() =>
		Changed?.Invoke(this);

	/// <summary>
	/// Which side of the bar the item sits on.
	/// </summary>
	/// <remarks>
	/// Only used for top-level entries. The value is ignored for actions inside a
	/// <see cref="ToolbarGroup"/>.
	/// </remarks>
	public ToolbarSide Side { get; set; } = ToolbarSide.Trailing;
}

/// <summary>
/// A toolbar item that performs an action or opens a menu.
/// </summary>
public abstract class ToolbarAction : ToolbarItem
{
	internal ToolbarAction()
	{
	}

	/// <summary>
	/// The item's text, or null when it shows only an icon.
	/// </summary>
	public string? Text
	{
		get;
		set
		{
			field = value;
			OnChanged();
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
			OnChanged();
		}
	}

	/// <summary>
	/// Whether the item is rendered as the prominent action.
	/// </summary>
	/// <remarks>
	/// A prominent item does not share its Liquid Glass background with other items.
	/// </remarks>
	public bool IsPrimary
	{
		get;
		set
		{
			field = value;
			OnChanged();
		}
	}

	/// <summary>
	/// How long the item remains visible when the bar has limited space.
	/// </summary>
	/// <remarks>
	/// iOS 27 and later. Earlier versions use the system's normal ordering.
	/// </remarks>
	public ToolbarVisibilityPriority VisibilityPriority
	{
		get;
		set
		{
			field = value;
			OnChanged();
		}
	} = ToolbarVisibilityPriority.Standard;

	/// <summary>
	/// The item's tint, or null to follow the page or app tint.
	/// </summary>
	public Color? Tint
	{
		get;
		set
		{
			field = value;
			OnChanged();
		}
	}
}

/// <summary>
/// A button in the page's navigation bar or bottom toolbar.
/// </summary>
public sealed class ToolbarButton : ToolbarAction
{
	/// <summary>
	/// Invoked when the item is tapped; its CanExecute drives the enabled state.
	/// </summary>
	public ICommand? Command { get; set; }

	/// <summary>
	/// The parameter passed to <see cref="Command"/>.
	/// </summary>
	public object? CommandParameter { get; set; }
}

/// <summary>
/// A button that opens a menu in the page's navigation bar or bottom toolbar.
/// </summary>
public sealed class ToolbarMenu : ToolbarAction
{
	readonly ObservableCollection<MenuAction> actions = [];

	public ToolbarMenu()
	{
		actions.CollectionChanged += (_, _) => OnChanged();
	}

	/// <summary>
	/// Menu entries shown on tap.
	/// </summary>
	public IList<MenuAction> Actions => actions;
}

/// <summary>
/// A set of toolbar actions rendered as one visual group.
/// </summary>
/// <remarks>
/// Items are laid out contiguously. On iOS 26 and later, adjacent top-level entries are
/// separated so each group keeps its own Liquid Glass background.
/// </remarks>
public sealed class ToolbarGroup : ToolbarItem
{
	readonly ObservableCollection<ToolbarAction> items = [];

	public ToolbarGroup()
	{
		items.CollectionChanged += (_, _) => OnChanged();
	}

	public ToolbarGroup(
		params ToolbarAction[] items) : this()
	{
		foreach (ToolbarAction item in items)
			Items.Add(item);
	}

	/// <summary>
	/// The actions in the group. Groups cannot contain other groups.
	/// </summary>
	public IList<ToolbarAction> Items => items;
}
