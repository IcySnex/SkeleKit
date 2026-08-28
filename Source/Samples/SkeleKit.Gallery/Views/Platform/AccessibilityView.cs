using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class AccessibilityView : ShowcaseView<AccessibilityViewModel>
{
	public AccessibilityView(
		AccessibilityViewModel viewModel) : base(viewModel, "Accessibility", Colors.Teal)
	{
		AddTestingNotice();
		AddLabelHintShowcase(viewModel);
		AddValueShowcase(viewModel);
		AddTraitsShowcase(viewModel);
		AddGroupingShowcase(viewModel);
		AddFocusShowcase(viewModel);
	}


	void AddTestingNotice() =>
		Sections.Children.Add(new Border
		{
			Padding = 14,
			Background = Colors.Teal.WithAlpha(0.12),
			CornerRadius = 14,

			Child = new Grid
			{
				ColumnSpacing = 12,

				Columns =
				{
					GridLength.Auto,
					GridLength.Star
				},

				Children =
				{
					new Image
					{
						VerticalAlignment = VerticalAlignment.Center,
						Width = 24,
						Height = 24,
						Source = ImageSource.Symbol("accessibility"),
						SymbolSize = 20,
						Tint = Colors.Teal
					},

					new Label
					{
						VerticalAlignment = VerticalAlignment.Center,
						Text = "Enable VoiceOver or use Accessibility Inspector to verify the semantics in these specimens.",
						TextStyle = TextStyle.Footnote,
						TextColor = Colors.SecondaryLabel,
						MaxLines = 3
					}.Column(1)
				}
			}
		});

	void AddLabelHintShowcase(
		AccessibilityViewModel viewModel)
	{
		Label status = ResultLabel("Not activated");

		AddShowcase(
			"Label & hint",
			"Give an icon-only control a concise spoken name and additional activation context.",
			ShowcaseBox.Canvas(
				new StackPanel
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Spacing = 10,

					Children =
					{
						new Button
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							Width = 52,
							Height = 52,
				Icon = ImageSource.Symbol("info.circle"),
							IconSize = 22,
							Kind = ButtonStyle.Tinted,
							AccessibilityLabel = "More information",
							AccessibilityHint = "Shows additional information.",
							Command = Command.From(() => status.Text = "Activated")
						},

						status
					}
				},
				170),
			Code(model => model.LabelHintCode));
	}

	void AddValueShowcase(
		AccessibilityViewModel viewModel)
	{
		Slider slider = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Value = Bind(
				model => model.Value,
				static (model, value) => model.Value = value),
			Minimum = 0,
			Maximum = 100,
			AccessibilityLabel = "Value",
			AccessibilityValue = Bind(model => model.AccessibilityValueText)
		};

		AddShowcase(
			"Dynamic value",
			"Bind the spoken value so VoiceOver receives the same state shown visually.",
			ShowcaseBox.Canvas(
				new StackPanel
				{
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Center,
					MaxWidth = 300,
					Spacing = 10,

					Children =
					{
						new Label
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							Text = Bind(model => model.ValueLabel),
							TextStyle = TextStyle.Title2,
							FontWeight = FontWeight.Semibold,
							IsAccessibilityElement = false
						},

						slider
					}
				},
				180),
			Code(model => model.ValueCode));
	}

	void AddTraitsShowcase(
		AccessibilityViewModel viewModel)
	{
		AddShowcase(
			"Semantic traits",
			"Add header, image, and selected semantics while preserving each control's native traits.",
			ShowcaseBox.Canvas(
				new StackPanel
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					MinWidth = 230,
					Spacing = 16,

					Children =
					{
						new Label
						{
							Text = "Section heading",
							TextStyle = TextStyle.Headline,
							FontWeight = FontWeight.Semibold,
							AccessibilityTraits = AccessibilityTraits.Header
						},

						new StackPanel
						{
							Orientation = Orientation.Horizontal,
							Spacing = 10,

							Children =
							{
								new Image
								{
									Width = 40,
									Height = 40,
									Source = ImageSource.Symbol("photo"),
									SymbolSize = 28,
									AccessibilityLabel = "Sample image",
									AccessibilityTraits = AccessibilityTraits.Image,
									IsAccessibilityElement = true
								},

								new Label
								{
									VerticalAlignment = VerticalAlignment.Center,
									Text = "Sample image",
									TextColor = Colors.SecondaryLabel,
									IsAccessibilityElement = false
								}
							}
						},

						new Button
						{
							HorizontalAlignment = HorizontalAlignment.Start,
							Text = "Selected option",
							Kind = ButtonStyle.Tinted,
							AccessibilityTraits = AccessibilityTraits.Selected
						}
					}
				},
				230),
			Code(model => model.TraitsCode));
	}

	void AddGroupingShowcase(
		AccessibilityViewModel viewModel)
	{
		Border card = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 270,
			Padding = 16,
			Background = Colors.Teal.WithAlpha(0.14),
			CornerRadius = 16,
			AccessibilityLabel = "Sample item",
			AccessibilityValue = "Secondary text",
			AccessibilityIdentifier = "sample-item",
			IsAccessibilityElement = true,

			Child = new Grid
			{
				ColumnSpacing = 12,

				Columns =
				{
					GridLength.Auto,
					GridLength.Star
				},

				Children =
				{
					new Image
					{
						VerticalAlignment = VerticalAlignment.Center,
						Width = 42,
						Height = 42,
						Source = ImageSource.Symbol("square.stack.3d.up.fill"),
						SymbolSize = 30,
						Tint = Colors.Teal
					},

					new StackPanel
					{
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 2,

						Children =
						{
							new Label
							{
								Text = "Sample item",
								FontWeight = FontWeight.Semibold
							},

							new Label
							{
								Text = "Secondary text",
								TextStyle = TextStyle.Footnote,
								TextColor = Colors.SecondaryLabel
							}
						}
					}.Column(1)
				}
			}
		};

		AddShowcase(
			"Grouping & identifiers",
			"Expose a compound card as one VoiceOver element and give it a stable identifier for UI tests.",
			ShowcaseBox.Canvas(card, 180),
			Code(model => model.GroupingCode));
	}

	void AddFocusShowcase(
		AccessibilityViewModel viewModel)
	{
		Label status = ResultLabel("Not focused");
		TextField field = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Placeholder = "Value",
			AccessibilityLabel = "Value"
		};

		AddShowcase(
			"Input focus",
			"Move ordinary keyboard focus to and from a labeled text field.",
			ShowcaseBox.Canvas(
				new StackPanel
				{
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Center,
					MaxWidth = 280,
					Spacing = 12,

					Children =
					{
						field,

						new StackPanel
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							Orientation = Orientation.Horizontal,
							Spacing = 8,

							Children =
							{
								new Button
								{
									Text = "Focus",
									Kind = ButtonStyle.Tinted,
									Size = ButtonSize.Small,
									Command = Command.From(() =>
									{
										field.Focus();
										status.Text = "Focused";
									})
								},

								new Button
								{
									Text = "Unfocus",
									Kind = ButtonStyle.Gray,
									Size = ButtonSize.Small,
									Command = Command.From(() =>
									{
										field.Unfocus();
										status.Text = "Not focused";
									})
								}
							}
						},

						status
					}
				},
				210),
			Code(model => model.FocusCode));
	}


	static Label ResultLabel(
		string text) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = text,
			TextStyle = TextStyle.Footnote,
			TextColor = Colors.SecondaryLabel,
			TextAlignment = TextAlignment.Center
		};
}
