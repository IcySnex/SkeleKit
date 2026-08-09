using System.Windows.Input;
using SkeleKit.Gallery.ViewModels.Framework.Foundations;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Foundations;

[Page]
internal sealed class ViewView : ShowcaseView<ViewViewModel>
{
	public ViewView(
		ViewViewModel viewModel) : base(viewModel, "View", Colors.Indigo)
	{
		AddLayoutShowcase(viewModel);
		AddVisualShowcase(viewModel);
		AddInteractionShowcase(viewModel);
	}


	void AddLayoutShowcase(
		ViewViewModel viewModel)
	{
		Border card = new()
		{
			Width = viewModel.LayoutWidth,
			Height = 96,
			MinWidth = 100,
			MaxWidth = 240,
			Margin = new(viewModel.LeadingMargin, 0, 0, 0),
			HorizontalAlignment = viewModel.LayoutAlignment,
			VerticalAlignment = VerticalAlignment.Center,
			IsVisible = Bind(model => model.LayoutVisible),
			Background = Colors.Indigo,
			CornerRadius = 18,

			Child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = "View",
				TextStyle = TextStyle.Title3,
				FontWeight = FontWeight.Bold,
				TextColor = Colors.White
			}
		};

		Slider width = new()
		{
			Minimum = 100,
			Maximum = 240,
			Step = 10,
			Value = Bind(
				model => model.LayoutWidth,
				static (model, value) => model.LayoutWidth = value),
			ValueChanged = value => card.Width = value
		};

		SegmentedControl alignment = new()
		{
			SelectedIndex = Bind(
				model => model.LayoutAlignmentIndex,
				static (model, value) => model.LayoutAlignmentIndex = value),
			SelectionChanged = index => card.HorizontalAlignment = viewModel.LayoutAlignment
		};
		alignment.Items.Add("Start");
		alignment.Items.Add("Center");
		alignment.Items.Add("End");

		Slider leadingMargin = new()
		{
			Minimum = 0,
			Maximum = 56,
			Step = 1,
			Value = Bind(
				model => model.LeadingMargin,
				static (model, value) => model.LeadingMargin = value),
			ValueChanged = value => card.Margin = new(value, 0, 0, 0)
		};

		Switch visible = new()
		{
			IsOn = Bind(
				model => model.LayoutVisible,
				static (model, value) => model.LayoutVisible = value)
		};

