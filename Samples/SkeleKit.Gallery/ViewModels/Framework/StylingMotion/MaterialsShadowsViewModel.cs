using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.StylingMotion;

internal sealed partial class MaterialsShadowsViewModel : ShowcaseViewModel
{
	static readonly SurfaceOption[] Surfaces =
	[
		new(
			"Solid",
			Color.FromHex(0x356B74),
			"Color.FromHex(0x356B74)",
			Colors.White,
			"Colors.White"),
		new(
			"Gradient",
			LinearGradient.Vertical(
				Color.FromHex(0x315E68),
				Color.FromHex(0x748894)),
			"LinearGradient.Vertical(Color.FromHex(0x315E68), Color.FromHex(0x748894))",
			Colors.White,
			"Colors.White"),
		Material("Thin material", MaterialKind.Thin),
		Material("Regular material", MaterialKind.Regular),
		Material("Thick material", MaterialKind.Thick),
		Material("Glass material", MaterialKind.Glass)
	];

	static readonly Shadow?[] Depths =
	[
		null,
		new(opacity: 0.12, radius: 6, offsetY: 2),
		new(opacity: 0.22, radius: 12, offsetY: 6),
		new(opacity: 0.3, radius: 20, offsetY: 10)
	];

	public IReadOnlyList<SurfaceOption> SurfaceOptions =>
		Surfaces;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SurfaceBrush))]
	[NotifyPropertyChangedFor(nameof(SurfaceName))]
	[NotifyPropertyChangedFor(nameof(SurfaceTextColor))]
	[NotifyPropertyChangedFor(nameof(CompositionCode))]
	SurfaceOption? selectedSurface = Surfaces[3];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedShadow))]
	[NotifyPropertyChangedFor(nameof(CompositionCode))]
	int depthIndex = 2;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(CompositionCode))]
	bool clipsContent;

	SurfaceOption CurrentSurface =>
		SelectedSurface ?? Surfaces[0];

	internal Brush SurfaceBrush =>
		CurrentSurface.Brush;

	internal Color SurfaceTextColor =>
		CurrentSurface.TextColor;

	internal Shadow? SelectedShadow =>
		Depths[Math.Clamp(DepthIndex, 0, Depths.Length - 1)];

	public string SurfaceName =>
		CurrentSurface.Title;

	public IReadOnlyList<Span> CompositionCode =>
		Code(
			$$"""
			Border surface = new()
			{
				Width = 224,
				Height = 120,
				Background = {{CurrentSurface.Code}},
				CornerRadius = 22,
				Shadow = {{ShadowCode()}},
				Child = new Label
				{
					Text = "{{SurfaceName}}",
					TextColor = {{CurrentSurface.TextColorCode}}
				}
			};

			Overlay overflowHost = new()
			{
				Width = 224,
				Height = 120,
				ClipsToBounds = {{Bool(ClipsContent)}},
				Children =
				{
					new Border
					{
						Width = 70,
						Height = 28,
						HorizontalAlignment = HorizontalAlignment.End,
						VerticalAlignment = VerticalAlignment.Start,
						Translation = new(14, -10),
						Background = Colors.Cyan,
						CornerRadius = 14,
						Child = new Label { Text = "Outside" }
					}
				}
			};

			Overlay scene = new()
			{
				Children =
				{
					surface,
					overflowHost
				}
			};
			""");


	string ShadowCode() =>
		Math.Clamp(DepthIndex, 0, Depths.Length - 1) switch
		{
			0 => "null",
			1 => "new(opacity: 0.12, radius: 6, offsetY: 2)",
			2 => "new(opacity: 0.22, radius: 12, offsetY: 6)",
			_ => "new(opacity: 0.3, radius: 20, offsetY: 10)"
		};

	static SurfaceOption Material(
		string title,
		MaterialKind kind) =>
		new(
			title,
			new Material(kind),
			$"new Material(MaterialKind.{kind})",
			Colors.Label,
			"Colors.Label");

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Bool(
		bool value) =>
		value ? "true" : "false";
}

internal sealed record SurfaceOption(
	string Title,
	Brush Brush,
	string Code,
	Color TextColor,
	string TextColorCode);
