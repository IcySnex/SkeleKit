using UIKit;

namespace BareUI;

/// <summary>
/// A text label wrapping <c>UILabel</c>.
/// </summary>
public class Label : Control
{
	/// <summary>
	/// The text to display.
	/// </summary>
	public Bindable<string?> Text
	{
		get => text;
		set => textBinding = Register(textBinding, value, value => Set(ref text, value, ApplyText));
	}
	string? text;
	Binding<string?>? textBinding;

	/// <summary>
	/// Font size in points.
	/// </summary>
	public Bindable<double> FontSize
	{
		get => fontSize;
		set => fontSizeBinding = Register(fontSizeBinding, value, value => Set(ref fontSize, value, ApplyFont));
	}
	double fontSize = 17;
	Binding<double>? fontSizeBinding;

	/// <summary>
	/// Whether the text is rendered bold.
	/// </summary>
	public Bindable<bool> Bold
	{
		get => bold;
		set => boldBinding = Register(boldBinding, value, value => Set(ref bold, value, ApplyFont));
	}
	bool bold;
	Binding<bool>? boldBinding;

	/// <summary>
	/// Text color, or null for the system label color.
	/// </summary>
	public Bindable<Color?> TextColor
	{
		get => textColor;
		set => textColorBinding = Register(textColorBinding, value, value => Set(ref textColor, value, ApplyTextColor, affectsMeasure: false));
	}
	Color? textColor;
	Binding<Color?>? textColorBinding;

	/// <summary>
	/// Maximum number of lines, or 0 for unlimited (wraps freely).
	/// </summary>
	public Bindable<int> MaxLines
	{
		get => maxLines;
		set => maxLinesBinding = Register(maxLinesBinding, value, value => Set(ref maxLines, value, ApplyMaxLines));
	}
	int maxLines;
	Binding<int>? maxLinesBinding;

	/// <summary>
	/// Horizontal alignment of the text.
	/// </summary>
	public Bindable<TextAlignment> TextAlignment
	{
		get => textAlignment;
		set => textAlignmentBinding = Register(textAlignmentBinding, value, value => Set(ref textAlignment, value, ApplyTextAlignment, affectsMeasure: false));
	}
	TextAlignment textAlignment = BareUI.TextAlignment.Leading;
	Binding<TextAlignment>? textAlignmentBinding;


	private protected override UIView CreateNative() =>
		new UILabel { BackgroundColor = UIColor.Clear };

	private protected override void ApplyProperties()
	{
		ApplyText();
		ApplyFont();
		ApplyTextColor();
		ApplyMaxLines();
		ApplyTextAlignment();
	}

	UILabel Ui =>
		(UILabel)Native;

	void ApplyText() =>
		Ui.Text = text;

	void ApplyFont() =>
		Ui.Font = bold
			? UIFont.BoldSystemFontOfSize((nfloat)fontSize)
			: UIFont.SystemFontOfSize((nfloat)fontSize);

	void ApplyTextColor()
	{
		if (textColor is { } color)
			Ui.TextColor = color.ToUIColor();
	}

	void ApplyMaxLines() =>
		Ui.Lines = maxLines;

	void ApplyTextAlignment() =>
		Ui.TextAlignment = textAlignment switch
		{
			BareUI.TextAlignment.Center => UITextAlignment.Center,
			BareUI.TextAlignment.Trailing => UITextAlignment.Right,
			_ => UITextAlignment.Left
		};
}
