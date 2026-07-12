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
				new Label { TextStyle = TextStyle.Body, Text = Bind(item => item.Title) },
				new Label { Style = Styles.Caption, Text = Bind(item => item.Detail) }
			}
		};
}
