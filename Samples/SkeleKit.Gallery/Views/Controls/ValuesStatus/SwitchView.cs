using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ValuesStatus;

[Page]
internal sealed class SwitchView : ShowcaseView<SwitchViewModel>
{
	public SwitchView(
		SwitchViewModel viewModel) : base(viewModel, "Switch", Colors.Red)
	{
		AddSwitchShowcase(viewModel);
	}


	void AddSwitchShowcase(
		SwitchViewModel viewModel)
	{
		Switch toggle = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			IsOn = Bind(
				model => model.IsOn,
				static (model, value) => model.IsOn = value),
			IsEnabled = viewModel.ControlEnabled,
			Toggled = viewModel.RecordToggle
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
			"State",
			"Toggle the native control, update it from the ViewModel, and compare enabled and disabled interaction.",
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
							Status(Bind(model => model.StateSummary), FontWeight.Medium),
							Status(Bind(model => model.ToggleStatus))
						}
					},
					170),
				SettingRow(
					"Bound value",
					new Button
					{
						Text = "Toggle",
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Small,
						Command = viewModel.ToggleFromViewModelCommand
					}),
				SettingRow("Enabled", enabled)),
			ShowcaseBox.Code(Bind(model => model.SwitchCode)));
	}


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
}
