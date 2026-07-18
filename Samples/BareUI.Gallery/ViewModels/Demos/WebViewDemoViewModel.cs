using CommunityToolkit.Mvvm.ComponentModel;

namespace BareUI.Gallery.ViewModels.Demos;

public partial class WebViewDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Url { get; set; } = "https://www.apple.com";
}
