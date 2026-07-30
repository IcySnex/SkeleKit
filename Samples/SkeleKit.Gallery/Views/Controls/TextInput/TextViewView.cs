using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.TextInput;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.TextInput;

[Page]
internal sealed class TextViewView : ShowcaseView<TextViewViewModel>
{
	public TextViewView(
		TextViewViewModel viewModel) : base(viewModel, "Text View", Colors.Pink)
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
			SelectedIndex = viewModel.ContentModeIndex,
			SelectionChanged = index => viewModel.ContentModeIndex = index
		};
		content.Items.Add("Plain");
		content.Items.Add("Links");

		Switch selectable = new()
		{
			IsOn = Bind(
				model => model.IsSelectable,
				static (model, value) => model.IsSelectable = value)
		};

		SegmentedControl linkColor = new()
		{
			SelectedIndex = viewModel.LinkColorIndex,
			SelectionChanged = index =>
			{
				viewModel.LinkColorIndex = index;
				text.LinkColor = index is 0 ? null : Colors.Pink;
			}
		};
		linkColor.Items.Add("Tint");
		linkColor.Items.Add("Pink");

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
								Text = Bind(model => model.SelectionSummary),
								TextStyle = TextStyle.Footnote,
								TextColor = Colors.SecondaryLabel,
								MaxLines = 2,
								TextAlignment = TextAlignment.Center
							},

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
					230),
				LabeledControl("Content", content),
				SettingRow("Selectable", selectable),
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
			TextColor = Colors.Pink,
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

				if (!viewModel.UsesExplicitSize)
					text.TextStyle = option.Value;
			}
		};

		SegmentedControl sizing = new()
		{
			SelectionChanged = index =>
			{
				viewModel.UsesExplicitSize = index is 1;
				text.TextStyle = viewModel.UsesExplicitSize ? null : viewModel.SelectedTextStyle.Value;
				text.FontSize = viewModel.UsesExplicitSize ? 24 : double.NaN;
			}
		};
		sizing.Items.Add("Dynamic");
		sizing.Items.Add("24 pt");

		Picker<ShowcaseOption<FontWeight>> weight = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.FontWeights,
			SelectedItem = viewModel.SelectedWeight,
			SelectionChanged = option =>
			{
				viewModel.SelectedWeight = option;
				text.FontWeight = option.Value;
			}
		};

		SegmentedControl design = new()
		{
			SelectedIndex = 1,
			SelectionChanged = index =>
			{
				viewModel.SelectedDesign = viewModel.FontDesigns[index];
				text.FontDesign = viewModel.SelectedDesign.Value;
			}
		};
		design.Items.Add("Default");
		design.Items.Add("Rounded");
		design.Items.Add("Serif");
		design.Items.Add("Mono");

		AddShowcase(
			"Typography",
			"Set the base Dynamic Type style, explicit size, weight, design and color inherited by every run.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(text, 220),
				SettingRow("Text style", style),
				LabeledControl("Sizing", sizing),
				SettingRow("Weight", weight),
				LabeledControl("Design", design)),
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
			SelectedIndex = viewModel.LineCountIndex,
			SelectionChanged = index =>
			{
				viewModel.LineCountIndex = index;
				text.MaxLines = viewModel.SelectedLineCount;
			}
		};
		lines.Items.Add("1");
		lines.Items.Add("2");
		lines.Items.Add("Free");

		SegmentedControl alignment = new()
		{
			SelectionChanged = index =>
			{
				viewModel.AlignmentIndex = index;
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
			Value = viewModel.LineSpacing,
			ValueChanged = value =>
			{
				viewModel.LineSpacing = value;
				text.LineSpacing = value;
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
