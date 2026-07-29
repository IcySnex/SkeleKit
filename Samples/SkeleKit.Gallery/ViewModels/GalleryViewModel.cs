using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Views.Pages;

namespace SkeleKit.Gallery.ViewModels;

internal abstract partial class GalleryViewModel : ObservableObject
{
	readonly INavigator navigator;


	protected GalleryViewModel(
		INavigator navigator)
	{
		this.navigator = navigator;
	}


	public Appearance Appearance =>
		SkeleApplication.Current?.Appearance ?? Appearance.System;


	[RelayCommand]
	void CycleAppearance()
	{
		if (SkeleApplication.Current is not SkeleApplication app)
			return;

		app.Appearance = app.Appearance switch
		{
			Appearance.System => Appearance.Dark,
			Appearance.Dark => Appearance.Light,
			_ => Appearance.System
		};

		OnPropertyChanged(nameof(Appearance));
	}

	[RelayCommand]
	Task ShowInfoAsync() =>
		navigator.PresentViewAsync<AboutView>(ModalStyle.Sheet(Detent.Content, Detent.Large));
}
