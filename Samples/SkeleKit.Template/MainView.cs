namespace SkeleKit.Template;

[Page]
public class MainView : ContentView
{
	public MainView()
	{
		Title = "SkeleKit Template";

		Content = new ScrollView()
		{
			Children =
			{
				new Label()
				{
					Background = Colors.Blue,
					Text = "Hello World!"
				}
			}
		};
	}
}
