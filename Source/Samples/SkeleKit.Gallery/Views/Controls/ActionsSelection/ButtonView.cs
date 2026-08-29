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
			Icon = ImageSource.Symbol("arrow.right"),
			Kind = viewModel.SelectedStyle.Value,
			Size = viewModel.SelectedSize
		};

		Picker<ShowcaseOption<ButtonStyle>> style = new()
		{
			ItemsSource = viewModel.Styles,
			SelectedItem = Bind(vm => vm.SelectedStyle)
				.TwoWay((vm, val) => vm.SelectedStyle = val!),
			SelectionChanged = option => button.Kind = option.Value
		};

		SegmentedControl size = new()
		{
			SelectedIndex = Bind(vm => vm.SelectedSizeIndex)
				.TwoWay((vm, val) => vm.SelectedSizeIndex = val),
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
			Code(vm => vm.ConfigurationCode));
	}

	void AddContentShowcase(
		ButtonViewModel viewModel)
	{
		Button button = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Text = "Save",
			Icon = ImageSource.Symbol("square.and.arrow.down"),
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
			SelectedItem = Bind(vm => vm.SelectedPlacement)
				.TwoWay((vm, val) => vm.SelectedPlacement = val!),
			SelectionChanged = option => button.IconPlacement = option.Value
		};

		Switch subtitle = new()
		{
			IsOn = Bind(vm => vm.ShowsSubtitle)
				.TwoWay((vm, val) => vm.ShowsSubtitle = val),
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
			Value = Bind(vm => vm.IconSize)
				.TwoWay((vm, val) => vm.IconSize = val),
			ValueChanged = value => button.IconSize = value
		};

		Slider iconSpacing = new()
		{
			Minimum = 0,
			Maximum = 20,
			Step = 1,
			Value = Bind(vm => vm.IconSpacing)
				.TwoWay((vm, val) => vm.IconSpacing = val),
			ValueChanged = value => button.IconSpacing = value
		};

		Slider padding = new()
		{
			Minimum = 8,
			Maximum = 32,
			Step = 1,
			Value = Bind(vm => vm.HorizontalPadding)
				.TwoWay((vm, val) => vm.HorizontalPadding = val),
			ValueChanged = value => button.Padding = new(value, 12)
		};

		AddShowcase(
			"Content & layout",
			"Arrange the symbol and tune spacing without replacing the native configuration.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(button, 176),
				SettingRow("Icon placement", placement),
				SettingRow("Subtitle", subtitle),
				LabeledSlider("Icon size", Bind(vm => vm.IconSizeLabel), iconSize),
				LabeledSlider("Icon spacing", Bind(vm => vm.IconSpacingLabel), iconSpacing),
				LabeledSlider("Horizontal padding", Bind(vm => vm.PaddingLabel), padding)),
			Code(vm => vm.ContentCode));
	}

	void AddStateShowcase(
		ButtonViewModel viewModel)
	{
		Button button = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = "Run command",
			Icon = ImageSource.Symbol("play.fill"),
			Kind = ButtonStyle.Filled,
			Size = ButtonSize.Large,
			IsLoading = Bind(vm => vm.IsLoading),
			Command = viewModel.TapCommand
		};

		Switch loading = new()
		{
			IsOn = Bind(vm => vm.IsLoading)
				.TwoWay((vm, val) => vm.IsLoading = val)
		};

		Switch destructive = new()
		{
			IsOn = Bind(vm => vm.IsDestructive)
				.TwoWay((vm, val) => vm.IsDestructive = val),
			Toggled = value => button.IsDestructive = value
		};

		Switch enabled = new()
		{
			IsOn = Bind(vm => vm.IsButtonEnabled)
				.TwoWay((vm, val) => vm.IsButtonEnabled = val)
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
								Text = Bind(vm => vm.StateStatus),
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
			Code(vm => vm.StateCode));
	}

	void AddMenuShowcase(
		ButtonViewModel viewModel)
	{
		Button actions = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = "Actions",
			Icon = ImageSource.Symbol("ellipsis.circle"),
			Kind = ButtonStyle.Gray
		};
		actions.Menu.Add(new()
		{
			Text = "Share",
			Icon = ImageSource.Symbol("square.and.arrow.up"),
			Command = viewModel.SelectMenuCommand,
			CommandParameter = "Share"
		});
		actions.Menu.Add(new()
		{
			Text = "Favorite",
			Icon = ImageSource.Symbol("star"),
			Command = viewModel.SelectMenuCommand,
			CommandParameter = "Favorite"
		});
		actions.Menu.Add(new()
		{
			Text = "Delete",
			Icon = ImageSource.Symbol("trash"),
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
							Text = Bind(vm => vm.MenuStatus),
							TextStyle = TextStyle.Footnote,
							TextColor = Colors.SecondaryLabel,
							TextAlignment = TextAlignment.Center
						}
					}
				},
				190),
			Code(vm => vm.MenuCode));
	}
}
