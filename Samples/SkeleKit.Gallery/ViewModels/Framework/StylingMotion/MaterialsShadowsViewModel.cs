using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.StylingMotion;

internal sealed partial class MaterialsShadowsViewModel : ShowcaseViewModel
{
	static readonly MaterialKind[] Materials =
	[
		MaterialKind.Thin,
		MaterialKind.Regular,
		MaterialKind.Thick,
		MaterialKind.Glass
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedMaterial))]
	[NotifyPropertyChangedFor(nameof(MaterialName))]
	[NotifyPropertyChangedFor(nameof(CompositionCode))]
	int materialIndex = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(CompositionCode))]
	bool castsShadow = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(CompositionCode))]
	bool clipsContent;

	internal Material SelectedMaterial =>
		new(Materials[Math.Clamp(MaterialIndex, 0, Materials.Length - 1)]);

	public string MaterialName =>
		Materials[Math.Clamp(MaterialIndex, 0, Materials.Length - 1)].ToString();

	public IReadOnlyList<Span> CompositionCode =>
		Code(
			$$"""
			Border material = new()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Width = 224,
				Height = 120,
				Background = new Material(MaterialKind.{{MaterialName}}),
				CornerRadius = 22,
				Shadow = {{(CastsShadow ? "new(opacity: 0.24, radius: 14, offsetY: 8)" : "null")}},

				Child = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Text = "{{MaterialName}}",
					TextStyle = TextStyle.Title3,
					FontWeight = FontWeight.Semibold
				}
			};

			Overlay overflowHost = new()
			{
				Width = 224,
				Height = 120,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
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

						Child = new Label
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							Text = "Outside",
							TextStyle = TextStyle.Caption1,
							FontWeight = FontWeight.Semibold,
							TextColor = Colors.White
						}
					}
				}
			};

			Overlay scene = new()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Width = 300,
				Height = 210,
				CornerRadius = 24,
				ClipsToBounds = true,

				Children =
				{
					Backdrop(),
					material,
					overflowHost
				}
			};

			static Grid Backdrop() =>
				new()
				{
					Padding = 14,
					ColumnSpacing = 10,
					RowSpacing = 10,
					Background = Colors.SecondaryBackground,
					Columns =
					{
						GridLength.Star,
						GridLength.Star
					},
					Rows =
					{
						GridLength.Star,
						GridLength.Star
					},

					Children =
					{
						Tile(Colors.Cyan.WithAlpha(0.28)).Row(0).Column(0),
						Tile(Colors.Blue.WithAlpha(0.22)).Row(0).Column(1),
						Tile(Colors.Teal.WithAlpha(0.2)).Row(1).Column(0),
						Tile(Colors.Indigo.WithAlpha(0.16)).Row(1).Column(1)
					}
				};

			static Border Tile(Color color) =>
				new()
				{
					Background = color,
					CornerRadius = 16
				};
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Bool(
		bool value) =>
		value ? "true" : "false";
}
