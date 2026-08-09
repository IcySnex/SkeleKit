using SkeleKit.Gallery.ViewModels.Framework.StylingMotion;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.StylingMotion;

[Page]
internal sealed class MaterialsShadowsView : ShowcaseView<MaterialsShadowsViewModel>
{
	static readonly Shadow SurfaceShadow = new(opacity: 0.24, radius: 14, offsetY: 8);


	public MaterialsShadowsView(
		MaterialsShadowsViewModel viewModel) : base(viewModel, "Materials & Shadows", Colors.Cyan)
	{
		AddCompositionShowcase(viewModel);
	}


	void AddCompositionShowcase(
		MaterialsShadowsViewModel viewModel)
	{
		Overlay overflowHost = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 224,
			Height = 120,
			ClipsToBounds = viewModel.ClipsContent,

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

		Border material = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 224,
			Height = 120,
			Background = viewModel.SelectedMaterial,
			CornerRadius = 22,
			Shadow = viewModel.CastsShadow ? SurfaceShadow : null,

			Child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = Bind(model => model.MaterialName),
				TextStyle = TextStyle.Title3,
				FontWeight = FontWeight.Semibold
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

		SegmentedControl kind = new()
		{
			SelectedIndex = Bind(
				model => model.MaterialIndex,
				static (model, value) => model.MaterialIndex = value),
			SelectionChanged = _ => material.Background = viewModel.SelectedMaterial
		};
		kind.Items.Add("Thin");
		kind.Items.Add("Regular");
		kind.Items.Add("Thick");
		kind.Items.Add("Glass");

		Switch shadow = new()
		{
			IsOn = Bind(
				model => model.CastsShadow,
				static (model, value) => model.CastsShadow = value),
			Toggled = enabled => material.Shadow = enabled ? SurfaceShadow : null
		};

		Switch clip = new()
		{
			IsOn = Bind(
				model => model.ClipsContent,
				static (model, value) => model.ClipsContent = value),
			Toggled = enabled => overflowHost.ClipsToBounds = enabled
		};

		AddShowcase(
			"Material, depth & overflow",
			"Change the blur material, shadow and clipping independently in one layered composition.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(scene, 252),
				LabeledControl("Material", kind),
				SettingRow("Shadow", shadow),
				SettingRow("Clip content", clip)),
			ShowcaseBox.Code(Bind(model => model.CompositionCode)));
	}


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

	static Border Tile(
		Color color) =>
		new()
		{
			Background = color,
			CornerRadius = 16
		};
}