		AddShowcase(
			"Layout & visibility",
			"Change explicit constraints, margin and alignment, then inspect the measured and arranged result.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(card, 190),
				LabeledSlider("Width", Bind(model => model.LayoutWidthLabel), width),
				LabeledControl("Horizontal alignment", alignment),
				LabeledSlider("Leading margin", Bind(model => model.LeadingMarginLabel), leadingMargin),
				SettingRow("Visible", visible),
				SettingRow(
					"Layout state",
					ActionButton(
						"Inspect",
						"ruler",
						Command.From(() => _ = viewModel.InspectLayoutAsync(card))))),
			ShowcaseBox.Code(Bind(model => model.LayoutCode)));
	}

	void AddVisualShowcase(
		ViewViewModel viewModel)
	{
		Border card = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 180,
			Height = 96,
			Background = Colors.Indigo,
			Opacity = viewModel.Opacity,
			CornerRadius = 18,
			Rotation = viewModel.Rotation,
			Scale = viewModel.Scale,
			AnchorPoint = viewModel.Anchor,

			Child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = "Transform",
				TextStyle = TextStyle.Title3,
				FontWeight = FontWeight.Bold,
				TextColor = Colors.White
			}
		};

		Slider rotation = new()
		{
			Minimum = -30,
			Maximum = 30,
			Step = 1,
			Value = Bind(
				model => model.Rotation,
				static (model, value) => model.Rotation = value),
			ValueChanged = value => card.Rotation = value
		};

		Slider scale = new()
		{
			Minimum = 0.7,
			Maximum = 1.3,
			Step = 0.05,
			Value = Bind(
				model => model.Scale,
				static (model, value) => model.Scale = value),
			ValueChanged = value => card.Scale = value
		};

		Slider opacity = new()
		{
			Minimum = 0.25,
			Maximum = 1,
			Step = 0.05,
			Value = Bind(
				model => model.Opacity,
				static (model, value) => model.Opacity = value),
			ValueChanged = value => card.Opacity = value
		};

		SegmentedControl anchor = new()
		{
			SelectedIndex = Bind(
				model => model.AnchorIndex,
				static (model, value) => model.AnchorIndex = value),
			SelectionChanged = index => card.AnchorPoint = viewModel.Anchor
		};
		anchor.Items.Add("Center");
		anchor.Items.Add("Top leading");

		AddShowcase(
			"Visuals & transforms",
			"Adjust opacity and transforms without changing the view's layout slot.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(card, 240),
				LabeledSlider("Rotation", Bind(model => model.RotationLabel), rotation),
				LabeledSlider("Scale", Bind(model => model.ScaleLabel), scale),
				LabeledSlider("Opacity", Bind(model => model.OpacityLabel), opacity),
				LabeledControl("Transform anchor", anchor)),
			ShowcaseBox.Code(Bind(model => model.VisualCode)));
	}

	void AddInteractionShowcase(
		ViewViewModel viewModel)
	{
		Label status = Status("Tap, double-tap, hold, drag, pinch or rotate.");

		Border commandCard = InteractionCard("Tap, double-tap, hold");
		commandCard.TapCommand = Command.From(() => RecordInteraction(status, "Tap"));
		commandCard.DoubleTapCommand = Command.From(() => RecordInteraction(status, "Double tap"));
		commandCard.LongPressCommand = Command.From(() => RecordInteraction(status, "Long press"));
		commandCard.LongPressDuration = 0.7;

		Border gestureCard = InteractionCard("Drag, pinch, rotate");
		gestureCard.Panned = gesture => Pan(gestureCard, status, gesture);
		gestureCard.Pinched = gesture => Pinch(gestureCard, status, gesture);
		gestureCard.Rotated = gesture => Rotate(gestureCard, status, gesture);

		gestureCard.ContextMenu.Add(new()
		{
			Text = "Copy",
			Command = Command.From(() => RecordInteraction(status, "Copy"))
		});
		gestureCard.ContextMenu.Add(new()
		{
			Text = "Share",
			Command = Command.From(() => RecordInteraction(status, "Share"))
		});

		AddShowcase(
			"Gestures & commands",
			"Use one view for tap commands and another for continuous gestures and its context menu.",
			ShowcaseBox.Canvas(
				new StackPanel
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Spacing = 14,

					Children =
					{
						commandCard,
						gestureCard,
						status
					}
				},
				280),
			ShowcaseBox.Code(Bind(model => model.InteractionCode)));
	}

	static void Pan(
		View card,
		Label status,
		PanGesture gesture)
	{
		if (gesture.State is GestureState.Changed)
		{
			card.Translation = new(
				Math.Clamp(gesture.Translation.X, -70, 70),
				Math.Clamp(gesture.Translation.Y, -36, 36));
		}

		if (gesture.State is GestureState.Ended or GestureState.Canceled)
		{
			RecordInteraction(status, "Pan");
			ReturnHome(card);
		}
	}

	static void Pinch(
		View card,
		Label status,
		PinchGesture gesture)
	{
		if (gesture.State is GestureState.Changed)
			card.Scale = Math.Clamp(gesture.Scale, 0.7, 1.45);

		if (gesture.State is GestureState.Ended or GestureState.Canceled)
		{
			RecordInteraction(status, "Pinch");
			ReturnHome(card);
		}
	}

	static void Rotate(
		View card,
		Label status,
		RotateGesture gesture)
	{
		if (gesture.State is GestureState.Changed)
			card.Rotation = Math.Clamp(gesture.Degrees, -35, 35);

		if (gesture.State is GestureState.Ended or GestureState.Canceled)
		{
			RecordInteraction(status, "Rotation");
			ReturnHome(card);
		}
	}

	static void ReturnHome(
		View card) =>
		View.Animate(
			Animation.Spring(0.42, damping: 0.72),
			() =>
			{
				card.Translation = Point.Zero;
				card.Scale = 1;
				card.Rotation = 0;
				card.Opacity = 1;
			});

	static Border InteractionCard(
		string text) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Width = 220,
			Height = 72,
			Background = Colors.Indigo,
			CornerRadius = 18,
			PointerEffect = PointerEffect.Automatic,

			Child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = text,
				TextStyle = TextStyle.Headline,
				FontWeight = FontWeight.Semibold,
				TextColor = Colors.White,
				MaxLines = 2,
				TextAlignment = TextAlignment.Center
			}
		};

	static void RecordInteraction(
		Label status,
		string interaction) =>
		status.Text = interaction;

	static Button ActionButton(
		string text,
		string icon,
		ICommand command) =>
		new()
		{
			Text = text,
			Icon = icon,
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Small,
			Command = command
		};

	static Label Status(
		string text) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = text,
			TextStyle = TextStyle.Caption1,
			TextColor = Colors.SecondaryLabel,
			MaxLines = 2,
			TextAlignment = TextAlignment.Center
		};
}
