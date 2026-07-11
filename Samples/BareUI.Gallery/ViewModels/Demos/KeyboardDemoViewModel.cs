using CommunityToolkit.Mvvm.ComponentModel;

namespace BareUI.Gallery.ViewModels.Demos;

public partial class KeyboardDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Message { get; set; } = "";
}
