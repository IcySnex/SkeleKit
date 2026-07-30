using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.TextInput;

internal sealed partial class LabelViewModel : ShowcaseViewModel
{
	static readonly int[] LineCounts =
	[
		1,
		2,
		0
	];

	static readonly TextAlignment[] Alignments =
	[
		TextAlignment.Leading,
		TextAlignment.Center,
		TextAlignment.Trailing
	];


	public LabelViewModel()
	{
		SelectedTextStyle = TextStyles[6];
		SelectedWeight = FontWeights[5];
		SelectedDesign = FontDesigns[1];
		SelectedTruncation = Truncations[1];
	}


	public List<ShowcaseOption<TextStyle>> TextStyles { get; } =
	[
		new("Large Title", TextStyle.LargeTitle),
		new("Title 1", TextStyle.Title1),
		new("Title 2", TextStyle.Title2),
		new("Title 3", TextStyle.Title3),
		new("Headline", TextStyle.Headline),
		new("Subheadline", TextStyle.Subheadline),
		new("Body", TextStyle.Body),
		new("Callout", TextStyle.Callout),
		new("Footnote", TextStyle.Footnote),
		new("Caption 1", TextStyle.Caption1),
		new("Caption 2", TextStyle.Caption2)
	];

	public List<ShowcaseOption<FontWeight>> FontWeights { get; } =
	[
		new("Ultra Light", FontWeight.UltraLight),
		new("Thin", FontWeight.Thin),
		new("Light", FontWeight.Light),
		new("Regular", FontWeight.Regular),
		new("Medium", FontWeight.Medium),
		new("Semibold", FontWeight.Semibold),
		new("Bold", FontWeight.Bold),
		new("Heavy", FontWeight.Heavy),
		new("Black", FontWeight.Black)
	];

	public List<ShowcaseOption<FontDesign>> FontDesigns { get; } =
	[
		new("Default", FontDesign.Default),
		new("Rounded", FontDesign.Rounded),
		new("Serif", FontDesign.Serif),
		new("Mono", FontDesign.Monospaced)
	];

	public List<ShowcaseOption<Truncation>> Truncations { get; } =
	[
		new("None", Truncation.None),
		new("Tail", Truncation.Tail),
		new("Head", Truncation.Head),
		new("Middle", Truncation.Middle)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(DynamicTypeCode))]
	ShowcaseOption<TextStyle> selectedTextStyle = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(DynamicTypeCode))]
	bool capsDynamicType;

	public IReadOnlyList<Span> DynamicTypeCode =>
		Code(
			$$"""
			new Label
			{
				Text = "Typography that follows the reader",
				TextStyle = TextStyle.{{SelectedTextStyle.Value}},
				MaxFontSize = {{(CapsDynamicType ? "34" : "double.NaN")}},
				TextAlignment = TextAlignment.Center
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(FontSizeLabel))]
	[NotifyPropertyChangedFor(nameof(FontCode))]
	double fontSize = 26;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedWeightValue))]
	[NotifyPropertyChangedFor(nameof(FontCode))]
	ShowcaseOption<FontWeight> selectedWeight = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(FontCode))]
	ShowcaseOption<FontDesign> selectedDesign = null!;

	public string FontSizeLabel => $"{Number(FontSize)} pt";
	public FontWeight SelectedWeightValue => SelectedWeight.Value;

	public IReadOnlyList<Span> FontCode =>
		Code(
			$$"""
			new Label
			{
				Text = "Designed for emphasis",
				FontSize = Bind(model => model.FontSize),
				FontWeight = FontWeight.{{SelectedWeight.Value}},
				FontDesign = FontDesign.{{SelectedDesign.Value}},
				TextColor = Colors.Purple
			};

			new Label
			{
				Text = "Bold shorthand",
				Bold = true
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(WrappingLines))]
	[NotifyPropertyChangedFor(nameof(FlowCode))]
	int lineCountIndex = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedAlignment))]
	[NotifyPropertyChangedFor(nameof(FlowCode))]
	int alignmentIndex;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(FlowCode))]
	ShowcaseOption<Truncation> selectedTruncation = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(FlowCode))]
	bool shrinksToFit;

	public int WrappingLines =>
		LineCounts[Math.Clamp(LineCountIndex, 0, LineCounts.Length - 1)];

	public TextAlignment SelectedAlignment =>
		Alignments[Math.Clamp(AlignmentIndex, 0, Alignments.Length - 1)];

	public IReadOnlyList<Span> FlowCode =>
		Code(
			$$"""
			new Label
			{
				Text = "Native text wraps naturally inside a constrained layout.",
				MaxLines = {{WrappingLines}},
				Truncation = Truncation.None,
				TextAlignment = TextAlignment.{{SelectedAlignment}}
			};

			new Label
			{
				Text = "Quarterly-performance-overview.pdf",
				MaxLines = 1,
				Truncation = Truncation.{{SelectedTruncation.Value}},
				AutoShrink = {{(ShrinksToFit ? "0.65" : "0")}}
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(LineSpacingLabel))]
	[NotifyPropertyChangedFor(nameof(AttributedCode))]
	double lineSpacing = 6;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(LetterSpacingLabel))]
	[NotifyPropertyChangedFor(nameof(AttributedCode))]
	double letterSpacing = 0.5;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(AttributedCode))]
	bool underlinesAll;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(AttributedCode))]
	bool strikesAll;

	public IReadOnlyList<Span> AttributedSpans { get; } =
	[
		"Mix ",
		new("weight") { Bold = true },
		", ",
		new("color") { TextColor = Colors.Purple },
		" and ",
		new("design") { FontWeight = FontWeight.Light, FontDesign = FontDesign.Serif },
		".\n",
		new("Per-run styling") { FontSize = 22, Underline = true },
		" replaces ",
		new("uniform text") { Strikethrough = true }
	];

	public string LineSpacingLabel => $"{Number(LineSpacing)} pt";
	public string LetterSpacingLabel => $"{Number(LetterSpacing)} pt";

	public IReadOnlyList<Span> AttributedCode =>
		Code(
			$$"""
			new Label
			{
				Text = "Fallback text",
				Spans =
				[
					"Mix ",
					new("weight") { Bold = true },
					", ",
					new("color") { TextColor = Colors.Purple },
					" and ",
					new("design")
					{
						FontWeight = FontWeight.Light,
						FontDesign = FontDesign.Serif
					},
					".\n",
					new("Per-run styling")
					{
						FontSize = 22,
						Underline = true
					},
					" replaces ",
					new("uniform text") { Strikethrough = true }
				],
				LineSpacing = {{Number(LineSpacing)}},
				LetterSpacing = {{Number(LetterSpacing)}},
				Underline = {{Boolean(UnderlinesAll)}},
				Strikethrough = {{Boolean(StrikesAll)}}
			};
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Boolean(
		bool value) =>
		value ? "true" : "false";

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
