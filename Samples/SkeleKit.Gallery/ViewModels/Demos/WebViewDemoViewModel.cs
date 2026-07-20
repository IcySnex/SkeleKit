using CommunityToolkit.Mvvm.ComponentModel;

namespace SkeleKit.Gallery.ViewModels.Demos;

public partial class WebViewDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Url { get; set; } = "https://www.apple.com";

	[ObservableProperty]
	public partial string? Address { get; set; }
}
