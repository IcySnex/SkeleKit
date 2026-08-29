using SkeleKit.Gallery.ViewModels.Framework.StylingMotion;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.StylingMotion;

[Page]
internal sealed class MaterialsShadowsView : ShowcaseView<MaterialsShadowsViewModel>
{
	public MaterialsShadowsView(
		MaterialsShadowsViewModel viewModel) : base(viewModel, "Surfaces & Shadows", Colors.Cyan)
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

		Label surfaceLabel = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Text = Bind(vm => vm.SurfaceName),
			TextStyle = TextStyle.Title3,
			FontWeight = FontWeight.Semibold,
			TextColor = viewModel.SurfaceTextColor
		};

		Border surface = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 224,
			Height = 120,
			Background = viewModel.SurfaceBrush,
			CornerRadius = 22,
			Shadow = viewModel.SelectedShadow,
			Child = surfaceLabel
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
				surface,
				overflowHost
			}
		};

		Picker<SurfaceOption> surfacePicker = new()
		{
			MinWidth = 180,
			ItemsSource = Bind(vm => vm.SurfaceOptions),
			SelectedItem = Bind(vm => vm.SelectedSurface)
				.TwoWay((vm, val) => vm.SelectedSurface = val),
			ItemTitle = option => option.Title,
			SelectionChanged = option =>
			{
				surface.Background = option.Brush;
				surfaceLabel.TextColor = option.TextColor;
			}
		};

		SegmentedControl depth = new()
		{
			SelectedIndex = Bind(vm => vm.DepthIndex)
				.TwoWay((vm, val) => vm.DepthIndex = val),
			SelectionChanged = _ => surface.Shadow = viewModel.SelectedShadow
		};
		depth.Items.Add("None");
		depth.Items.Add("Low");
		depth.Items.Add("Medium");
		depth.Items.Add("High");

		Switch clip = new()
		{
			IsOn = Bind(vm => vm.ClipsContent)
				.TwoWay((vm, val) => vm.ClipsContent = val),
			Toggled = enabled => overflowHost.ClipsToBounds = enabled
		};

		AddShowcase(
			"Surface, depth & clipping",
			"Compare solid, gradient and native material brushes, then adjust elevation and overflow.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(scene, 252),
				SettingRow("Surface", surfacePicker),
				LabeledControl("Depth", depth),
				SettingRow("Clip content", clip)),
			Code(vm => vm.CompositionCode));
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
