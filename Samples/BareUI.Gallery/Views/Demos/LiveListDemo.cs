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
	public LiveListDemo(
		LiveListDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Live list";

		ToolbarItems.Add(new() { Icon = "plus", IsPrimary = true, Command = ViewModel.AddCommand });
		ToolbarItems.Add(new() { Text = "Clear", Side = ToolbarSide.Leading, Command = ViewModel.ClearCommand });
		ToolbarItems.Add(new()
		{
			Icon = "ellipsis.circle",
			Menu =
			{
				new() { Text = "Add", Icon = "plus", Command = ViewModel.AddCommand },
				new() { Text = "Shuffle", Icon = "shuffle", Command = ViewModel.ShuffleCommand },
				new() { Text = "Clear", Icon = "trash", IsDestructive = true, Command = ViewModel.ClearCommand }
			}
		});

		Content = new Grid
		{
			Rows = { GridLength.Auto, GridLength.Star },
			RowSpacing = 8,
			Padding = new Thickness(0, 8),
			Children =
			{
				new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Spacing = 8,
					Padding = new Thickness(16, 0),
					Children =
					{
						new Button { Text = "Add", Kind = ButtonStyle.Tinted, Command = ViewModel.AddCommand },
						new Button { Text = "Remove", Kind = ButtonStyle.Gray, Command = ViewModel.RemoveCommand },
						new Button { Text = "Move", Kind = ButtonStyle.Gray, Command = ViewModel.ShuffleCommand },
						new Button { Text = "Clear", Kind = ButtonStyle.Gray, Command = ViewModel.ClearCommand }
					}
				}.Row(0),

				new CollectionView<TodoItem>
				{
					Layout = CollectionLayout.List(),
					ItemTemplate = () => new TodoCell(),
					ItemsSource = ViewModel.Items,
					RefreshCommand = ViewModel.RefreshCommand,
					IsRefreshing = Bind(vm => vm.IsRefreshing, (vm, value) => vm.IsRefreshing = value),

					// the list scrolls under the tab bar; its content stays above it
					IgnoresSafeArea = SafeAreaEdges.Bottom,
					EmptyView = new Label
					{
						Style = Styles.Detail,
						Text = "Nothing here yet — tap Add",
						TextAlignment = TextAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center
					},

					// native swipe: UIKit owns the gesture and the full-swipe-to-delete
					SwipeActions =
					{
						new() { Text = "Delete", Icon = "trash", IsDestructive = true, Command = ViewModel.DeleteCommand }
					},
					ItemContextMenu =
					{
						new() { Text = "Rename", Icon = "pencil", Command = ViewModel.RenameCommand },
						new() { Text = "Duplicate", Icon = "plus.square.on.square", Command = ViewModel.DuplicateCommand },
						new() { Text = "Delete", Icon = "trash", IsDestructive = true, Command = ViewModel.DeleteCommand }
					}
				}.Row(1)
			}
		};
	}
}
