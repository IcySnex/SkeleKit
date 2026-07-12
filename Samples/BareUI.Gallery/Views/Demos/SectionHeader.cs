namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A section header in the settings list.
/// </summary>
public class SectionHeader : ItemView<Section<Models.SettingsEntry>>
{
	public SectionHeader() =>
		Content = new Label
		{
			Style = Styles.SectionHeader,
			Text = Bind(section => section.Title)
		};
}
