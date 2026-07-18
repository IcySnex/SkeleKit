namespace BareUI;

/// <summary>
/// A styled run of text inside a <see cref="Label"/>'s <see cref="Label.Spans"/>.
/// </summary>
/// <remarks>
/// Every unset visual property follows the label; a set one overrides it for this run alone.
/// </remarks>
public class Span
{
	/// <summary>
	/// Creates a span.
	/// </summary>
	/// <param name="text">The run's text.</param>
	public Span(
		string text) =>
		Text = text;


	/// <summary>
	/// Wraps a plain string as an unstyled span, so string literals sit beside styled runs in a list.
	/// </summary>
	/// <param name="text">The run's text.</param>
	public static implicit operator Span(
		string text) =>
		new(text);


	/// <summary>
	/// The run's text.
	/// </summary>
	public string Text { get; set; }

	/// <summary>
	/// Shorthand for a bold <see cref="FontWeight"/>.
	/// </summary>
	public bool Bold { get; set; }

	/// <summary>
	/// The run's font weight, or null to follow the label.
	/// </summary>
	public FontWeight? FontWeight { get; set; }

	/// <summary>
	/// The run's font design, or null to follow the label.
	/// </summary>
	public FontDesign? FontDesign { get; set; }

	/// <summary>
	/// The run's font size in points, or NaN to follow the label.
	/// </summary>
	public double FontSize { get; set; } = double.NaN;

	/// <summary>
	/// The run's text color, or null to follow the label.
	/// </summary>
	public Color? TextColor { get; set; }

	/// <summary>
	/// Underlines the run.
	/// </summary>
	public bool Underline { get; set; }

	/// <summary>
	/// Strikes the run through.
	/// </summary>
	public bool Strikethrough { get; set; }
}
