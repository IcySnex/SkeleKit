using System.ComponentModel;
using SkeleKit.Gallery.ViewModels;

namespace SkeleKit.Gallery.Views.Shared;

internal abstract class TintView<TViewModel> : ContentView<TViewModel>
	where TViewModel : class
{
	readonly Color tint;
	readonly GalleryViewModel? galleryViewModel;
	readonly ToolbarItem? appearanceItem;


	protected TintView(
		TViewModel viewModel,
		Color tint) : base(viewModel)
	{
		this.tint = tint;

		if (viewModel is GalleryViewModel gallery)
		{
			galleryViewModel = gallery;
			appearanceItem = new()
			{
				Icon = AppearanceIcon(gallery.Appearance),
				Command = gallery.CycleAppearanceCommand
			};

			ToolbarItems.Add(appearanceItem);
		}
	}


	static string AppearanceIcon(
		Appearance appearance) =>
		appearance switch
		{
			Appearance.Dark => "moon.fill",
			Appearance.Light => "sun.max.fill",
			_ => "circle.lefthalf.filled"
		};


	void AppearanceChanged(
		object? sender,
		PropertyChangedEventArgs args)
	{
		if (args.PropertyName == nameof(GalleryViewModel.Appearance))
			ApplyAppearance();
	}

	void ApplyAppearance()
	{
		if (galleryViewModel is not null && appearanceItem is not null)
			appearanceItem.Icon = AppearanceIcon(galleryViewModel.Appearance);
	}


	protected override void OnLoaded()
	{
		base.OnLoaded();

		if (galleryViewModel is not null)
			galleryViewModel.PropertyChanged += AppearanceChanged;
	}

	protected override void OnUnloaded()
	{
		if (galleryViewModel is not null)
			galleryViewModel.PropertyChanged -= AppearanceChanged;

		base.OnUnloaded();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		ApplyAppearance();

		if (SkeleApplication.Current is SkeleApplication app)
			app.Tint = tint;
	}
}
