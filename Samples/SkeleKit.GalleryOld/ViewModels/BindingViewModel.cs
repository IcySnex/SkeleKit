using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels;

public partial class BindingViewModel : ObservableObject
{
	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(ClearNameCommand))]
	public partial string Name { get; set; } = "Kevin";

	[ObservableProperty]
	public partial bool IsSubscribed { get; set; } = true;

	[ObservableProperty]
	public partial double Volume { get; set; } = 42;

	[RelayCommand(CanExecute = nameof(CanClearName))]
	void ClearName() =>
		Name = "";

	bool CanClearName() =>
		Name.Length > 0;
}
