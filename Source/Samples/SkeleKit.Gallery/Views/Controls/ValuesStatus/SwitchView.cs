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
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			IsOn = Bind(
				model => model.IsOn,
				static (model, value) => model.IsOn = value),
			IsEnabled = Bind(model => model.ControlEnabled)
		};

		Switch enabled = new()
		{
			IsOn = Bind(
				model => model.ControlEnabled,
				static (model, value) => model.ControlEnabled = value)
		};

		AddShowcase(
			"State",
			"Toggle the native control, update it from the ViewModel, and compare enabled and disabled interaction.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(toggle, 150),
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
			Code(model => model.SwitchCode));
	}

}
