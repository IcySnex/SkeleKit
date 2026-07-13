using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A native inset-grouped list, the shape Velura's Settings screen needs.
/// </summary>
public class ListDemo : ContentView<ListDemoViewModel>
{
	public ListDemo(
		ListDemoViewModel viewModel) : base(viewModel)
	{
		Title = "List";

		// the large title collapses as the list scrolls
		TitleStyle = TitleStyle.Large;

		Content = new CollectionView<SettingsEntry>
		{
			Layout = CollectionLayout.List(grouped: true),
			ItemTemplate = () => new SettingsCell(),
			HeaderTemplate = () => new SectionHeader(),
			GroupedItemsSource = ViewModel.Sections,
			SelectionCommand = ViewModel.OpenCommand
		};
	}
}
