using BareUI.Gallery.Models;

namespace BareUI.Gallery.Views.Demos;

public class TodoCell : ItemView<TodoItem>
{
	public TodoCell() =>
		Content = new VStack
		{
			Spacing = 2,
			Padding = new Thickness(16, 10),
			Children =
			{
				new Label { Text = Bind(item => item.Title), FontSize = 17 },
				new Label { Text = Bind(item => item.Detail), FontSize = 13, TextColor = Theme.Secondary }
			}
		};
}
