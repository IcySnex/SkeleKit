using System.Collections;
using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// One row of the settings-style list.
/// </summary>
public class SettingsCell : ItemView<SettingsEntry>
{
	public SettingsCell() =>
		Content = new StackPanel
		{
			Orientation = Orientation.Horizontal,
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
					TextStyle = TextStyle.Body,
					HorizontalAlignment = HorizontalAlignment.Start
				},

				new Label
				{
					Style = Styles.Detail,
					Text = Bind(vm => vm.Detail),
					HorizontalAlignment = HorizontalAlignment.End,
					TextAlignment = TextAlignment.Trailing
				}
			}
		};
}
