using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class SharingViewModel(
	ISharer sharer) : ShowcaseViewModel
{
	[ObservableProperty]
	string textResult = "Not shown";

	[ObservableProperty]
	string linkResult = "Not shown";

	[ObservableProperty]
	string imageResult = "Not shown";

	[ObservableProperty]
	string combinedResult = "Not shown";

	public IReadOnlyList<Span> TextCode { get; } =
	[
		new(
			"""
			await sharer.ShareAsync("Sample text");
			""")
	];

	public IReadOnlyList<Span> LinkCode { get; } =
	[
		new(
			"""
			await sharer.ShareAsync(
				new Uri("https://example.com"));
			""")
	];

	public IReadOnlyList<Span> ImageCode { get; } =
	[
		new(
			"""
			await sharer.ShareAsync(
				ImageSource.Symbol("photo"));
			""")
	];

	public IReadOnlyList<Span> CombinedCode { get; } =
	[
		new(
			"""
			await sharer.ShareAsync(new ShareContent
			{
				Text = "Sample content",
				Url = new Uri("https://example.com"),
				Image = ImageSource.Symbol("photo")
			});
			""")
	];


	[RelayCommand]
	async Task ShareTextAsync()
	{
		TextResult = "Presented";
		await sharer.ShareAsync("Sample text");
		TextResult = "Dismissed";
	}

	[RelayCommand]
	async Task ShareLinkAsync()
	{
		LinkResult = "Presented";
		await sharer.ShareAsync(new Uri("https://example.com"));
		LinkResult = "Dismissed";
	}

	[RelayCommand]
	async Task ShareImageAsync()
	{
		ImageResult = "Presented";
		await sharer.ShareAsync(ImageSource.Symbol("photo"));
		ImageResult = "Dismissed";
	}

	[RelayCommand]
	async Task ShareCombinedAsync()
	{
		CombinedResult = "Presented";

		await sharer.ShareAsync(new ShareContent
		{
			Text = "Sample content",
			Url = new Uri("https://example.com"),
			Image = ImageSource.Symbol("photo")
		});

		CombinedResult = "Dismissed";
	}
}
