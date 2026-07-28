using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels.Demos;

public partial class SystemPickerDemoViewModel(
	ISystemPicker systemPicker) : ObservableObject
{
	[ObservableProperty]
	public partial ImageSource? Photo { get; set; }

	[ObservableProperty]
	public partial string Status { get; set; } = "Nothing picked yet.";

	[RelayCommand]
	async Task PickPhoto()
	{
		if (await systemPicker.PickImagesAsync() is PickedAsset[] picked)
		{
			Photo = ImageSource.Data(picked[0].Data);
			Status = $"Photo: {picked[0].Data.Length / 1024} KB, {picked[0].Name}";
		}
		else
		{
			Photo = null;
			Status = "Canceled";
		}
	}

	[RelayCommand]
	async Task PickFile()
	{
		if (await systemPicker.PickFileAsync() is PickedAsset picked)
		{
			Photo = null;
			Status = $"File: {picked.Data.Length / 1024} KB, {picked.Name}";
		}
		else
		{
			Photo = null;
			Status = "Canceled";
		}
	}
}
