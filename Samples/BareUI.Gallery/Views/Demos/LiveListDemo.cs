using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// An ObservableCollection driving a CollectionView: every change animates, and the EmptyView shows
/// itself once the list runs dry.
/// </summary>
[Page]
public class LiveListDemo : ContentView<LiveListDemoViewModel>
{
	public LiveListDemo(
		LiveListDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Live list";

		ToolbarItem select = new() { Text = "Select", Command = ViewModel.ToggleEditCommand };
		ToolbarItem delete = new() { Text = "Delete", IsVisible = false, Command = ViewModel.RemoveSelectedCommand };
		ToolbarItem menu = new()
		{
			Icon = "ellipsis.circle",
			Menu =
			{
				new() { Text = "Add", Icon = "plus", Command = ViewModel.AddCommand },
				new() { Text = "Shuffle", Icon = "shuffle", Command = ViewModel.ShuffleCommand },
				new() { Text = "Clear", Icon = "trash", IsDestructive = true, Command = ViewModel.ClearCommand }
			}
		};

		// live toolbar: Select flips to Done and Delete only exists in edit mode
		ViewModel.PropertyChanged += (_, e) =>
		{
			if (e.PropertyName != nameof(ViewModel.IsEditing))
				return;

			select.Text = ViewModel.IsEditing ? "Done" : "Select";
			delete.IsVisible = ViewModel.IsEditing;
			menu.IsVisible = !ViewModel.IsEditing;
		};

		ToolbarItems.Add(select);
		ToolbarItems.Add(delete);
		ToolbarItems.Add(menu);

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
					ReorderCommand = ViewModel.ReorderCommand,
					IsEditing = Bind(vm => vm.IsEditing, (vm, value) => vm.IsEditing = value),
					SelectedItems = ViewModel.Selected,
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
