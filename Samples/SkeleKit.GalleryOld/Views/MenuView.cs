using System.Windows.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels;

namespace SkeleKit.Gallery.Views;

[Page(Singleton = true)]
public class MenuView : ContentView<MenuViewModel>
{
	public MenuView(
		MenuViewModel viewModel) : base(viewModel)
	{
		Title = "SkeleKit Gallery";

		// slim page (ADR-013): pushed as an instance, no ViewModel behind it
		ToolbarItems.Add(new()
		{
			Icon = "info.circle",
			Command = Command.From(() =>
			{
				Haptics.Play(
					HapticEvent.Tap(0),
					HapticEvent.Continuous(0.1, 0.3, intensity: 0.6, sharpness: 0.2),
					HapticEvent.Tap(0.5));

				Navigator.PushViewAsync(new AboutView("SkeleKit Gallery", "1.0"));
			})
		});

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
					Command = ViewModel.OpenMovieCommand
				}.Row(0),

				new CollectionView<DemoEntry>
				{
					Layout = CollectionLayout.List(),
					ItemTemplate = () => new DemoRow(),
					ItemsSource = ViewModel.Demos,
					ItemCommand = ViewModel.OpenDemoCommand,
					IgnoresSafeArea = SafeAreaEdges.Bottom,
					HighlightColor = Palette.Highlight
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
