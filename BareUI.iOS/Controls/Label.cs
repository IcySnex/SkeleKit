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
	/// Shorthand for a bold <see cref="FontWeight"/>.
	/// </summary>
	public Bindable<bool> Bold
	{
		get => weight is BareUI.FontWeight.Bold;
		set => boldBinding = Register(boldBinding, value, value => Set(ref weight, value ? BareUI.FontWeight.Bold : BareUI.FontWeight.Regular, ApplyFont));
	}
	Binding<bool>? boldBinding;

	/// <summary>
	/// The font's weight.
	/// </summary>
	public Bindable<FontWeight> FontWeight
	{
		get => weight;
		set => weightBinding = Register(weightBinding, value, value => Set(ref weight, value, ApplyFont));
	}
	FontWeight weight = BareUI.FontWeight.Regular;
	Binding<BareUI.FontWeight>? weightBinding;

	/// <summary>
	/// The font's design: system, rounded, serif or monospaced.
	/// </summary>
	public FontDesign FontDesign
	{
		get => design;
		set => Set(ref design, value, ApplyFont);
	}
	FontDesign design = FontDesign.Default;

	/// <summary>
	/// How the text is shortened when it does not fit.
	/// </summary>
	public Truncation Truncation
	{
		get => truncation;
		set => Set(ref truncation, value, ApplyTruncation);
	}
	Truncation truncation = Truncation.Tail;

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
		new UILabel
		{
			BackgroundColor = UIColor.Clear,
			AdjustsFontForContentSizeCategory = true
		};

	private protected override void ApplyProperties()
	{
		ApplyText();
		ApplyFont();
		ApplyTextColor();
		ApplyMaxLines();
		ApplyTextAlignment();
		ApplyTruncation();
	}

	UILabel Ui =>
		(UILabel)Native;

	void ApplyText() =>
		Ui.Text = text;

	// scaled by UIFontMetrics so the user's text-size setting is honoured
	void ApplyFont() =>
		Ui.Font = Fonts.Scaled(fontSize, weight, design);

	void ApplyTruncation() =>
		Ui.LineBreakMode = truncation switch
		{
			Truncation.Head => UILineBreakMode.HeadTruncation,
			Truncation.Middle => UILineBreakMode.MiddleTruncation,
			Truncation.None => UILineBreakMode.WordWrap,
			_ => UILineBreakMode.TailTruncation
		};

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
