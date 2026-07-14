using BareUI.Gallery.Models;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A section footer in the settings list.
/// </summary>
public class SectionFooter : ItemView<SettingsSection>
{
	public SectionFooter() =>
		Content = new Label
		{
			Style = Styles.SectionFooter,
			Text = Bind(section => section.Footer)
		};
}
