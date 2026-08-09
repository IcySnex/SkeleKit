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
					Item("One"),
					Item("Two"),
					Item("Three")
				}
			};

			static Border Item(string text) =>
				new()
				{
					Width = 72,
					Height = 56,
					Background = Colors.Blue,
					CornerRadius = 12,
					Child = new Label
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = text,
						TextStyle = TextStyle.Subheadline,
						FontWeight = FontWeight.Semibold,
						TextColor = Colors.White
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
