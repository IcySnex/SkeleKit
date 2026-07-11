using BareUI.Gallery.Models;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// One row of the settings-style list.
/// </summary>
public class SettingsCell : ItemView<SettingsEntry>
{
	public SettingsCell() =>
		Content = new HStack
		{
			Spacing = 12,
			Padding = new Thickness(16, 12),
			Children =
			{
				new Image
				{
					Source = Bind<string, ImageSource?>(vm => vm.Icon, icon => ImageSource.Symbol(icon)),
					Width = 24,
					Height = 24
				},

				new Label
				{
					Text = Bind(vm => vm.Title),
					FontSize = 17,
					HorizontalAlignment = HorizontalAlignment.Start
				},

				new Label
				{
					Text = Bind(vm => vm.Detail),
					FontSize = 15,
					TextColor = Theme.Secondary,
					HorizontalAlignment = HorizontalAlignment.End,
					TextAlignment = TextAlignment.Trailing
				}
			}
		};
}
