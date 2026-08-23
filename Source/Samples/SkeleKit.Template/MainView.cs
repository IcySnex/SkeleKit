using SkeleKit;

namespace SkeleKit.Template;

[Page]
public class MainView : ContentView
{
	int count;


	public MainView()
	{
		Label counterLabel;


		Content = new StackPanel()
		{
			VerticalAlignment = VerticalAlignment.Center,
			Spacing = 4,

			Children =
			{
				(counterLabel = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Text = "Count: 0"
				}),

				new Button
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Text = "Click me",

					Command = Command.From(() => counterLabel.Text = $"Count: {++count}")
				}
			}
		};
	}
}
