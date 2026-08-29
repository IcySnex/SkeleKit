using SkeleKit.Gallery.ViewModels.Framework.Foundations;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Foundations;

[Page]
internal sealed class BindingView : ShowcaseView<BindingViewModel>
{
	public BindingView(
		BindingViewModel viewModel) : base(viewModel, "Binding", Colors.Indigo)
	{
		AddOneWayShowcase(viewModel);
		AddTwoWayShowcase(viewModel);
		AddPathShowcase(viewModel);
		AddListShowcase(viewModel);
	}


	void AddOneWayShowcase(
		BindingViewModel viewModel)
	{
		Label target = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = Bind(vm => vm.OneWayValue)
				.ConvertTo(value => $"{value:0}"),
			TextStyle = TextStyle.LargeTitle,
			FontWeight = FontWeight.Bold,
			TextColor = Colors.Indigo
		};

		Slider source = new()
		{
			Minimum = 12,
			Maximum = 48,
			Step = 1,
			Value = viewModel.OneWayValue,
			ValueChanged = value => viewModel.OneWayValue = value
		};

		AddShowcase(
			"One-way value",
			"Change the source value and watch the formatted label update automatically.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 6,

						Children =
						{
							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = "Bound label",
								TextStyle = TextStyle.Caption1,
								FontWeight = FontWeight.Semibold,
								TextColor = Colors.SecondaryLabel
							},
							target
						}
					},
					170),
				LabeledSlider("Source value", Bind(vm => vm.OneWayValueLabel), source)),
			Code(vm => vm.OneWayCode));
	}

	void AddTwoWayShowcase(
		BindingViewModel viewModel)
	{
		TextField field = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Text = Bind(vm => vm.TwoWayText)
				.TwoWay((vm, val) => vm.TwoWayText = val),
			Placeholder = "Type a value",
			ClearButton = ClearButton.WhileEditing
		};

		Button updateSource = new()
		{
			Text = "Set example",
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Small,
			Command = Command.From(viewModel.SetTwoWayExample)
		};

		AddShowcase(
			"Two-way value",
			"Type to update the ViewModel, or change the ViewModel and watch the field update.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 300,
						Spacing = 12,

						Children =
						{
							field,

							new Border
							{
								Padding = 14,
								Background = Colors.Indigo.WithAlpha(0.18),
								CornerRadius = 12,

								Child = new StackPanel
								{
									Spacing = 3,

									Children =
									{
										new Label
										{
											Text = "ViewModel value",
											TextStyle = TextStyle.Caption1,
											FontWeight = FontWeight.Semibold,
											TextColor = Colors.SecondaryLabel
										},

										new Label
										{
											Text = Bind(vm => vm.TwoWayText),
											TextStyle = TextStyle.Headline,
											FontWeight = FontWeight.Semibold,
											MaxLines = 2
										}
									}
								}
							}
						}
					},
					220),
				SettingRow("Update source", updateSource)),
			Code(vm => vm.TwoWayCode));
	}

	void AddListShowcase(
		BindingViewModel viewModel)
	{
		Button add = new()
		{
			Text = "Add",
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Small,
			IsEnabled = Bind(vm => vm.CanAddItem),
			Command = Command.From(viewModel.AddItem)
		};

		Button remove = new()
		{
			Text = "Remove",
			Kind = ButtonStyle.Gray,
			Size = ButtonSize.Small,
			IsEnabled = Bind(vm => vm.CanRemoveItem),
			Command = Command.From(viewModel.RemoveItem)
		};

		Picker<BindingSampleItem> picker = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			MinWidth = 220,
			ItemsSource = Bind(vm => vm.Items),
			SelectedItem = Bind(vm => vm.SelectedItem)
				.TwoWay((vm, val) => vm.SelectedItem = val),
			ItemTitle = item => item.Title
		};

		AddShowcase(
			"Live list binding",
			"Add or remove source items, then open the picker to inspect its live options.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 10,

						Children =
						{
							picker,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(vm => vm.SelectedItemLabel)
									.ConvertTo(value => $"Selected: {value}"),
								TextStyle = TextStyle.Subheadline,
								TextColor = Colors.SecondaryLabel
							}
						}
					},
					180),
				SettingRow(
					"Items",
					new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Spacing = 8,

						Children =
						{
							new Label
							{
								VerticalAlignment = VerticalAlignment.Center,
								Text = Bind(vm => vm.ItemCountLabel),
								TextStyle = TextStyle.Subheadline,
								TextColor = Colors.SecondaryLabel
							},
							remove,
							add
						}
					})),
			Code(vm => vm.ListCode));
	}

	void AddPathShowcase(
		BindingViewModel viewModel)
	{
		Label name = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = Bind(vm => vm.Profile)
				.Path(profile => profile.DisplayName),
			TextStyle = TextStyle.Title2,
			FontWeight = FontWeight.Semibold,
			TextColor = Colors.Indigo
		};

		Button rename = new()
		{
			Text = "Change name",
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Small,
			Command = Command.From(viewModel.RenameProfile)
		};

		Button replace = new()
		{
			Text = "Replace profile",
			Kind = ButtonStyle.Gray,
			Size = ButtonSize.Small,
			Command = Command.From(viewModel.ReplaceProfile)
		};

		AddShowcase(
			"Nested path",
			"Observe a property on a nested object, including changes after that object is replaced.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(name, 150),
				SettingRow(
					"Source",
					new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Spacing = 8,
						Children = { rename, replace }
					})),
			Code(vm => vm.PathCode));
	}
}
