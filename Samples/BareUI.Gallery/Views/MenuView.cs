using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels;

namespace BareUI.Gallery.Views;

public class MenuView : ContentView<MenuViewModel>
{
	protected override View Build()
	{
		Title = "BareUI Gallery";

		VStack list = new()
		{
			Spacing = 12,
			Margin = new Thickness(16),
			Children =
			{
				new Button
				{
					Text = "MovieInfo",
					Style = ButtonStyle.Filled,
					Command = ViewModel!.OpenMovieCommand
				}
			}
		};

		foreach (DemoEntry demo in ViewModel.Demos)
			list.Children.Add(new Button
			{
				Text = demo.Title,
				Style = ButtonStyle.Gray,
				Command = ViewModel.OpenDemoCommand,
				CommandParameter = demo
			});

		return new ScrollView { Content = list };
	}
}
