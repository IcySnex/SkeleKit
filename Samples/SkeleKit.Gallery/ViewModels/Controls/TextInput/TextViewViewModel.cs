using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.TextInput;

internal sealed partial class TextViewViewModel : ShowcaseViewModel
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


	public TextViewViewModel()
	{
		SelectedTextStyle = TextStyles[6];
		SelectedWeight = FontWeights[3];
		SelectedDesign = FontDesigns[1];
		UpdateInteractiveSpans();
	}


	public ObservableCollection<Span> InteractiveSpans { get; } = [];

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


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectionSummary))]
	[NotifyPropertyChangedFor(nameof(SelectionCode))]
	int contentModeIndex = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectionSummary))]
	[NotifyPropertyChangedFor(nameof(SelectionCode))]
	bool isSelectable = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectionCode))]
	int linkColorIndex;

	[ObservableProperty]
	string interactionStatus = "Tap a link or hold it for its native menu.";

	public string SelectionSummary =>
		ContentModeIndex is 1
			? IsSelectable
				? "Links are active and the surrounding text is selectable."
				: "Links stay active because UIKit requires selectable text items."
			: IsSelectable
				? "Press and hold to select and copy the plain text."
				: "Selection is disabled for the plain text.";

	public IReadOnlyList<Span> SelectionCode =>
		Code(
			$$"""
			ObservableCollection<Span> spans =
			[
				"Read the ",
				new Link("documentation")
				{
					Command = viewModel.OpenLinkCommand,
					CommandParameter = "Documentation"
				}
			];

			TextView text = new()
			{
				Spans = spans,
				IsSelectable = {{Boolean(IsSelectable)}},
				LinkColor = {{(LinkColorIndex is 0 ? "null" : "Colors.Pink")}}
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TypographyCode))]
	ShowcaseOption<TextStyle> selectedTextStyle = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TypographyCode))]
	bool usesExplicitSize;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TypographyCode))]
	ShowcaseOption<FontWeight> selectedWeight = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TypographyCode))]
	ShowcaseOption<FontDesign> selectedDesign = null!;

	public List<Span> TypographySpans { get; } =
	[
		"Base typography flows through every run, while ",
		new("individual spans") { FontWeight = FontWeight.Bold, Underline = true },
		" can override it."
	];

	public IReadOnlyList<Span> TypographyCode =>
		Code(
			$$"""
			new TextView
			{
				Spans =
				[
					"Base typography flows through every run, while ",
					new("individual spans")
					{
						FontWeight = FontWeight.Bold,
						Underline = true
					},
					" can override it."
				],
				TextStyle = {{(UsesExplicitSize ? "null" : $"TextStyle.{SelectedTextStyle.Value}")}},
				FontSize = {{(UsesExplicitSize ? "24" : "double.NaN")}},
				FontWeight = FontWeight.{{SelectedWeight.Value}},
				FontDesign = FontDesign.{{SelectedDesign.Value}},
				TextColor = Colors.Pink
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedLineCount))]
	[NotifyPropertyChangedFor(nameof(ContainerCode))]
	int lineCountIndex = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedAlignment))]
	[NotifyPropertyChangedFor(nameof(ContainerCode))]
	int alignmentIndex;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(LineSpacingLabel))]
	[NotifyPropertyChangedFor(nameof(ContainerCode))]
	double lineSpacing = 5;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(LetterSpacingLabel))]
	[NotifyPropertyChangedFor(nameof(ContainerCode))]
	double letterSpacing;

	public int SelectedLineCount =>
		LineCounts[Math.Clamp(LineCountIndex, 0, LineCounts.Length - 1)];

	public TextAlignment SelectedAlignment =>
		Alignments[Math.Clamp(AlignmentIndex, 0, Alignments.Length - 1)];

	public string LineSpacingLabel => $"{Number(LineSpacing)} pt";
	public string LetterSpacingLabel => $"{Number(LetterSpacing)} pt";

	public List<Span> ContainerSpans { get; } =
	[
		"Text views use their native text container to wrap ",
		new("styled content") { TextColor = Colors.Pink, FontWeight = FontWeight.Semibold },
		" across a constrained width and truncate at the selected line limit."
	];

	public IReadOnlyList<Span> ContainerCode =>
		Code(
			$$"""
			new TextView
			{
				Width = 250,
				Spans =
				[
					"Text views use their native text container to wrap ",
					new("styled content")
					{
						TextColor = Colors.Pink,
						FontWeight = FontWeight.Semibold
					},
					" across a constrained width and truncate at the selected line limit."
				],
				MaxLines = {{SelectedLineCount}},
				TextAlignment = TextAlignment.{{SelectedAlignment}},
				LineSpacing = {{Number(LineSpacing)}},
				LetterSpacing = {{Number(LetterSpacing)}}
			};
			""");


	partial void OnContentModeIndexChanged(
		int value) =>
		UpdateInteractiveSpans();


	[RelayCommand]
	void OpenLink(
		string target) =>
		InteractionStatus = $"{target} link selected.";

	[RelayCommand]
	void RunMenuAction(
		string action) =>
		InteractionStatus = $"{action} selected from the context menu.";


	void UpdateInteractiveSpans()
	{
		InteractiveSpans.Clear();

		if (ContentModeIndex is 0)
		{
			InteractiveSpans.Add("This plain rich text can be selected and copied when selection is enabled.");
			return;
		}

		Link documentation = new("documentation")
		{
			Command = OpenLinkCommand,
			CommandParameter = "Documentation"
		};
		documentation.ContextMenu.Add(new()
		{
			Text = "Open",
			Icon = "arrow.up.forward",
			Command = RunMenuActionCommand,
			CommandParameter = "Open"
		});
		documentation.ContextMenu.Add(new()
		{
			Text = "Save",
			Icon = "bookmark",
			Command = RunMenuActionCommand,
			CommandParameter = "Save"
		});

		InteractiveSpans.Add("Read the ");
		InteractiveSpans.Add(documentation);
		InteractiveSpans.Add(" or inspect the ");
		InteractiveSpans.Add(new Link("source")
		{
			Command = OpenLinkCommand,
			CommandParameter = "Source"
		});
		InteractiveSpans.Add(". Hold documentation for more actions.");
	}


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
