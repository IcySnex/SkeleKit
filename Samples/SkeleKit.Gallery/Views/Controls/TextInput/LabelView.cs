using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.TextInput;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.TextInput;

[Page]
internal sealed class LabelView : ShowcaseView<LabelViewModel>
{
	public LabelView(
		LabelViewModel viewModel) : base(viewModel, "Label", Colors.Pink)
	{
		AddDynamicTypeShowcase(viewModel);
		AddFontShowcase(viewModel);
		AddFlowShowcase(viewModel);
		AddAttributedShowcase(viewModel);
	}


	void AddDynamicTypeShowcase(
		LabelViewModel viewModel)
	{
		Label label = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			VerticalAlignment = VerticalAlignment.Center,
			Text = "Typography that follows the reader",
			TextStyle = viewModel.SelectedTextStyle.Value,
			MaxFontSize = double.NaN,
			MaxLines = 2,
			TextAlignment = TextAlignment.Center
		};

		Picker<ShowcaseOption<TextStyle>> style = new()
		{
			MinWidth = 140,
			ItemsSource = viewModel.TextStyles,
			SelectedItem = viewModel.SelectedTextStyle,
			SelectionChanged = option =>
			{
				viewModel.SelectedTextStyle = option;
				label.TextStyle = option.Value;
			}
		};

		Switch cap = new()
		{
			IsOn = viewModel.CapsDynamicType,
			Toggled = value =>
			{
				viewModel.CapsDynamicType = value;
				label.MaxFontSize = value ? 24 : double.NaN;
			}
		};

