using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ValuesStatus;

[Page]
internal sealed class SwitchView : ShowcaseView<SwitchViewModel>
{
	public SwitchView(
		SwitchViewModel viewModel) : base(viewModel, "Switch", Colors.Red)
	{
		AddBindingShowcase(viewModel);
		AddConfigurationShowcase(viewModel);
	}


	void AddBindingShowcase(
		SwitchViewModel viewModel)
	{
		Switch toggle = new()
		{
			VerticalAlignment = VerticalAlignment.Center,
			IsOn = Bind(
				model => model.IsOn,
				static (model, value) => model.IsOn = value),
			Toggled = viewModel.RecordToggle
		};

		AddShowcase(
			"Binding & callback",
			"Toggle native state, observe the two-way ViewModel value, and compare user changes with programmatic updates.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 320,
						Spacing = 12,

						Children =
						{
							SettingsRow(
								"Notifications",
								"Receive updates about new gallery examples.",
								toggle),
							Status(Bind(model => model.StateSummary), FontWeight.Medium),
							Status(Bind(model => model.ToggleStatus))
						}
					},
					210),
				SettingRow(
					"Bound value",
					new Button
					{
						Text = "Toggle",
						Icon = "arrow.left.arrow.right",
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Small,
						Command = viewModel.ToggleFromViewModelCommand
					})),
			ShowcaseBox.Code(Bind(model => model.BindingCode)));
	}

	void AddConfigurationShowcase(
		SwitchViewModel viewModel)
	{
		Switch toggle = new()
		{
			IsOn = viewModel.PreviewOn,
			OnColor = viewModel.SelectedOnColor.Value,
			ThumbColor = viewModel.SelectedThumbColor.Value,
			IsEnabled = viewModel.ControlEnabled
		};

		Picker<ShowcaseOption<Color?>> onColor = new()
		{
			MinWidth = 140,
			ItemsSource = viewModel.OnColors,
			SelectedItem = viewModel.SelectedOnColor,
			SelectionChanged = option =>
			{
				viewModel.SelectedOnColor = option;
				toggle.OnColor = option.Value;
			}
		};

		Picker<ShowcaseOption<Color?>> thumbColor = new()
		{
			MinWidth = 120,
			ItemsSource = viewModel.ThumbColors,
			SelectedItem = viewModel.SelectedThumbColor,
			SelectionChanged = option =>
			{
				viewModel.SelectedThumbColor = option;
				toggle.ThumbColor = option.Value;
			}
		};

		Switch previewOn = new()
		{
			IsOn = viewModel.PreviewOn,
			Toggled = value =>
			{
				viewModel.PreviewOn = value;
				toggle.IsOn = value;
			}
		};

		Switch enabled = new()
		{
			IsOn = viewModel.ControlEnabled,
			Toggled = value =>
			{
				viewModel.ControlEnabled = value;
				toggle.IsEnabled = value;
			}
		};

		AddShowcase(
			"Color & state",
			"Compare inherited and explicit colors across on, off, enabled and disabled native states.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 10,

						Children =
						{
							toggle,
							Status("The default on color follows the nearest SkeleKit tint.")
						}
					},
					170),
				SettingRow("On color", onColor),
				SettingRow("Thumb color", thumbColor),
				SettingRow("Preview on", previewOn),
				SettingRow("Control enabled", enabled)),
			ShowcaseBox.Code(Bind(model => model.ConfigurationCode)));
	}


	static View SettingsRow(
		string title,
		string summary,
		Switch toggle) =>
		new Grid
		{
			ColumnSpacing = 16,
			Padding = new(14, 12),
			Background = Colors.SecondaryBackground,
			CornerRadius = 12,

			Columns =
			{
				GridLength.Star,
				GridLength.Auto
			},

			Children =
			{
				new StackPanel
				{
					Spacing = 2,

					Children =
					{
						new Label
						{
							Text = title,
							TextStyle = TextStyle.Body,
							FontWeight = FontWeight.Medium
						},

						new Label
						{
							Text = summary,
							TextStyle = TextStyle.Footnote,
							TextColor = Colors.SecondaryLabel,
							MaxLines = 2
						}
					}
				},

				toggle.Column(1)
			}
		};

	static Label Status(
		BindingExpression<string?> text,
		FontWeight weight = FontWeight.Regular) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = text,
			TextStyle = TextStyle.Footnote,
			FontWeight = weight,
			TextColor = weight is FontWeight.Regular ? Colors.SecondaryLabel : (Color?)null,
			MaxLines = 2,
			TextAlignment = TextAlignment.Center
		};

	static Label Status(
		string text) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = text,
			TextStyle = TextStyle.Footnote,
			TextColor = Colors.SecondaryLabel,
			MaxLines = 2,
			TextAlignment = TextAlignment.Center
		};
}
