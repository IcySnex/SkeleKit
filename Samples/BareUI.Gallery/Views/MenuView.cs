using System.Windows.Input;
using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels;

namespace BareUI.Gallery.Views;

public class MenuView : ContentView<MenuViewModel>
{
	public MenuView()
	{
		Title = "BareUI Gallery";

		Content = new Grid
		{
			Rows = { GridLength.Auto, GridLength.Star },
			RowSpacing = 8,
			Padding = new Thickness(0, 8),
			Children =
			{
				new Button
				{
					Text = "MovieInfo",
					Kind = ButtonStyle.Filled,
					Margin = new Thickness(16, 0),
					Command = Bind<ICommand?>(vm => vm.OpenMovieCommand)
				}.Row(0),

				new CollectionView<DemoEntry>
				{
					Layout = CollectionLayout.List(),
					ItemTemplate = () => new DemoRow(),
					ItemsSource = Bind<IReadOnlyList<DemoEntry>?>(vm => vm.Demos),
					SelectionCommand = Bind<ICommand?>(vm => vm.OpenDemoCommand),
					IgnoresSafeArea = SafeAreaEdges.Bottom
				}.Row(1)
			}
		};
	}
}

/// <summary>
/// One tappable demo row.
/// </summary>
public class DemoRow : ItemView<DemoEntry>
{
	public DemoRow() =>
		Content = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Padding = new Thickness(16, 12),
			Children =
			{
				new Label
				{
					Text = Bind(vm => vm.Title),
					TextStyle = TextStyle.Body,
					HorizontalAlignment = HorizontalAlignment.Start
				},

				new Label
				{
					Text = "›",
					TextStyle = TextStyle.Body,
					TextColor = Palette.Secondary,
					HorizontalAlignment = HorizontalAlignment.End
				}
			}
		};
}
