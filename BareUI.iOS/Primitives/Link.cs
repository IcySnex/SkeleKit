using System.Windows.Input;

namespace BareUI;

/// <summary>
/// A tappable run of text inside a <see cref="TextView"/>'s <see cref="TextView.Spans"/>.
/// </summary>
/// <remarks>
/// It renders like a <see cref="Span"/> but fires <see cref="Command"/> when tapped and shows <see cref="ContextMenu"/> as a native hold-to-peek menu.<br/>
/// Inside a plain <see cref="Label"/> it is styled text only: the command and menu are ignored, since a <see cref="Label"/> is not interactive.
/// </remarks>
public sealed class Link : Span
{
	/// <summary>
	/// Creates a link.
	/// </summary>
	/// <param name="text">The run's text.</param>
	public Link(
		string text) : base(text)
	{ }


	/// <summary>
	/// The command run when the link is tapped.
	/// </summary>
	public ICommand? Command { get; set; }

	/// <summary>
	/// The parameter passed to <see cref="Command"/>.
	/// </summary>
	public object? CommandParameter { get; set; }

	/// <summary>
	/// Entries shown in the link's long-press peek menu, or empty for a plain tappable link.
	/// </summary>
	public IList<MenuAction> ContextMenu { get; } = [];
}
