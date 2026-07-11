using System.Windows.Input;
using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// An ObservableCollection driving a CollectionView: every change animates, and the EmptyView shows
/// itself once the list runs dry.
/// </summary>
public class LiveListDemo : ContentView<LiveListDemoViewModel>
{
	readonly CollectionView<TodoItem> items = new()
	{
		Layout = CollectionLayout.List(),
		ItemTemplate = () => new TodoCell(),
		EmptyView = new Label
		{
			Text = "Nothing here yet — tap Add",
			TextColor = Theme.Secondary,
			TextAlignment = TextAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		}
	};

	public LiveListDemo()
	{
		Title = "Live list";

		Content = new Grid
		{
			Rows = { GridLength.Auto, GridLength.Star },
			RowSpacing = 8,
			Padding = new Thickness(0, 8),
			Children =
			{
				new HStack
				{
					Spacing = 8,
					Padding = new Thickness(16, 0),
					Children =
					{
						new Button { Text = "Add", Style = ButtonStyle.Tinted, Command = Bind<ICommand?>(vm => vm.AddCommand) },
						new Button { Text = "Remove", Style = ButtonStyle.Gray, Command = Bind<ICommand?>(vm => vm.RemoveCommand) },
						new Button { Text = "Move", Style = ButtonStyle.Gray, Command = Bind<ICommand?>(vm => vm.ShuffleCommand) },
						new Button { Text = "Clear", Style = ButtonStyle.Gray, Command = Bind<ICommand?>(vm => vm.ClearCommand) }
					}
				}.Row(0),

				items.Row(1)
			}
		};
	}

	protected override void OnViewModelAttached() =>
		items.ItemsSource = Bindable.From<IReadOnlyList<TodoItem>?>(ViewModel!.Items);
}
