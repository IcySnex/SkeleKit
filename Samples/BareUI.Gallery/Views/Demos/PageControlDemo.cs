using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// The dots that mark a paged scroll. Two-way: tap or scrub the dots, or drive them from the ViewModel.
/// </summary>
public class PageControlDemo : ContentView<PageControlDemoViewModel>
{
	public PageControlDemo(
		PageControlDemoViewModel viewModel) : base(viewModel)
	{
		Title = "PageControl";

		Content = new StackPanel
		{
			Spacing = 24,
			Margin = new Thickness(16),
			VerticalAlignment = VerticalAlignment.Center,
			Children =
			{
				new Label
				{
					Text = Bind(vm => vm.Page, page => $"Page {page + 1}"),
					TextStyle = TextStyle.LargeTitle,
					TextAlignment = TextAlignment.Center
				},

				new PageControl
				{
					Count = 5,
					Current = Bind(vm => vm.Page, (vm, value) => vm.Page = value),
					DotColor = Colors.Gray3,
					CurrentDotColor = Colors.Indigo
				},

				new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Spacing = 12,
					HorizontalAlignment = HorizontalAlignment.Center,
					Children =
					{
						new Button { Text = "Back", Kind = ButtonStyle.Gray, Command = ViewModel.PreviousCommand },
						new Button { Text = "Next", Kind = ButtonStyle.Filled, Command = ViewModel.NextCommand }
					}
				},

				new Label
				{
					Style = Styles.Caption,
					Text = "Drag across the dots to scrub, or drive them from the buttons.",
					TextAlignment = TextAlignment.Center
				}
			}
		};
	}
}
