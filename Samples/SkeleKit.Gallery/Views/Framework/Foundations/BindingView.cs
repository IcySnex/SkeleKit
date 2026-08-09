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
		AddListShowcase(viewModel);
	}


	void AddOneWayShowcase(
		BindingViewModel viewModel)
	{
		Label target = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = Bind(
				model => model.OneWayValue,
				value => $"{value:0}"),
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
				LabeledSlider("Source value", Bind(model => model.OneWayValueLabel), source)),
			Code(model => model.OneWayCode));
	}

	void AddTwoWayShowcase(
		BindingViewModel viewModel)
	{
		TextField field = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Text = Bind(
				model => model.TwoWayText,
				static (model, value) => model.TwoWayText = value),
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
											Text = Bind(model => model.TwoWayText),
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
			Code(model => model.TwoWayCode));
	}

	void AddListShowcase(
		BindingViewModel viewModel)
	{
		Button add = new()
		{
			Text = "Add",
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Small,
			IsEnabled = Bind(model => model.CanAddItem),
			Command = Command.From(viewModel.AddItem)
		};

		Button remove = new()
		{
			Text = "Remove",
			Kind = ButtonStyle.Gray,
			Size = ButtonSize.Small,
			IsEnabled = Bind(model => model.CanRemoveItem),
			Command = Command.From(viewModel.RemoveItem)
		};

		Picker<BindingSampleItem> picker = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			MinWidth = 220,
			ItemsSource = Bind(model => model.Items),
			SelectedItem = Bind(
				model => model.SelectedItem,
				static (model, item) => model.SelectedItem = item),
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
								Text = Bind(
									model => model.SelectedItemLabel,
									value => $"Selected: {value}"),
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
								Text = Bind(model => model.ItemCountLabel),
								TextStyle = TextStyle.Subheadline,
								TextColor = Colors.SecondaryLabel
							},
							remove,
							add
						}
					})),
			Code(model => model.ListCode));
	}
}
