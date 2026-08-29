using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ValuesStatus;

[Page]
internal sealed class ActivityIndicatorView : ShowcaseView<ActivityIndicatorViewModel>
{
	public ActivityIndicatorView(
		ActivityIndicatorViewModel viewModel) : base(viewModel, "Activity Indicator", Colors.Red)
	{
		AddActivityShowcase(viewModel);
	}


	void AddActivityShowcase(
		ActivityIndicatorViewModel viewModel)
	{
		ActivityIndicator indicator = new()
		{
			IsAnimating = Bind(vm => vm.IsAnimating),
			IsLarge = viewModel.IsLarge,
			Color = Colors.Red
		};

		Switch animating = new()
		{
			IsOn = Bind(vm => vm.IsAnimating)
				.TwoWay((vm, val) => vm.IsAnimating = val)
		};

		Switch size = new()
		{
			IsOn = Bind(vm => vm.IsLarge)
				.TwoWay((vm, val) => vm.IsLarge = val),
			Toggled = value =>
			{
				indicator.IsLarge = value;
			}
		};

		AddShowcase(
			"Loading state",
			"Switch the native indicator between medium and large, then stop and restart its animation.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(indicator, 150),
				SettingRow("Large", size),
				SettingRow("Animating", animating)),
			Code(vm => vm.IndicatorCode));
	}
}
