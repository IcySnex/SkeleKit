using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels.Showcase;

internal abstract partial class ShowcaseViewModel : ObservableObject
{
	[ObservableProperty]
	public partial Appearance Appearance { get; set; } = SkeleApplication.Current?.Appearance ?? Appearance.System;

	[RelayCommand]
	void CycleAppearance() =>
		Appearance = Appearance switch
		{
			Appearance.System => Appearance.Dark,
			Appearance.Dark => Appearance.Light,
			_ => Appearance.System
		};

	partial void OnAppearanceChanged(
		Appearance value)
	{
		if (SkeleApplication.Current is SkeleApplication app)
			app.Appearance = value;
	}
}
