#if IOS
using UIKit;
#endif

namespace BareUI;

/// <summary>
/// A text label wrapping <c>UILabel</c>.
/// </summary>
public class Label : Control
{
	/// <summary>
	/// The text to display.
	/// </summary>
	public string? Text { get; set; }

	/// <summary>
	/// Font size in points.
	/// </summary>
	public double FontSize { get; set; } = 17;

	/// <summary>
	/// Whether the text is rendered bold.
	/// </summary>
	public bool Bold { get; set; }

	/// <summary>
	/// Text color, or null for the system label color.
	/// </summary>
	public Color? TextColor { get; set; }

	/// <summary>
	/// Maximum number of lines, or 0 for unlimited (wraps freely).
	/// </summary>
	public int MaxLines { get; set; } = 0;

	/// <summary>
	/// Horizontal alignment of the text.
	/// </summary>
	public TextAlignment TextAlignment { get; set; } = TextAlignment.Leading;

#if IOS
	private protected override UIView CreateNative()
	{
		UILabel label = new()
		{
			Text = Text,
			BackgroundColor = UIColor.Clear,
			Font = Bold
				? UIFont.BoldSystemFontOfSize((nfloat)FontSize)
				: UIFont.SystemFontOfSize((nfloat)FontSize),
			Lines = MaxLines,
			TextAlignment = TextAlignment switch
			{
				TextAlignment.Center => UITextAlignment.Center,
				TextAlignment.Trailing => UITextAlignment.Right,
				_ => UITextAlignment.Left
			}
		};

		if (TextColor is { } color)
			label.TextColor = color.ToUIColor();

		return label;
	}
#endif
}
