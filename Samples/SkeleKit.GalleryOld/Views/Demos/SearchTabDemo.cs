using SkeleKit;
using SkeleKit.Gallery.ViewModels.Demos;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// The search tab's page: the tab bar itself morphs into the search field.
/// </summary>
[Page(Singleton = true)]
public class SearchTabDemo : ContentView<SearchTabDemoViewModel>
{
	public SearchTabDemo(
		SearchTabDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Search";
		SearchPlaceholder = "Search demos";
		SearchText = Bind(
			model => model.Query,
			static (model, value) => model.Query = value ?? "");
		SearchCanceled = viewModel.CancelSearch;

		Content = new StackPanel
		{
			Spacing = 12,
			Margin = new Thickness(16),
			Children =
			{
				new Label { Style = Styles.Caption, Text = "Tapping this tab morphs the bar into the search field" },
				new Label { Text = Bind(vm => vm.Status) }
			}
		};
	}
}
