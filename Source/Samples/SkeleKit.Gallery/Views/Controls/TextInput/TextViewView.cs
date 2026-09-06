using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.TextInput;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.TextInput;

[Page]
internal sealed class TextViewView : ShowcaseView<TextViewViewModel>
{
	public TextViewView(
		TextViewViewModel viewModel) : base(viewModel, "Text View", Colors.Purple)
	{
		AddSelectionShowcase(viewModel);
		AddTypographyShowcase(viewModel);
		AddContainerShowcase(viewModel);
	}


	void AddSelectionShowcase(
		TextViewViewModel viewModel)
	{
		TextView text = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			MaxWidth = 290,
			Spans = Bind(vm => vm.InteractiveSpans),
			IsSelectable = Bind(vm => vm.IsSelectable),
			TextStyle = TextStyle.Body,
			TextAlignment = TextAlignment.Center
		};

		SegmentedControl content = new()
		{
			SelectedIndex = Bind(vm => vm.ContentModeIndex)
				.TwoWay((vm, val) => vm.ContentModeIndex = val)
		};
		content.Items.Add("Plain");
		content.Items.Add("Links");

		Switch selectable = new()
		{
			IsOn = Bind(vm => vm.IsSelectable)
				.TwoWay((vm, val) => vm.IsSelectable = val)
		};

		View selectableSetting = SettingRow("Selectable", selectable);
		selectableSetting.IsVisible = viewModel.ContentModeIndex is 0;

		content.SelectionChanged = index =>
		{
			selectableSetting.IsVisible = index is 0;
		};

		SegmentedControl linkColor = new()
		{
			SelectedIndex = Bind(vm => vm.LinkColorIndex)
				.TwoWay((vm, val) => vm.LinkColorIndex = val),
			SelectionChanged = index =>
			{
				text.LinkColor = index is 0 ? null : Colors.Blue;
			}
		};
		linkColor.Items.Add("Tint");
		linkColor.Items.Add("Blue");

		AddShowcase(
			"Selection & links",
			"Switch a live span collection between selectable text and tappable links with a native hold menu.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 300,
						Spacing = 12,

						Children =
						{
							text,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(vm => vm.InteractionStatus),
								TextStyle = TextStyle.Caption1,
								TextColor = Colors.SecondaryLabel,
								MaxLines = 2,
								TextAlignment = TextAlignment.Center
							}
						}
					},
					200),
				LabeledControl("Content", content),
				selectableSetting,
				LabeledControl("Link color", linkColor)),
			Code(vm => vm.SelectionCode));
	}

	void AddTypographyShowcase(
		TextViewViewModel viewModel)
	{
		TextView text = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			MaxWidth = 290,
			Spans = viewModel.TypographySpans,
			TextStyle = viewModel.SelectedTextStyle.Value,
			FontWeight = viewModel.SelectedWeight.Value,
			FontDesign = viewModel.SelectedDesign.Value,
			TextAlignment = TextAlignment.Center
		};

		Picker<ShowcaseOption<TextStyle>> style = new()
		{
			MinWidth = 140,
			ItemsSource = viewModel.TextStyles,
			SelectedItem = Bind(vm => vm.SelectedTextStyle)
				.TwoWay((vm, val) => vm.SelectedTextStyle = val!),
			SelectionChanged = option =>
			{
				if (!viewModel.UsesExplicitSize)
					text.TextStyle = option.Value;
			}
		};

		View styleSetting = SettingRow("Text style", style);

		Slider size = new()
		{
			Minimum = 12,
			Maximum = 40,
			Step = 1,
			Value = Bind(vm => vm.FontSize)
				.TwoWay((vm, val) => vm.FontSize = val),
			ValueChanged = value =>
			{
				text.FontSize = value;
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
				text.TextStyle = viewModel.UsesExplicitSize ? null : viewModel.SelectedTextStyle.Value;
				text.FontSize = viewModel.UsesExplicitSize ? viewModel.FontSize : double.NaN;
				styleSetting.IsVisible = !viewModel.UsesExplicitSize;
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
			SelectionChanged = option => text.FontWeight = option.Value
		};

		SegmentedControl design = new()
		{
			SelectedIndex = Bind(vm => vm.SelectedDesignIndex)
				.TwoWay((vm, val) => vm.SelectedDesignIndex = val),
			SelectionChanged = index =>
			{
				text.FontDesign = viewModel.SelectedDesign.Value;
			}
		};
		design.Items.Add("Default");
		design.Items.Add("Rounded");
		design.Items.Add("Serif");
		design.Items.Add("Mono");

		SegmentedControl color = new()
		{
			SelectedIndex = Bind(vm => vm.TextColorIndex)
				.TwoWay((vm, val) => vm.TextColorIndex = val),
			SelectionChanged = index =>
			{
				text.TextColor = index is 0 ? (Color?)null : Colors.Blue;
			}
		};
		color.Items.Add("System");
		color.Items.Add("Blue");

		AddShowcase(
			"Typography",
			"Choose either Dynamic Type or an explicit size, then set the base weight, design and color inherited by every run.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(text, 220),
				LabeledControl("Sizing", sizing),
				styleSetting,
				sizeSetting,
				SettingRow("Weight", weight),
				LabeledControl("Design", design),
				LabeledControl("Text color", color)),
			Code(vm => vm.TypographyCode));
	}

	void AddContainerShowcase(
		TextViewViewModel viewModel)
	{
		TextView text = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 250,
			Spans = viewModel.ContainerSpans,
			TextStyle = TextStyle.Body,
			MaxLines = viewModel.SelectedLineCount,
			TextAlignment = viewModel.SelectedAlignment,
			LineSpacing = viewModel.LineSpacing,
			LetterSpacing = viewModel.LetterSpacing
		};

		SegmentedControl lines = new()
		{
			SelectedIndex = Bind(vm => vm.LineCountIndex)
				.TwoWay((vm, val) => vm.LineCountIndex = val),
			SelectionChanged = index =>
			{
				text.MaxLines = viewModel.SelectedLineCount;
			}
		};
		lines.Items.Add("1");
		lines.Items.Add("2");
		lines.Items.Add("Free");

		SegmentedControl alignment = new()
		{
			SelectedIndex = Bind(vm => vm.AlignmentIndex)
				.TwoWay((vm, val) => vm.AlignmentIndex = val),
			SelectionChanged = index =>
			{
				text.TextAlignment = viewModel.SelectedAlignment;
			}
		};
		alignment.Items.Add("Leading");
		alignment.Items.Add("Center");
		alignment.Items.Add("Trailing");

		Slider lineSpacing = new()
		{
			Minimum = 0,
			Maximum = 12,
			Step = 1,
			Value = Bind(vm => vm.LineSpacing)
				.TwoWay((vm, val) => vm.LineSpacing = val),
			ValueChanged = value =>
			{
				text.LineSpacing = value;
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
				text.LetterSpacing = value;
			}
		};

		AddShowcase(
			"Text container",
			"Constrain rich text with native wrapping, line limits, alignment and typographic spacing.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(text, 230),
				LabeledControl("Maximum lines", lines),
				LabeledControl("Text alignment", alignment),
				LabeledSlider("Line spacing", Bind(vm => vm.LineSpacingLabel), lineSpacing),
				LabeledSlider("Letter spacing", Bind(vm => vm.LetterSpacingLabel), letterSpacing)),
			Code(vm => vm.ContainerCode));
	}
}
