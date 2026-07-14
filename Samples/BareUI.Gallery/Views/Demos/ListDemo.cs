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

		ToolbarItems.Add(new() { Icon = "plus", IsPrimary = true, Command = ViewModel.AddSettingCommand });

		Content = new CollectionView<SettingsEntry, SettingsSection>
		{
			Layout = CollectionLayout.List(grouped: true),
			ItemTemplate = () => new SettingsCell(),
			HeaderTemplate = () => new SectionHeader(),
			FooterTemplate = () => new SectionFooter(),
			GroupedItemsSource = ViewModel.Sections,
			SelectionCommand = ViewModel.OpenCommand,
			SeparatorInsets = new Thickness(52, 0, 0, 0),
			HighlightsSelection = false
		};
	}
}
