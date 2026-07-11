using System.Windows.Input;
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

		list.Children.Add(new Button
		{
			Text = "MovieInfo",
			Style = ButtonStyle.Filled,
			Command = Bind<ICommand?>(vm => vm.OpenMovieCommand)
		});

		Content = new ScrollView { Content = list };
	}

	// the demo list needs the ViewModel instance; CollectionView's ItemsSource replaces this in M5
	protected override void OnViewModelAttached()
	{
		foreach (DemoEntry demo in ViewModel!.Demos)
			list.Children.Add(new Button
			{
				Text = demo.Title,
				Style = ButtonStyle.Gray,
				Command = Bindable.From<ICommand?>(ViewModel.OpenDemoCommand),
				CommandParameter = demo
			});
	}
}
