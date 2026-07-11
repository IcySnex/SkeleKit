using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels;

namespace BareUI.Gallery.Views;

public class MenuView : ContentView<MenuViewModel>
{
	readonly VStack list = new()
	{
		Spacing = 12,
		Margin = new Thickness(16)
	};

	public MenuView()
	{
		Title = "BareUI Gallery";

		Content = new ScrollView { Content = list };
	}

	// the demo list comes from the ViewModel, which arrives after construction
	protected override void OnViewModelAttached()
	{
		list.Children.Clear();

		list.Children.Add(new Button
		{
			Text = "MovieInfo",
			Style = ButtonStyle.Filled,
			Command = ViewModel!.OpenMovieCommand
		});

		foreach (DemoEntry demo in ViewModel.Demos)
			list.Children.Add(new Button
			{
				Text = demo.Title,
				Style = ButtonStyle.Gray,
				Command = ViewModel.OpenDemoCommand,
				CommandParameter = demo
			});
	}
}
