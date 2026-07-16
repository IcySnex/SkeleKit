using BareUI;

namespace BareUI.Gallery.Views;

/// <summary>
/// A fake mini player for the tab accessory slot.
/// </summary>
public class PlayerBar : Overlay
{
	public PlayerBar()
	{
		Children.Add(new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Spacing = 12,
			Margin = new Thickness(12, 8),
			HorizontalAlignment = HorizontalAlignment.Start,
			VerticalAlignment = VerticalAlignment.Center,
			Children =
			{
				new Image
				{
					Source = ImageSource.Symbol("music.note"),
					SymbolSize = 20,
					Width = 36,
					Height = 36,
					CornerRadius = 8,
					Background = Colors.Gray5
				},

				new Label
				{
					Text = "Nothing playing",
					TextStyle = TextStyle.Subheadline,
					Bold = true,
					VerticalAlignment = VerticalAlignment.Center
				}
			}
		});

		Children.Add(new Button
		{
			Icon = "play.fill",
			Margin = new Thickness(0, 0, 12, 0),
			HorizontalAlignment = HorizontalAlignment.End,
			VerticalAlignment = VerticalAlignment.Center,
			Command = Command.From(() => Haptics.Impact())
		});
	}
}
