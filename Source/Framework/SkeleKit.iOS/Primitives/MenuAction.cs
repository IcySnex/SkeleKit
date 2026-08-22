using System.Windows.Input;

namespace SkeleKit;

/// <summary>
/// An action shown in a button, toolbar or context menu.
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
	/// Command invoked when the action is chosen.
	/// </summary>
	public ICommand? Command { get; set; }

	/// <summary>
	/// The parameter passed to <see cref="Command"/>.
	/// </summary>
	/// <remarks>
	/// A collection item menu uses its current item while this is null.
	/// </remarks>
	public object? CommandParameter { get; set; }
}
