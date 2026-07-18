namespace BareUI;

/// <summary>
/// A text label.
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
	/// Styled runs composing the text, overriding <see cref="Text"/> when set. Each run styles itself
	/// over the label's own font and color, and a run with a <see cref="Span.Command"/> is tappable.
	/// </summary>
	public IReadOnlyList<Span>? Spans
	{
		get => spans;
		set => Set(ref spans, value, ApplyText);
	}
	IReadOnlyList<Span>? spans;

	/// <summary>
	/// The step of the native type hierarchy the text follows, or null to size it by <see cref="FontSize"/>.
	/// </summary>
	public TextStyle? TextStyle
	{
		get => textStyle;
		set => Set(ref textStyle, value, ApplyFont);
	}
	TextStyle? textStyle;

	/// <summary>
	/// Explicit font size in points, overriding <see cref="TextStyle"/>. NaN falls back to the text style, or 17 points without one.
	/// </summary>
	public Bindable<double> FontSize
	{
		get => fontSize;
		set => fontSizeBinding = Register(fontSizeBinding, value, value => Set(ref fontSize, value, ApplyFont));
	}
	double fontSize = double.NaN;
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
	Binding<FontWeight>? weightBinding;

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

	/// <summary>
	/// Extra points between lines.
	/// </summary>
	public double LineSpacing
	{
		get => lineSpacing;
		set => Set(ref lineSpacing, value, ApplyText);
	}
	double lineSpacing;

	/// <summary>
	/// Extra points between characters (negative tightens).
	/// </summary>
	public double LetterSpacing
	{
		get => letterSpacing;
		set => Set(ref letterSpacing, value, ApplyText);
	}
	double letterSpacing;

	/// <summary>
	/// Underlines the text.
	/// </summary>
	public bool Underline
	{
		get => underline;
		set => Set(ref underline, value, ApplyText, affectsMeasure: false);
	}
	bool underline;

	/// <summary>
	/// Strikes the text through.
	/// </summary>
	public bool Strikethrough
	{
		get => strikethrough;
		set => Set(ref strikethrough, value, ApplyText, affectsMeasure: false);
	}
	bool strikethrough;

	/// <summary>
	/// How far the text may shrink to fit its width, 0.5 meaning half size, or 0 to truncate instead.
	/// </summary>
	public double AutoShrink
	{
		get => autoShrink;
		set => Set(ref autoShrink, value, ApplyAutoShrink, affectsMeasure: false);
	}
	double autoShrink;

	/// <summary>
	/// The largest point size Dynamic Type may scale the text to, or NaN to follow the accessibility sizes all the way up.
	/// </summary>
	public double MaxFontSize
	{
		get => maxFontSize;
		set => Set(ref maxFontSize, value, ApplyFont);
	}
	double maxFontSize = double.NaN;


	private protected override UIView CreateNative() =>
		new UILabel
		{
			BackgroundColor = UIColor.Clear,
			AdjustsFontForContentSizeCategory = true
		};

	private protected override void ApplyProperties()
	{
		ApplyFont();
		ApplyTextColor();
		ApplyMaxLines();
		ApplyTextAlignment();
		ApplyTruncation();
		ApplyAutoShrink();
		ApplyText();
	}

	UILabel Ui =>
		(UILabel)Native;

	bool UsesAttributes =>
		lineSpacing is not 0 || letterSpacing is not 0 || underline || strikethrough;

	void ApplyText()
	{
		if (spans is { Count: > 0 })
		{
			ApplySpans();
			return;
		}

		if (!UsesAttributes || text is null)
		{
			Ui.Text = text;
			return;
		}

		UIStringAttributes attributes = new()
		{
			ParagraphStyle = BuildParagraph()
		};

		if (letterSpacing is not 0)
			attributes.KerningAdjustment = (float)letterSpacing;
		if (underline)
			attributes.UnderlineStyle = NSUnderlineStyle.Single;
		if (strikethrough)
			attributes.StrikethroughStyle = NSUnderlineStyle.Single;

		Ui.AttributedText = new NSAttributedString(text, attributes);
	}

	// the paragraph style mirrors the label's own wrap and alignment, or it would override them
	NSMutableParagraphStyle BuildParagraph() =>
		new()
		{
			LineSpacing = (nfloat)lineSpacing,
			LineBreakMode = Ui.LineBreakMode,
			Alignment = Ui.TextAlignment
		};

	void ApplyAutoShrink()
	{
		Ui.AdjustsFontSizeToFitWidth = autoShrink > 0;
		Ui.MinimumScaleFactor = (nfloat)autoShrink;
	}

	void ApplyFont()
	{
		Ui.Font = FontFor(weight, design, fontSize);

		if (spans is { Count: > 0 })
			ApplySpans();
	}

	// both paths scale with the user's text-size setting; a weight of Regular leaves a text style's own
	UIFont FontFor(
		FontWeight fontWeight,
		FontDesign fontDesign,
		double size) =>
		FontSpec.UsesTextStyle(textStyle, size)
			? Fonts.Preferred(textStyle!.Value, fontWeight, fontDesign, maxFontSize)
			: Fonts.Scaled(FontSpec.SizeOf(size), fontWeight, fontDesign, maxFontSize);

	void ApplyTruncation()
	{
		Ui.LineBreakMode = truncation switch
		{
			Truncation.Head => UILineBreakMode.HeadTruncation,
			Truncation.Middle => UILineBreakMode.MiddleTruncation,
			Truncation.None => UILineBreakMode.WordWrap,
			_ => UILineBreakMode.TailTruncation
		};

		if (UsesAttributes)
			ApplyText();
	}

	void ApplyTextColor()
	{
		if (textColor is { } color)
			Ui.TextColor = color.ToUIColor();
	}

	void ApplyMaxLines() =>
		Ui.Lines = maxLines;

	void ApplyTextAlignment()
	{
		Ui.TextAlignment = textAlignment switch
		{
			BareUI.TextAlignment.Center => UITextAlignment.Center,
			BareUI.TextAlignment.Trailing => UITextAlignment.Right,
			_ => UITextAlignment.Left
		};

		if (UsesAttributes || spans is { Count: > 0 })
			ApplyText();
	}


	void ApplySpans()
	{
		if (spans is not { Count: > 0 })
			return;

		NSMutableParagraphStyle paragraph = BuildParagraph();
		UIColor baseColor = textColor?.ToUIColor() ?? UIColor.Label;

		NSMutableAttributedString composed = new();

		foreach (Span span in spans)
		{
			UIStringAttributes attributes = new()
			{
				ParagraphStyle = paragraph,
				Font = FontFor(span),
				ForegroundColor = span.TextColor?.ToUIColor() ?? baseColor
			};

			if (letterSpacing is not 0)
				attributes.KerningAdjustment = (float)letterSpacing;
			if (underline || span.Underline)
				attributes.UnderlineStyle = NSUnderlineStyle.Single;
			if (strikethrough || span.Strikethrough)
				attributes.StrikethroughStyle = NSUnderlineStyle.Single;

			composed.Append(new NSAttributedString(span.Text, attributes));
		}

		Ui.AttributedText = composed;
	}

	// a span's own size decides the text-style-vs-explicit path; NaN falls through to the label's size
	UIFont FontFor(
		Span span) =>
		FontFor(
			span.Bold ? BareUI.FontWeight.Bold : span.FontWeight ?? weight,
			span.FontDesign ?? design,
			double.IsNaN(span.FontSize) ? fontSize : span.FontSize);
}
