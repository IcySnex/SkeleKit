using SkeleKit.Gallery.ViewModels.Framework.Foundations;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Foundations;

[Page]
internal sealed class PanelsView : ShowcaseView<PanelsViewModel>
{
	public PanelsView(
		PanelsViewModel viewModel) : base(viewModel, "Panels", Colors.Indigo)
	{
		AddChildrenShowcase(viewModel);
		AddPaddingShowcase(viewModel);
		AddBindingShowcase(viewModel);
	}


	void AddChildrenShowcase(
		PanelsViewModel viewModel)
	{
		StackPanel row = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Orientation = Orientation.Horizontal,
			Spacing = 8
		};
		SetChildCount(row, (int)viewModel.ChildCount);

		Stepper count = new()
		{
			Value = Bind(
				model => model.ChildCount,
				static (model, value) => model.ChildCount = value),
			Minimum = 1,
			Maximum = 5,
			Step = 1,
			ValueChanged = value => SetChildCount(row, (int)Math.Round(value))
		};

		AddShowcase(
			"Child collection",
			"Add or remove child views and watch the panel update its layout immediately.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(row, 170),
				SettingRow(
					"Children",
					new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Spacing = 10,

						Children =
						{
							new Label
							{
								VerticalAlignment = VerticalAlignment.Center,
								Text = Bind(model => model.ChildCountLabel),
								TextStyle = TextStyle.Subheadline,
								TextColor = Colors.SecondaryLabel
							},
							count
						}
					})),
			Code(model => model.ChildrenCode));
	}

	void AddPaddingShowcase(
		PanelsViewModel viewModel)
	{
		Overlay panel = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 240,
			Height = 124,
			Padding = new(viewModel.PanelPadding),
			Background = Colors.Indigo.WithAlpha(0.18),
			CornerRadius = 18,

			Children =
			{
				new Border
				{
					Background = Colors.Indigo,
					CornerRadius = 12,

					Child = new Label
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = "Content",
						TextStyle = TextStyle.Headline,
						FontWeight = FontWeight.Semibold,
						TextColor = Colors.White
					}
				}
			}
		};

		Slider padding = new()
		{
			Minimum = 0,
			Maximum = 32,
			Step = 1,
			Value = Bind(
				model => model.PanelPadding,
				static (model, value) => model.PanelPadding = value),
			ValueChanged = value => panel.Padding = new(value)
		};

		AddShowcase(
			"Panel padding",
			"Adjust the space reserved between the panel's bounds and its child.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(panel, 190),
				LabeledSlider("Padding", Bind(model => model.PaddingLabel), padding)),
			Code(model => model.PaddingCode));
	}

	void AddBindingShowcase(
		PanelsViewModel viewModel)
	{
		StackPanel panel = new()
		{
			BindingContext = viewModel,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 300,
			Padding = 16,
			Spacing = 10,
			Background = Colors.Indigo.WithAlpha(0.18),
			CornerRadius = 18,

			Children =
			{
				new Label
				{
					Text = "Panel BindingContext",
					TextStyle = TextStyle.Caption1,
					FontWeight = FontWeight.Semibold,
					TextColor = Colors.SecondaryLabel
				},

				new Border
				{
					Padding = 14,
					Background = Colors.SecondaryBackground,
					CornerRadius = 12,

					Child = new Label
					{
						Text = Bind(
							model => model.InheritedText,
							text => $"Child reads: {text}"),
						TextStyle = TextStyle.Headline,
						FontWeight = FontWeight.Semibold,
						MaxLines = 2
					}
				}
			}
		};

		TextField value = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Text = Bind(
				model => model.InheritedText,
				static (model, text) => model.InheritedText = text),
			Placeholder = "Type a value",
			ClearButton = ClearButton.WhileEditing
		};

		AddShowcase(
			"Binding inheritance",
			"Edit the panel's context value and watch its nested child resolve the same binding.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(panel, 200),
				LabeledControl("Context value", value)),
			Code(model => model.BindingCode));
	}


	static void SetChildCount(
		StackPanel panel,
		int count)
	{
		count = Math.Clamp(count, 1, 5);

		while (panel.Children.Count < count)
			panel.Children.Add(ChildCard(panel.Children.Count + 1));

		while (panel.Children.Count > count)
		{
			View last = panel.Children[panel.Children.Count - 1];
			panel.Children.Remove(last);
		}
	}

	static Border ChildCard(
		int number) =>
		new()
		{
			Width = 44,
			Height = 56,
			Background = Colors.Indigo,
			CornerRadius = 12,

			Child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = number.ToString(),
				TextStyle = TextStyle.Headline,
				FontWeight = FontWeight.Semibold,
				TextColor = Colors.White
			}
		};
}
