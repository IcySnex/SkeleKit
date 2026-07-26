namespace SkeleKit.Template;

[Page]
public class MainView : ContentView
{
	public MainView()
	{
		Title = "SkeleKit Template";

		Content = new ScrollView()
		{
			VerticalAlignment = VerticalAlignment.Center,
			Height = 0,
			Background = Color.FromHex(0x000000),
			Children =
			{
				new Label()
				{
					Background = Colors.Red,
					Text = "Hello World!"
				}
			}
		};
	}
}
