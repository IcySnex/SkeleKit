using CommunityToolkit.Mvvm.ComponentModel;

namespace SkeleKit.Gallery.ViewModels.Demos;

public partial class KeyboardDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Message { get; set; } = "";
}
