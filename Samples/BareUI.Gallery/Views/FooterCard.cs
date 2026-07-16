using BareUI;

namespace BareUI.Gallery.Views;

/// <summary>
/// A slim view for the iPad sidebar's bottom bar.
/// </summary>
public class FooterCard : StackPanel
{
	public FooterCard()
	{
		Orientation = Orientation.Horizontal;
		Spacing = 10;
		Margin = new Thickness(16, 10);

		Children.Add(new Image { Source = ImageSource.Symbol("person.crop.circle"), SymbolSize = 26, VerticalAlignment = VerticalAlignment.Center });
		Children.Add(new Label { Text = "BareUI Gallery 1.0", TextStyle = TextStyle.Footnote, VerticalAlignment = VerticalAlignment.Center });
	}
}
