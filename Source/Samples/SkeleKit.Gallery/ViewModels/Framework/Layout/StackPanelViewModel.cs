using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Layout;

internal sealed partial class StackPanelViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(Orientation))]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	int orientationIndex;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SpacingLabel))]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	double spacing = 12;


	internal Orientation Orientation =>
		OrientationIndex == 1
			? Orientation.Horizontal
			: Orientation.Vertical;

	public string SpacingLabel =>
		Number(Spacing);

	public IReadOnlyList<Span> ConfigurationCode =>
		Code(
			$$"""
			StackPanel stack = new()
			{
				Orientation = Orientation.{{Orientation}},
				Spacing = {{Number(Spacing)}},
				Children =
				{
					new Label { Text = "One" },
					new Label { Text = "Two" },
					new Label { Text = "Three" }
				}
			};
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
