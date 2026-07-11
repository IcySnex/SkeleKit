using BareUI.Gallery.ViewModels;

namespace BareUI.Gallery.Views;

/// <summary>
/// Renders whichever control demo the menu pushed.
/// </summary>
public class DemoView : ContentView<DemoViewModel>
{
	protected override View Build()
	{
		Title = ViewModel!.Title;

		return ViewModel.Content();
	}
}
