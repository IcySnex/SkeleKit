using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.TextInput;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.TextInput;

[Page]
internal sealed class LabelView : ShowcaseView<LabelViewModel>
{
	public LabelView(
		LabelViewModel viewModel) : base(viewModel, "Label", Colors.Purple)
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
			TextColor = Colors.Purple,
			MaxLines = 2,
			TextAlignment = TextAlignment.Center
		};

		Picker<ShowcaseOption<TextStyle>> style = new()
		{
			MinWidth = 140,
			ItemsSource = viewModel.TextStyles,
			SelectedItem = Bind(
				model => model.SelectedTextStyle,
				static (model, value) => model.SelectedTextStyle = value!),
			SelectionChanged = option => label.TextStyle = option.Value
		};

		View styleSetting = SettingRow("Text style", style);

		Switch cap = new()
		{
			IsOn = Bind(
				model => model.CapsDynamicType,
				static (model, value) => model.CapsDynamicType = value),
			Toggled = value =>
			{
				label.MaxFontSize = value ? 24 : double.NaN;
			}
		};

		View capSetting = SettingRow("Cap at 24 pt", cap);

		Slider size = new()
		{
			Minimum = 12,
			Maximum = 40,
			Step = 1,
			Value = Bind(
				model => model.FontSize,
				static (model, value) => model.FontSize = value),
			ValueChanged = value =>
			{
				label.FontSize = value;
			}
		};

		View sizeSetting = LabeledSlider("Font size", Bind(model => model.FontSizeLabel), size);
		sizeSetting.IsVisible = false;

		SegmentedControl sizing = new()
		{
			SelectedIndex = Bind(
				model => model.UsesExplicitSize,
				static (model, value) => model.UsesExplicitSize = value,
				static value => value ? 1 : 0,
				static index => index is 1),
			SelectionChanged = index =>
			{
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
			SelectedItem = Bind(
				model => model.SelectedWeight,
				static (model, value) => model.SelectedWeight = value!),
			SelectionChanged = option => label.FontWeight = option.Value
		};

		SegmentedControl design = new()
		{
			SelectedIndex = Bind(
				model => model.SelectedDesignIndex,
				static (model, value) => model.SelectedDesignIndex = value),
			SelectionChanged = index =>
			{
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
			Code(model => model.TypographyCode));
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
			SelectedIndex = Bind(
				model => model.LineCountIndex,
				static (model, value) => model.LineCountIndex = value)
		};
		lines.Items.Add("1");
		lines.Items.Add("2");
		lines.Items.Add("Free");

		SegmentedControl alignment = new()
		{
			SelectedIndex = Bind(
				model => model.AlignmentIndex,
				static (model, value) => model.AlignmentIndex = value)
		};
		alignment.Items.Add("Leading");
		alignment.Items.Add("Center");
		alignment.Items.Add("Trailing");

		Picker<ShowcaseOption<Truncation>> truncation = new()
		{
			MinWidth = 120,
			ItemsSource = viewModel.Truncations,
			SelectedItem = Bind(
				model => model.SelectedTruncation,
				static (model, value) => model.SelectedTruncation = value!),
			SelectionChanged = option => label.Truncation = option.Value
		};

		Switch shrink = new()
		{
			IsOn = Bind(
				model => model.ShrinksToFit,
				static (model, value) => model.ShrinksToFit = value)
		};

		lines.SelectionChanged = index =>
			label.AutoShrink = viewModel.ShrinksToFit ? 0.65 : 0;

		shrink.Toggled = value => label.AutoShrink = value ? 0.65 : 0;

		AddShowcase(
			"Layout & fitting",
			"Combine line limits, truncation, alignment and single-line shrinking on one constrained label.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(label, 220),
				LabeledControl("Maximum lines", lines),
				LabeledControl("Text alignment", alignment),
				SettingRow("Truncation", truncation),
				SettingRow("Shrink to fit", shrink)),
			Code(model => model.FlowCode));
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
			Value = Bind(
				model => model.LineSpacing,
				static (model, value) => model.LineSpacing = value),
			ValueChanged = value =>
			{
				label.LineSpacing = value;
			}
		};

		Slider letterSpacing = new()
		{
			Minimum = -1,
			Maximum = 3,
			Step = 0.25,
			Value = Bind(
				model => model.LetterSpacing,
				static (model, value) => model.LetterSpacing = value),
			ValueChanged = value =>
			{
				label.LetterSpacing = value;
			}
		};

		Switch underline = new()
		{
			IsOn = Bind(
				model => model.UnderlinesAll,
				static (model, value) => model.UnderlinesAll = value),
			Toggled = value =>
			{
				label.Underline = value;
			}
		};

		Switch strike = new()
		{
			IsOn = Bind(
				model => model.StrikesAll,
				static (model, value) => model.StrikesAll = value),
			Toggled = value =>
			{
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
			Code(model => model.AttributedCode));
	}
}
