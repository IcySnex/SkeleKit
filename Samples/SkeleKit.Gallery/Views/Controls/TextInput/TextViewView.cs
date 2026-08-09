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
			Spans = Bind(model => model.InteractiveSpans),
			IsSelectable = Bind(model => model.IsSelectable),
			TextStyle = TextStyle.Body,
			TextAlignment = TextAlignment.Center
		};

		SegmentedControl content = new()
		{
			SelectedIndex = Bind(
				model => model.ContentModeIndex,
				static (model, value) => model.ContentModeIndex = value)
		};
		content.Items.Add("Plain");
		content.Items.Add("Links");

		Switch selectable = new()
		{
			IsOn = Bind(
				model => model.IsSelectable,
				static (model, value) => model.IsSelectable = value)
		};

		View selectableSetting = SettingRow("Selectable", selectable);
		selectableSetting.IsVisible = viewModel.ContentModeIndex is 0;

		content.SelectionChanged = index =>
		{
			selectableSetting.IsVisible = index is 0;
		};

		SegmentedControl linkColor = new()
		{
			SelectedIndex = Bind(
				model => model.LinkColorIndex,
				static (model, value) => model.LinkColorIndex = value),
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
								Text = Bind(model => model.InteractionStatus),
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
			ShowcaseBox.Code(Bind(model => model.SelectionCode)));
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
			SelectedItem = Bind(
				model => model.SelectedTextStyle,
				static (model, value) => model.SelectedTextStyle = value!),
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
			Value = Bind(
				model => model.FontSize,
				static (model, value) => model.FontSize = value),
			ValueChanged = value =>
			{
				text.FontSize = value;
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
			SelectedItem = Bind(
				model => model.SelectedWeight,
				static (model, value) => model.SelectedWeight = value!),
			SelectionChanged = option => text.FontWeight = option.Value
		};

		SegmentedControl design = new()
		{
			SelectedIndex = Bind(
				model => model.SelectedDesignIndex,
				static (model, value) => model.SelectedDesignIndex = value),
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
			SelectedIndex = Bind(
				model => model.TextColorIndex,
				static (model, value) => model.TextColorIndex = value),
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
			ShowcaseBox.Code(Bind(model => model.TypographyCode)));
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
			SelectedIndex = Bind(
				model => model.LineCountIndex,
				static (model, value) => model.LineCountIndex = value),
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
			SelectedIndex = Bind(
				model => model.AlignmentIndex,
				static (model, value) => model.AlignmentIndex = value),
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
			Value = Bind(
				model => model.LineSpacing,
				static (model, value) => model.LineSpacing = value),
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
			Value = Bind(
				model => model.LetterSpacing,
				static (model, value) => model.LetterSpacing = value),
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
				LabeledSlider("Line spacing", Bind(model => model.LineSpacingLabel), lineSpacing),
				LabeledSlider("Letter spacing", Bind(model => model.LetterSpacingLabel), letterSpacing)),
			ShowcaseBox.Code(Bind(model => model.ContainerCode)));
	}
}
