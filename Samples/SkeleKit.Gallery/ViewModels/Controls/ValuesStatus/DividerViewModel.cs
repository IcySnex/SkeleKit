using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class DividerViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(DividerColor))]
	[NotifyPropertyChangedFor(nameof(ColorLabel))]
	bool usesAccent;

	public Color? DividerColor =>
		UsesAccent ? Colors.Red : null;

	public string ColorLabel =>
		UsesAccent ? "Accent color" : "System separator";

	public IReadOnlyList<Span> DividerCode =>
	[
		new(
			"""
			new Divider
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Color = Bind(model => model.DividerColor)
			};
			""")
	];
}
