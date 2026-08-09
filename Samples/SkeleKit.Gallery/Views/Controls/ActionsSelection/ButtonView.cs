using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ActionsSelection;

[Page]
internal sealed class ButtonView : ShowcaseView<ButtonViewModel>
{
	public ButtonView(
		ButtonViewModel viewModel) : base(viewModel, "Button", Colors.Pink)
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
			SelectedItem = Bind(
				model => model.SelectedStyle,
				static (model, value) => model.SelectedStyle = value!),
			SelectionChanged = option => button.Kind = option.Value
		};

		SegmentedControl size = new()
		{
			SelectedIndex = Bind(
				model => model.SelectedSizeIndex,
				static (model, value) => model.SelectedSizeIndex = value),
			SelectionChanged = index =>
			{
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
			ShowcaseBox.Code(Bind(model => model.ConfigurationCode)));
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
			MinWidth = 150,
			ItemsSource = viewModel.Placements,
			SelectedItem = Bind(
				model => model.SelectedPlacement,
				static (model, value) => model.SelectedPlacement = value!),
			SelectionChanged = option => button.IconPlacement = option.Value
		};

		Switch subtitle = new()
		{
			IsOn = Bind(
				model => model.ShowsSubtitle,
				static (model, value) => model.ShowsSubtitle = value),
			Toggled = value =>
			{
				button.Subtitle = value ? "Updated moments ago" : null;
			}
		};

		Slider iconSize = new()
		{
			Minimum = 10,
			Maximum = 28,
			Step = 1,
			Value = Bind(
				model => model.IconSize,
				static (model, value) => model.IconSize = value),
			ValueChanged = value => button.IconSize = value
		};

		Slider iconSpacing = new()
		{
			Minimum = 0,
			Maximum = 20,
			Step = 1,
			Value = Bind(
				model => model.IconSpacing,
				static (model, value) => model.IconSpacing = value),
			ValueChanged = value => button.IconSpacing = value
		};

		Slider padding = new()
		{
			Minimum = 8,
			Maximum = 32,
			Step = 1,
			Value = Bind(
				model => model.HorizontalPadding,
				static (model, value) => model.HorizontalPadding = value),
			ValueChanged = value => button.Padding = new(value, 12)
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
			ShowcaseBox.Code(Bind(model => model.ContentCode)));
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
			Command = viewModel.TapCommand
		};

		Switch loading = new()
		{
			IsOn = Bind(
				model => model.IsLoading,
				static (model, value) => model.IsLoading = value)
		};

		Switch destructive = new()
		{
			IsOn = Bind(
				model => model.IsDestructive,
				static (model, value) => model.IsDestructive = value),
			Toggled = value => button.IsDestructive = value
		};

		Switch enabled = new()
		{
			IsOn = Bind(
				model => model.IsButtonEnabled,
				static (model, value) => model.IsButtonEnabled = value)
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
			ShowcaseBox.Code(Bind(model => model.StateCode)));
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
			Command = viewModel.SelectMenuCommand,
			CommandParameter = "Share"
		});
		actions.Menu.Add(new()
		{
			Text = "Favorite",
			Icon = "star",
			Command = viewModel.SelectMenuCommand,
			CommandParameter = "Favorite"
		});
		actions.Menu.Add(new()
		{
			Text = "Delete",
			Icon = "trash",
			IsDestructive = true,
			Command = viewModel.SelectMenuCommand,
			CommandParameter = "Delete"
		});

		Button density = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = "Density",
			Kind = ButtonStyle.Tinted,
			SelectsFromMenu = true
		};
		density.Menu.Add(new()
		{
			Text = "Compact",
			Command = viewModel.SelectMenuCommand,
			CommandParameter = "Compact"
		});
		density.Menu.Add(new()
		{
			Text = "Comfortable",
			Command = viewModel.SelectMenuCommand,
			CommandParameter = "Comfortable"
		});
		density.Menu.Add(new()
		{
			Text = "Spacious",
			Command = viewModel.SelectMenuCommand,
			CommandParameter = "Spacious"
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
			ShowcaseBox.Code(Bind(model => model.MenuCode)));
	}
}