		AddShowcase(
			"Dynamic Type",
			"Follow every native text-style curve and optionally cap its largest accessibility size.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(label, 190),
				SettingRow("Text style", style),
				SettingRow("Cap at 24 pt", cap)),
			ShowcaseBox.Code(Bind(model => model.DynamicTypeCode)));
	}

	void AddFontShowcase(
		LabelViewModel viewModel)
	{
		Label label = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = "Designed for emphasis",
			FontSize = Bind(model => model.FontSize),
			FontWeight = Bind(model => model.SelectedWeightValue),
			FontDesign = viewModel.SelectedDesign.Value,
			TextColor = Colors.Pink,
			TextAlignment = TextAlignment.Center
		};

		Picker<ShowcaseOption<FontWeight>> weight = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.FontWeights,
			SelectedItem = viewModel.SelectedWeight,
			SelectionChanged = option => viewModel.SelectedWeight = option
		};

		SegmentedControl design = new()
		{
			SelectedIndex = 1,
			SelectionChanged = index =>
			{
				viewModel.SelectedDesign = viewModel.FontDesigns[index];
				label.FontDesign = viewModel.SelectedDesign.Value;
			}
		};
		design.Items.Add("Default");
		design.Items.Add("Rounded");
		design.Items.Add("Serif");
		design.Items.Add("Mono");

		Slider size = new()
		{
			Minimum = 12,
			Maximum = 40,
			Step = 1,
			Value = viewModel.FontSize,
			ValueChanged = value => viewModel.FontSize = value
		};

		AddShowcase(
			"Font configuration",
			"Compose explicit size, every native weight, system font designs and semantic color.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 12,

						Children =
						{
							label,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = "Bold shorthand",
								Bold = true,
								TextStyle = TextStyle.Subheadline
							}
						}
					},
					190),
				SettingRow("Weight", weight),
				LabeledControl("Design", design),
				LabeledSlider("Font size", Bind(model => model.FontSizeLabel), size)),
			ShowcaseBox.Code(Bind(model => model.FontCode)));
	}

	void AddFlowShowcase(
		LabelViewModel viewModel)
	{
		Label label = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 250,
			Text = "Quarterly performance overview for international product teams and regional partners.",
			TextStyle = TextStyle.Body,
			MaxLines = Bind(model => model.WrappingLines),
			Truncation = viewModel.SelectedTruncation.Value,
			TextAlignment = Bind(model => model.SelectedAlignment),
			AutoShrink = 0
		};

		SegmentedControl lines = new()
		{
			SelectedIndex = 1
		};
		lines.Items.Add("1");
		lines.Items.Add("2");
		lines.Items.Add("Free");

		SegmentedControl alignment = new()
		{
			SelectionChanged = index => viewModel.AlignmentIndex = index
		};
		alignment.Items.Add("Leading");
		alignment.Items.Add("Center");
		alignment.Items.Add("Trailing");

		Picker<ShowcaseOption<Truncation>> truncation = new()
		{
			MinWidth = 120,
			ItemsSource = viewModel.Truncations,
			SelectedItem = viewModel.SelectedTruncation,
			SelectionChanged = option =>
			{
				viewModel.SelectedTruncation = option;
				label.Truncation = option.Value;
			}
		};

		Switch shrink = new()
		{
			IsOn = viewModel.ShrinksToFit
		};

		lines.SelectionChanged = index =>
		{
			viewModel.LineCountIndex = index;

			if (index is not 0 && viewModel.ShrinksToFit)
			{
				viewModel.ShrinksToFit = false;
				shrink.IsOn = false;
				label.AutoShrink = 0;
			}
		};

		shrink.Toggled = value =>
		{
			viewModel.ShrinksToFit = value;

			if (value && viewModel.LineCountIndex is not 0)
			{
				viewModel.LineCountIndex = 0;
				lines.SelectedIndex = 0;
			}

			label.AutoShrink = value ? 0.65 : 0;
		};

		AddShowcase(
			"Layout & fitting",
			"Combine line limits, truncation, alignment and single-line shrinking on one constrained label.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(label, 220),
				LabeledControl("Maximum lines", lines),
				LabeledControl("Text alignment", alignment),
				SettingRow("Truncation", truncation),
				SettingRow("Shrink to fit", shrink)),
			ShowcaseBox.Code(Bind(model => model.FlowCode)));
	}

	void AddAttributedShowcase(
		LabelViewModel viewModel)
	{
		Label label = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			VerticalAlignment = VerticalAlignment.Center,
			Text = "Fallback text",
			Spans = viewModel.AttributedSpans,
			TextStyle = TextStyle.Body,
			LineSpacing = viewModel.LineSpacing,
			LetterSpacing = viewModel.LetterSpacing,
			Underline = viewModel.UnderlinesAll,
			Strikethrough = viewModel.StrikesAll,
			MaxLines = 0,
			TextAlignment = TextAlignment.Center
		};

		Slider lineSpacing = new()
		{
			Minimum = 0,
			Maximum = 12,
			Step = 1,
			Value = viewModel.LineSpacing,
			ValueChanged = value =>
			{
				viewModel.LineSpacing = value;
				label.LineSpacing = value;
			}
		};

		Slider letterSpacing = new()
		{
			Minimum = -1,
			Maximum = 3,
			Step = 0.25,
			Value = viewModel.LetterSpacing,
			ValueChanged = value =>
			{
				viewModel.LetterSpacing = value;
				label.LetterSpacing = value;
			}
		};

		Switch underline = new()
		{
			IsOn = viewModel.UnderlinesAll,
			Toggled = value =>
			{
				viewModel.UnderlinesAll = value;
				label.Underline = value;
			}
		};

		Switch strike = new()
		{
			IsOn = viewModel.StrikesAll,
			Toggled = value =>
			{
				viewModel.StrikesAll = value;
				label.Strikethrough = value;
			}
		};

		AddShowcase(
			"Attributed text",
			"Mix styled spans, spacing and whole-label decoration while preserving inherited typography.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(label, 240),
				LabeledSlider("Line spacing", Bind(model => model.LineSpacingLabel), lineSpacing),
				LabeledSlider("Letter spacing", Bind(model => model.LetterSpacingLabel), letterSpacing),
				SettingRow("Underline all", underline),
				SettingRow("Strike all", strike)),
			ShowcaseBox.Code(Bind(model => model.AttributedCode)));
	}
}
