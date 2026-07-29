using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ActionsSelection;

[Page]
internal sealed class ButtonView : ShowcaseView<ButtonViewModel>
{
	public ButtonView(
		ButtonViewModel viewModel) : base(viewModel, "Button", Colors.Purple)
	{
		AddConfigurationShowcase(viewModel);
		AddContentShowcase(viewModel);
		AddStateShowcase(viewModel);
		AddMenuShowcase(viewModel);
	}


	void AddConfigurationShowcase(
		ButtonViewModel viewModel)
	{
		Button button = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Text = "Continue",
			Icon = "arrow.right",
			Kind = viewModel.SelectedStyle.Value,
			Size = viewModel.SelectedSize
		};

		Picker<ShowcaseOption<ButtonStyle>> style = new()
		{
			ItemsSource = viewModel.Styles,
			SelectedItem = viewModel.SelectedStyle,
			SelectionChanged = option =>
			{
				viewModel.SelectedStyle = option;
				button.Kind = option.Value;
			}
		};

		SegmentedControl size = new()
		{
			SelectedIndex = viewModel.SelectedSizeIndex,
			SelectionChanged = index =>
			{
				viewModel.SelectedSizeIndex = index;
				button.Size = viewModel.SelectedSize;
			}
		};
		size.Items.Add("Mini");
		size.Items.Add("Small");
		size.Items.Add("Medium");
		size.Items.Add("Large");

