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
			IsAnimating = Bind(model => model.IsAnimating),
			IsLarge = viewModel.IsLarge,
			Color = Colors.Red
		};

		Switch animating = new()
		{
			IsOn = Bind(
				model => model.IsAnimating,
				static (model, value) => model.IsAnimating = value)
		};

		Switch size = new()
		{
			IsOn = Bind(
				model => model.IsLarge,
				static (model, value) => model.IsLarge = value),
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
			ShowcaseBox.Code(Bind(model => model.IndicatorCode)));
	}
}
