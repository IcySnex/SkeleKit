using System.Windows.Input;
using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A native inset-grouped list, the shape Velura's Settings screen needs.
/// </summary>
public class ListDemo : ContentView<ListDemoViewModel>
{
	readonly CollectionView<SettingsEntry> entries = new()
	{
		Layout = CollectionLayout.List(grouped: true),
		ItemTemplate = () => new SettingsCell(),
		HeaderTemplate = () => new SectionHeader()
	};

	public ListDemo(
		ListDemoViewModel viewModel) : base(viewModel)
	{
		Title = "List";

		// the large title collapses as the list scrolls
		TitleStyle = TitleStyle.Large;

		Content = entries;

		AttachViewModel();
		}

	void AttachViewModel()
	{
		entries.GroupedItemsSource = Bindable.From<IReadOnlyList<Section<SettingsEntry>>?>(ViewModel!.Sections);
		entries.SelectionCommand = ViewModel.OpenCommand;
	}
}