		AddShowcase(
			"Configuration",
			"Compare every native treatment and size class on the same button.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(button),
				SettingRow("Style", style),
				LabeledControl("Size", size)),
			CodeView(Bind(model => model.ConfigurationCode)));
	}

	void AddContentShowcase(
		ButtonViewModel viewModel)
	{
		Button button = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Text = "Save",
			Icon = "square.and.arrow.down",
			Subtitle = "Updated moments ago",
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Large,
			IconPlacement = viewModel.SelectedPlacement.Value,
			IconSize = viewModel.IconSize,
			IconSpacing = viewModel.IconSpacing,
			Padding = new(viewModel.HorizontalPadding, 12)
		};

		Picker<ShowcaseOption<IconPlacement>> placement = new()
		{
			ItemsSource = viewModel.Placements,
			SelectedItem = viewModel.SelectedPlacement,
			SelectionChanged = option =>
			{
				viewModel.SelectedPlacement = option;
				button.IconPlacement = option.Value;
			}
		};

		Switch subtitle = new()
		{
			IsOn = viewModel.ShowsSubtitle,
			Toggled = value =>
			{
				viewModel.ShowsSubtitle = value;
				button.Subtitle = value ? "Updated moments ago" : null;
			}
		};

		Slider iconSize = new()
		{
			Minimum = 10,
			Maximum = 28,
			Step = 1,
			Value = viewModel.IconSize,
			ValueChanged = value =>
			{
				viewModel.IconSize = value;
				button.IconSize = value;
			}
		};

		Slider iconSpacing = new()
		{
			Minimum = 0,
			Maximum = 20,
			Step = 1,
			Value = viewModel.IconSpacing,
			ValueChanged = value =>
			{
				viewModel.IconSpacing = value;
				button.IconSpacing = value;
			}
		};

		Slider padding = new()
		{
			Minimum = 8,
			Maximum = 32,
			Step = 1,
			Value = viewModel.HorizontalPadding,
			ValueChanged = value =>
			{
				viewModel.HorizontalPadding = value;
				button.Padding = new(value, 12);
			}
		};

		AddShowcase(
			"Content & layout",
			"Arrange the symbol and tune spacing without replacing the native configuration.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(button, 176),
				SettingRow("Icon placement", placement),
				SettingRow("Subtitle", subtitle),
				LabeledSlider("Icon size", Bind(model => model.IconSizeLabel), iconSize),
				LabeledSlider("Icon spacing", Bind(model => model.IconSpacingLabel), iconSpacing),
				LabeledSlider("Horizontal padding", Bind(model => model.PaddingLabel), padding)),
			CodeView(Bind(model => model.ContentCode)));
	}

	void AddStateShowcase(
		ButtonViewModel viewModel)
	{
		Button button = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = "Run command",
			Icon = "play.fill",
			Kind = ButtonStyle.Filled,
			Size = ButtonSize.Large,
			IsLoading = Bind(model => model.IsLoading),
			Command = viewModel.TapCommand,
			CommandParameter = "Button showcase"
		};

		Switch loading = new()
		{
			IsOn = viewModel.IsLoading,
			Toggled = value => viewModel.IsLoading = value
		};

		Switch destructive = new()
		{
			IsOn = viewModel.IsDestructive,
			Toggled = value =>
			{
				viewModel.IsDestructive = value;
				button.IsDestructive = value;
			}
		};

		Switch enabled = new()
		{
			IsOn = viewModel.IsButtonEnabled,
			Toggled = value => viewModel.IsButtonEnabled = value
		};

		AddShowcase(
			"States & commands",
			"Exercise loading, destructive and disabled states while observing command execution.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 10,

						Children =
						{
							button,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.StateStatus),
								TextStyle = TextStyle.Footnote,
								TextColor = Colors.SecondaryLabel,
								TextAlignment = TextAlignment.Center,
								MaxLines = 2
							}
						}
					},
					176),
				SettingRow("Loading", loading),
				SettingRow("Destructive", destructive),
				SettingRow("Enabled", enabled)),
			CodeView(Bind(model => model.StateCode)));
	}

	void AddMenuShowcase(
		ButtonViewModel viewModel)
	{
		Button actions = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = "Actions",
			Icon = "ellipsis.circle",
			Kind = ButtonStyle.Gray
		};
		actions.Menu.Add(new()
		{
			Text = "Share",
			Icon = "square.and.arrow.up",
			Command = viewModel.ShareCommand
		});
		actions.Menu.Add(new()
		{
			Text = "Favorite",
			Icon = "star",
			Command = viewModel.FavoriteCommand
		});
		actions.Menu.Add(new()
		{
			Text = "Delete",
			Icon = "trash",
			IsDestructive = true,
			Command = viewModel.DeleteCommand
		});

		Button density = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = "Density",
			Icon = "line.3.horizontal.decrease",
			Kind = ButtonStyle.Tinted,
			SelectsFromMenu = true
		};
		density.Menu.Add(new()
		{
			Text = "Compact",
			Command = viewModel.CompactCommand
		});
		density.Menu.Add(new()
		{
			Text = "Comfortable",
			Command = viewModel.ComfortableCommand
		});
		density.Menu.Add(new()
		{
			Text = "Spacious",
			Command = viewModel.SpaciousCommand
		});

		AddShowcase(
			"Menus",
			"Compare an action menu with a selection menu that adopts the chosen title.",
			ShowcaseBox.Canvas(
				new StackPanel
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Spacing = 14,

					Children =
					{
						new StackPanel
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							Orientation = Orientation.Horizontal,
							Spacing = 12,

							Children =
							{
								actions,
								density
							}
						},

						new Label
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							Text = Bind(model => model.MenuStatus),
							TextStyle = TextStyle.Footnote,
							TextColor = Colors.SecondaryLabel,
							TextAlignment = TextAlignment.Center
						}
					}
				},
				190),
			CodeView(Bind(model => model.MenuCode)));
	}


	static View PreviewWithSettings(
		View canvas,
		params View[] settings)
	{
		StackPanel configuration = new()
		{
			Padding = 16,
			Spacing = 14
		};

		foreach (View setting in settings)
			configuration.Children.Add(setting);

		return new StackPanel
		{
			Children =
			{
				canvas,
				new Divider(),
				configuration
			}
		};
	}

	static View SettingRow(
		string title,
		View control) =>
		new Grid
		{
			ColumnSpacing = 12,

			Columns =
			{
				GridLength.Star,
				GridLength.Auto
			},

			Children =
			{
				new Label
				{
					VerticalAlignment = VerticalAlignment.Center,
					Text = title,
					TextStyle = TextStyle.Body
				},

				control.Column(1)
			}
		};

	static View LabeledControl(
		string title,
		View control) =>
		new StackPanel
		{
			Spacing = 7,

			Children =
			{
				new Label
				{
					Text = title,
					TextStyle = TextStyle.Footnote,
					FontWeight = FontWeight.Medium,
					TextColor = Colors.SecondaryLabel
				},

				control
			}
		};

	static View LabeledSlider(
		string title,
		BindingExpression<string?> value,
		Slider slider) =>
		new StackPanel
		{
			Spacing = 7,

			Children =
			{
				new Grid
				{
					Columns =
					{
						GridLength.Star,
						GridLength.Auto
					},

					Children =
					{
						new Label
						{
							Text = title,
							TextStyle = TextStyle.Footnote,
							FontWeight = FontWeight.Medium,
							TextColor = Colors.SecondaryLabel
						},

						new Label
						{
							Text = value,
							TextStyle = TextStyle.Footnote,
							TextColor = Colors.SecondaryLabel
						}.Column(1)
					}
				},

				slider
			}
		};

	static View CodeView(
		BindingExpression<IReadOnlyList<Span>?> spans) =>
		new Border
		{
			Padding = 16,
			Background = Colors.TertiaryGroupedBackground,

			Child = new TextView
			{
				Spans = spans,
				IsSelectable = true,
				FontSize = 13,
				FontDesign = FontDesign.Monospaced,
				TextColor = Colors.Label,
				LineSpacing = 2
			}
		};
}
