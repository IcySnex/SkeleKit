using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SkeleKit.Gallery.Views.Pages;

namespace SkeleKit.Gallery.ViewModels;

internal abstract class GalleryViewModel : INotifyPropertyChanged
{
	readonly INavigator navigator;


	protected GalleryViewModel(
		INavigator navigator)
	{
		this.navigator = navigator;

		CycleAppearanceCommand = Command.From(CycleAppearance);
		ShowInfoCommand = Command.From(ShowInfo);
	}


	public event PropertyChangedEventHandler? PropertyChanged;


	public Appearance Appearance =>
		SkeleApplication.Current?.Appearance ?? Appearance.System;

	public ICommand CycleAppearanceCommand { get; }
	public ICommand ShowInfoCommand { get; }


	protected void OnPropertyChanged(
		[CallerMemberName] string? name = null) =>
		PropertyChanged?.Invoke(this, new(name));


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

	void ShowInfo() =>
		_ = navigator.PresentViewAsync<AboutView>(ModalStyle.Sheet(Detent.Content, Detent.Large));
}
