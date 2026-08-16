using System.Windows.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class HapticsView : ShowcaseView<HapticsViewModel>
{
	public HapticsView(
		HapticsViewModel viewModel) : base(viewModel, "Haptics", Colors.Teal)
	{
		AddHardwareNotice();
		AddImpactShowcase(viewModel);
		AddSelectionShowcase(viewModel);
		AddNotificationShowcase(viewModel);
		AddCustomPatternShowcase(viewModel);
	}


	void AddHardwareNotice() =>
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
						Source = ImageSource.Symbol("iphone.gen3.radiowaves.left.and.right"),
						SymbolSize = 20,
						Tint = Colors.Teal
					},

					new Label
					{
						VerticalAlignment = VerticalAlignment.Center,
						Text = "Haptic feedback is felt on supported physical devices; the simulator does not reproduce it.",
						TextStyle = TextStyle.Footnote,
						TextColor = Colors.SecondaryLabel,
						MaxLines = 3
					}.Column(1)
				}
			}
		});

	void AddImpactShowcase(
		HapticsViewModel viewModel)
	{
		Picker<ShowcaseOption<HapticStyle>> style = new()
		{
			MinWidth = 140,
			ItemsSource = viewModel.ImpactStyles,
			SelectedItem = Bind(
				model => model.SelectedImpactStyle,
				static (model, value) => model.SelectedImpactStyle = value!)
		};

		AddShowcase(
			"Impact",
			"Compare the five native impact weight profiles.",
			PreviewWithSettings(
				HapticCanvas(
					"Trigger impact",
					"circle.grid.cross",
					viewModel.TriggerImpactCommand,
					Bind(model => model.ImpactResult)),
				SettingRow("Style", style)),
			Code(model => model.ImpactCode));
	}

	void AddSelectionShowcase(
		HapticsViewModel viewModel)
	{
		AddShowcase(
			"Selection",
			"Trigger the subtle feedback used when a selection changes.",
			HapticCanvas(
				"Trigger selection",
				"checkmark.circle",
				viewModel.TriggerSelectionCommand,
				Bind(model => model.SelectionResult)),
			Code(model => model.SelectionCode));
	}

	void AddNotificationShowcase(
		HapticsViewModel viewModel)
	{
		Picker<ShowcaseOption<HapticsNotification>> notification = new()
		{
			MinWidth = 140,
			ItemsSource = viewModel.Notifications,
			SelectedItem = Bind(
				model => model.SelectedNotification,
				static (model, value) => model.SelectedNotification = value!)
		};

		AddShowcase(
			"Notification",
			"Compare the system success, warning, and error feedback patterns.",
			PreviewWithSettings(
				HapticCanvas(
					"Trigger notification",
					"bell.badge",
					viewModel.TriggerNotificationCommand,
					Bind(model => model.NotificationResult)),
				SettingRow("Type", notification)),
			Code(model => model.NotificationCode));
	}

	void AddCustomPatternShowcase(
		HapticsViewModel viewModel)
	{
		Slider intensity = new()
		{
			Value = Bind(
				model => model.Intensity,
				static (model, value) => model.Intensity = value),
			Minimum = 0,
			Maximum = 1,
			Step = 0.1
		};

		Slider sharpness = new()
		{
			Value = Bind(
				model => model.Sharpness,
				static (model, value) => model.Sharpness = value),
			Minimum = 0,
			Maximum = 1,
			Step = 0.1
		};

		AddShowcase(
			"Custom pattern",
			"Combine timed taps and a continuous event, then adjust their intensity and sharpness.",
			PreviewWithSettings(
				HapticCanvas(
					"Play pattern",
					"waveform.path",
					viewModel.PlayCustomPatternCommand,
					Bind(model => model.CustomPatternResult)),
				LabeledSlider("Intensity", Bind(model => model.IntensityLabel), intensity),
				LabeledSlider("Sharpness", Bind(model => model.SharpnessLabel), sharpness)),
			Code(model => model.CustomPatternCode));
	}


	static View HapticCanvas(
		string title,
		string icon,
		ICommand command,
		BindingExpression<string?> result) =>
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
						Text = title,
						Icon = icon,
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Medium,
						Command = command
					},

					new Label
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Text = result,
						TextStyle = TextStyle.Footnote,
						TextColor = Colors.SecondaryLabel,
						TextAlignment = TextAlignment.Center
					}
				}
			},
			170);
}
