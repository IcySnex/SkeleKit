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
		AddTypographyShowcase(viewModel);
		AddFlowShowcase(viewModel);
		AddAttributedShowcase(viewModel);
	}


	void AddTypographyShowcase(
		LabelViewModel viewModel)
	{
		Label label = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			VerticalAlignment = VerticalAlignment.Center,
			Text = "Typography that follows the reader",
			TextStyle = viewModel.SelectedTextStyle.Value,
			MaxFontSize = double.NaN,
			FontWeight = viewModel.SelectedWeight.Value,
			FontDesign = viewModel.SelectedDesign.Value,
			TextColor = Colors.Pink,
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

		View styleSetting = SettingRow("Text style", style);

		Switch cap = new()
		{
			IsOn = viewModel.CapsDynamicType,
			Toggled = value =>
			{
				viewModel.CapsDynamicType = value;
				label.MaxFontSize = value ? 24 : double.NaN;
			}
		};

		View capSetting = SettingRow("Cap at 24 pt", cap);

		Slider size = new()
		{
			Minimum = 12,
			Maximum = 40,
			Step = 1,
			Value = viewModel.FontSize,
			ValueChanged = value =>
			{
				viewModel.FontSize = value;
				label.FontSize = value;
			}
		};

		View sizeSetting = LabeledSlider("Font size", Bind(model => model.FontSizeLabel), size);
		sizeSetting.IsVisible = false;

		SegmentedControl sizing = new()
		{
			SelectionChanged = index =>
			{
				viewModel.UsesExplicitSize = index is 1;
				label.TextStyle = viewModel.UsesExplicitSize ? null : viewModel.SelectedTextStyle.Value;
				label.FontSize = viewModel.UsesExplicitSize ? viewModel.FontSize : double.NaN;
				label.MaxFontSize = !viewModel.UsesExplicitSize && viewModel.CapsDynamicType
					? 24
					: double.NaN;
				styleSetting.IsVisible = !viewModel.UsesExplicitSize;
				capSetting.IsVisible = !viewModel.UsesExplicitSize;
				sizeSetting.IsVisible = viewModel.UsesExplicitSize;
			}
		};
		sizing.Items.Add("Dynamic");
		sizing.Items.Add("Fixed");

		Picker<ShowcaseOption<FontWeight>> weight = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.FontWeights,
			SelectedItem = viewModel.SelectedWeight,
			SelectionChanged = option =>
			{
				viewModel.SelectedWeight = option;
				label.FontWeight = option.Value;
			}
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

		AddShowcase(
			"Typography",
			"Choose Dynamic Type or a fixed size, then configure the native weight and system font design.",
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
					210),
				LabeledControl("Sizing", sizing),
				styleSetting,
				capSetting,
				sizeSetting,
				SettingRow("Weight", weight),
				LabeledControl("Design", design)),
			ShowcaseBox.Code(Bind(model => model.TypographyCode)));
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
