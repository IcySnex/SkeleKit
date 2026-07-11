namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A section header in the settings list.
/// </summary>
public class SectionHeader : ItemView<Section<Models.SettingsEntry>>
{
	public SectionHeader() =>
		Content = new Label
		{
			Text = Bind(section => section.Title),
			FontSize = 13,
			Bold = true,
			TextColor = Theme.Secondary,
			Margin = new Thickness(16, 8)
		};
}
