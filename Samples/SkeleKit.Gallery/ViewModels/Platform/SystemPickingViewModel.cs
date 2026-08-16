using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class SystemPickingViewModel : ShowcaseViewModel
{
	readonly ISystemPicker systemPicker;


	public SystemPickingViewModel(
		ISystemPicker systemPicker)
	{
		this.systemPicker = systemPicker;

		SelectedImageLimit = ImageLimits[0];
		SelectedFileFilter = FileFilters[0];
	}


	public List<ShowcaseOption<int>> ImageLimits { get; } =
	[
		new("1 image", 1),
		new("Up to 3", 3)
	];

	public List<ShowcaseOption<string[]>> FileFilters { get; } =
	[
		new("Any file", []),
		new("PDF", ["pdf"]),
		new("Text", ["txt"])
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ImagesCode))]
	ShowcaseOption<int> selectedImageLimit = null!;

	[ObservableProperty]
	ImageSource? imagePreview = ImageSource.Symbol("photo");

	[ObservableProperty]
	string imagesResult = "No result";

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(FileCode))]
	ShowcaseOption<string[]> selectedFileFilter = null!;

	[ObservableProperty]
	string fileResult = "No result";

	public IReadOnlyList<Span> ImagesCode =>
	[
		new(
			$$"""
			PickedAsset[]? images =
				await systemPicker.PickImagesAsync(limit: {{SelectedImageLimit.Value}});

			if (images is { Length: > 0 } picked)
				preview.Source = ImageSource.Data(picked[0].Data);
			""")
	];

	public IReadOnlyList<Span> FileCode
	{
		get
		{
			string extensions = string.Join(
				", ",
				SelectedFileFilter.Value.Select(extension => $"\"{extension}\""));
			string invocation = extensions.Length == 0
				? "systemPicker.PickFileAsync()"
				: $"systemPicker.PickFileAsync({extensions})";

			return
			[
				new(
					$$"""
					PickedAsset? file = await {{invocation}};

					if (file is PickedAsset picked)
						status.Text = $"{picked.Name} · {picked.Data.Length} bytes";
					""")
			];
		}
	}


	[RelayCommand]
	async Task PickImagesAsync()
	{
		PickedAsset[]? images = await systemPicker.PickImagesAsync(SelectedImageLimit.Value);

		if (images is not { Length: > 0 })
		{
			ImagePreview = ImageSource.Symbol("photo");
			ImagesResult = "Canceled";
			return;
		}

		ImagePreview = ImageSource.Data(images[0].Data);
		ImagesResult = images.Length == 1
			? $"{images[0].Name} · {FormatSize(images[0].Data.Length)}"
			: $"{images.Length} images · {FormatSize(images.Sum(image => (long)image.Data.Length))}";
	}

	[RelayCommand]
	async Task PickFileAsync()
	{
		PickedAsset? file = await systemPicker.PickFileAsync(SelectedFileFilter.Value);

		FileResult = file is null
			? "Canceled"
			: $"{file.Name} · {FormatSize(file.Data.Length)}";
	}


	static string FormatSize(
		long bytes) =>
		bytes switch
		{
			< 1024 => $"{bytes} B",
			< 1024 * 1024 => $"{bytes / 1024d:0.#} KB",
			_ => $"{bytes / (1024d * 1024d):0.#} MB"
		};
}
