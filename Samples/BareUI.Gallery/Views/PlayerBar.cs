using BareUI;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views;

/// <summary>
/// A fake mini player for the tab accessory slot.
/// </summary>
public class PlayerBar : Overlay
{
	public PlayerBar(
		PlayerBarViewModel viewModel)
	{
		IsVisible = BindingFactory.Bind((PlayerBarViewModel vm) => vm.Visible);

		Children.Add(new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Spacing = 12,
			Margin = new Thickness(12, 12),
			HorizontalAlignment = HorizontalAlignment.Start,
			VerticalAlignment = VerticalAlignment.Center,
			Children =
			{
				// a flat background here would be repainted by the slot's glass treatment
				new Image
				{
					Source = ImageSource.Symbol("music.note"),
					SymbolSize = 22,
					Width = 32,
					Height = 32,
					VerticalAlignment = VerticalAlignment.Center
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
			Command = viewModel.PlayCommand
		});
	}
}
