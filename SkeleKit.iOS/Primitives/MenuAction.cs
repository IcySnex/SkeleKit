using System.Windows.Input;

namespace SkeleKit;

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
