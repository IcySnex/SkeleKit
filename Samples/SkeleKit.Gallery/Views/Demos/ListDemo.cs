using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Demos;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// A native inset-grouped list, the shape Velura's Settings screen needs.
/// </summary>
[Page]
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

			// no context menu here, so the drag starts straight from a long-press; crossing sections works
			ReorderCommand = ViewModel.ReorderCommand,
			SeparatorInsets = new Thickness(52, 0, 0, 0),
			HighlightsSelection = false
		};
	}
}
