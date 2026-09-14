namespace SkeleKit;

/// <summary>
/// A styled run of text inside a <see cref="Label"/>'s or <see cref="TextView"/>'s spans.
/// </summary>
/// <remarks>
/// Every unset visual property follows the containing control; a set one overrides it for this run alone.
/// </remarks>
public class Span
{
	/// <summary>
	/// Creates a span.
	/// </summary>
	/// <param name="text">The run's text.</param>
	public Span(
		string text)
	{
		Text = text;
	}


	/// <summary>
	/// Wraps a plain string as an unstyled span, so string literals sit beside styled runs in a list.
	/// </summary>
	/// <param name="text">The run's text.</param>
	/// <returns>An unstyled span containing the text.</returns>
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
	/// The run's font weight, or null to follow the containing control.
	/// </summary>
	public FontWeight? FontWeight { get; set; }

	/// <summary>
	/// The run's font design, or null to follow the containing control.
	/// </summary>
	public FontDesign? FontDesign { get; set; }

	/// <summary>
	/// The run's Dynamic Type text style, or null to follow the containing control.
	/// </summary>
	/// <remarks>
	/// An explicit <see cref="FontSize"/> takes precedence over this style.
	/// </remarks>
	public TextStyle? TextStyle { get; set; }

	/// <summary>
	/// The run's base font size in points, scaled by Dynamic Type, or NaN to follow its text style or the containing control.
	/// </summary>
	public double FontSize { get; set; } = double.NaN;

	/// <summary>
	/// The smallest point size Dynamic Type may produce, or NaN to follow the containing control.
	/// </summary>
	public double MinFontSize { get; set; } = double.NaN;

	/// <summary>
	/// The largest point size Dynamic Type may produce, or NaN to follow the containing control.
	/// </summary>
	/// <remarks>
	/// Set this and <see cref="MinFontSize"/> to the same value for a fixed-size run.
	/// </remarks>
	public double MaxFontSize { get; set; } = double.NaN;

	/// <summary>
	/// The run's text color, or null to follow the containing control.
	/// </summary>
	public Color? TextColor { get; set; }

	/// <summary>
	/// The alignment of the run's paragraph, or null to follow the containing control.
	/// </summary>
	/// <remarks>
	/// Paragraph alignment applies to the complete paragraph. Use this override on a span that covers the whole paragraph, including its terminating newline when one follows.
	/// </remarks>
	public TextAlignment? TextAlignment { get; set; }

	/// <summary>
	/// Underlines the run.
	/// </summary>
	public bool Underline { get; set; }

	/// <summary>
	/// Strikes the run through.
	/// </summary>
	public bool Strikethrough { get; set; }
}
