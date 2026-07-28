namespace SkeleKit.Template;

[Page]
public class MainView : ContentView
{
	int count = 1;

	public MainView()
	{
		Label counterLabel;

		Content = new Grid()
		{
			VerticalAlignment = VerticalAlignment.Center,

			Rows =
			{
				GridLength.Auto,
				GridLength.Auto
			},
			RowSpacing = 4,

			Children =
			{
				(counterLabel = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,

					Text = "Count: 0",

					Shadow = new(1, 4, 2)
					{
						Color = Colors.Blue
					}
				}).Row(0),

				new Button
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Padding = new Thickness(24, 8),

					Text = "Click me",
					Kind = ButtonStyle.ProminentGlass,

					Command = Command.From(() => counterLabel.Text = $"Count: {count++}")
				}.Row(1)
			}
		};
	}
}
