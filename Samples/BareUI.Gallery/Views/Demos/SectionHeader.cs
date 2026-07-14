using BareUI.Gallery.Models;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A section header in the settings list, bound to the app's own section model.
/// </summary>
public class SectionHeader : ItemView<SettingsSection>
{
	public SectionHeader() =>
		Content = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Spacing = 6,
			Margin = new Thickness(16, 8),
			Children =
			{
				new Image
				{
					Source = Bind<string, ImageSource?>(section => section.Icon, icon => ImageSource.Symbol(icon)),
					Tint = Palette.Secondary,
					Width = 14,
					Height = 14
				},

				new Label
				{
					Style = Styles.SectionHeader,
					Text = Bind(section => section.Title),
					HorizontalAlignment = HorizontalAlignment.Start
				}
			}
		};
}
