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
			SelectedItem = Bind(vm => vm.SelectedTextStyle)
				.TwoWay((vm, val) => vm.SelectedTextStyle = val!),
			SelectionChanged = option => label.TextStyle = option.Value
		};

		View styleSetting = SettingRow("Text style", style);

		Switch cap = new()
		{
			IsOn = Bind(vm => vm.CapsDynamicType)
				.TwoWay((vm, val) => vm.CapsDynamicType = val),
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
			Value = Bind(vm => vm.FontSize)
				.TwoWay((vm, val) => vm.FontSize = val),
			ValueChanged = value =>
			{
				label.FontSize = value;
			}
		};

		View sizeSetting = LabeledSlider("Font size", Bind(vm => vm.FontSizeLabel), size);
		sizeSetting.IsVisible = false;

		SegmentedControl sizing = new()
		{
			SelectedIndex = Bind(vm => vm.UsesExplicitSize)
				.ConvertTo(val => val ? 1 : 0)
				.ConvertFrom(val => val is 1)
				.TwoWay((vm, val) => vm.UsesExplicitSize = val),
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
			SelectedItem = Bind(vm => vm.SelectedWeight)
				.TwoWay((vm, val) => vm.SelectedWeight = val!),
			SelectionChanged = option => label.FontWeight = option.Value
		};

		SegmentedControl design = new()
		{
			SelectedIndex = Bind(vm => vm.SelectedDesignIndex)
				.TwoWay((vm, val) => vm.SelectedDesignIndex = val),
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
			Code(vm => vm.TypographyCode));
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
			MaxLines = Bind(vm => vm.WrappingLines),
			Truncation = viewModel.SelectedTruncation.Value,
			TextAlignment = Bind(vm => vm.SelectedAlignment),
			AutoShrink = 0
		};

		SegmentedControl lines = new()
		{
			SelectedIndex = Bind(vm => vm.LineCountIndex)
				.TwoWay((vm, val) => vm.LineCountIndex = val)
		};
		lines.Items.Add("1");
		lines.Items.Add("2");
		lines.Items.Add("Free");

		SegmentedControl alignment = new()
		{
			SelectedIndex = Bind(vm => vm.AlignmentIndex)
				.TwoWay((vm, val) => vm.AlignmentIndex = val)
		};
		alignment.Items.Add("Leading");
		alignment.Items.Add("Center");
		alignment.Items.Add("Trailing");

		Picker<ShowcaseOption<Truncation>> truncation = new()
		{
			MinWidth = 120,
			ItemsSource = viewModel.Truncations,
			SelectedItem = Bind(vm => vm.SelectedTruncation)
				.TwoWay((vm, val) => vm.SelectedTruncation = val!),
			SelectionChanged = option => label.Truncation = option.Value
		};

		Switch shrink = new()
		{
			IsOn = Bind(vm => vm.ShrinksToFit)
				.TwoWay((vm, val) => vm.ShrinksToFit = val)
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
			Code(vm => vm.FlowCode));
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
			Value = Bind(vm => vm.LineSpacing)
				.TwoWay((vm, val) => vm.LineSpacing = val),
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
			Value = Bind(vm => vm.LetterSpacing)
				.TwoWay((vm, val) => vm.LetterSpacing = val),
			ValueChanged = value =>
			{
				label.LetterSpacing = value;
			}
		};

		Switch underline = new()
		{
			IsOn = Bind(vm => vm.UnderlinesAll)
				.TwoWay((vm, val) => vm.UnderlinesAll = val),
			Toggled = value =>
			{
				label.Underline = value;
			}
		};

		Switch strike = new()
		{
			IsOn = Bind(vm => vm.StrikesAll)
				.TwoWay((vm, val) => vm.StrikesAll = val),
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
				LabeledSlider("Line spacing", Bind(vm => vm.LineSpacingLabel), lineSpacing),
				LabeledSlider("Letter spacing", Bind(vm => vm.LetterSpacingLabel), letterSpacing),
				SettingRow("Underline all", underline),
				SettingRow("Strike all", strike)),
			Code(vm => vm.AttributedCode));
	}
}
