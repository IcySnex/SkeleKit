using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.MediaContent;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.MediaContent;

[Page]
internal sealed class ImageView : ShowcaseView<ImageViewModel>
{
	public ImageView(
		ImageViewModel viewModel) : base(viewModel, "Image", Colors.Orange)
	{
		AddSourceShowcase(viewModel);
		AddRenderingShowcase(viewModel);
		AddEffectsShowcase(viewModel);
	}


	void AddSourceShowcase(
		ImageViewModel viewModel)
	{
		Image image = new()
		{
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Width = 280,
			Height = 180,
			Source = Bind(vm => vm.RemoteSource),
			Placeholder = ImageSource.Symbol("photo"),
			Fallback = ImageSource.Symbol("exclamationmark.triangle.fill"),
			FadesIn = true,
			Stretch = viewModel.SelectedStretch.Value,
			Background = Colors.SecondaryBackground,
			CornerRadius = 18
		};

		Picker<ShowcaseOption<string>> source = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Sources,
			SelectedItem = Bind(vm => vm.SelectedSource)
				.TwoWay((vm, val) => vm.SelectedSource = val!)
		};

		Picker<ShowcaseOption<Stretch>> stretch = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Stretches,
			SelectedItem = Bind(vm => vm.SelectedStretch)
				.TwoWay((vm, val) => vm.SelectedStretch = val!),
			SelectionChanged = option => image.Stretch = option.Value
		};

		AddShowcase(
			"Source & layout",
			"Load a remote image with placeholder, fallback, cross-dissolve, and native content modes.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(image, 230),
				SettingRow("Source", source),
				SettingRow("Stretch", stretch)),
			Code(vm => vm.SourceCode));
	}

	void AddRenderingShowcase(
		ImageViewModel viewModel)
	{
		Image symbol = new()
		{
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Width = 120,
			Height = 110,
			Source = ImageSource.Symbol("cloud.sun.rain.fill"),
			SymbolSize = viewModel.SymbolSize,
			SymbolWeight = viewModel.SelectedWeight.Value,
			SymbolScale = viewModel.SelectedScale.Value,
			PrefersMulticolor = viewModel.PrefersMulticolor
		};

		Slider size = new()
		{
			MinWidth = 150,
			Value = Bind(vm => vm.SymbolSize)
				.TwoWay((vm, val) => vm.SymbolSize = val),
			Minimum = 32,
			Maximum = 96,
			ValueChanged = value => symbol.SymbolSize = value
		};

		Picker<ShowcaseOption<FontWeight>> weight = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Weights,
			SelectedItem = Bind(vm => vm.SelectedWeight)
				.TwoWay((vm, val) => vm.SelectedWeight = val!),
			SelectionChanged = option => symbol.SymbolWeight = option.Value
		};

		Picker<ShowcaseOption<SymbolScale>> scale = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Scales,
			SelectedItem = Bind(vm => vm.SelectedScale)
				.TwoWay((vm, val) => vm.SelectedScale = val!),
			SelectionChanged = option => symbol.SymbolScale = option.Value
		};

		Switch multicolor = new()
		{
			IsOn = Bind(vm => vm.PrefersMulticolor)
				.TwoWay((vm, val) => vm.PrefersMulticolor = val),
			Toggled = value =>
			{
				symbol.PrefersMulticolor = value;
			}
		};

		AddShowcase(
			"Symbol rendering",
			"Configure SF Symbol size, weight, relative scale, palette, and multicolor rendering.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(symbol, 170),
				SettingRow("Point size", size),
				SettingRow("Weight", weight),
				SettingRow("Scale", scale),
				SettingRow("Multicolor", multicolor)),
			Code(vm => vm.RenderingCode));
	}

	void AddEffectsShowcase(
		ImageViewModel viewModel)
	{
		Image symbol = new()
		{
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Width = 120,
			Height = 110,
			Source = ImageSource.Symbol("speaker.wave.3.fill"),
			SymbolSize = 72,
			SymbolValue = Bind(vm => vm.SymbolValue),
			SymbolEffect = viewModel.SelectedEffect.Value
		};

		Slider value = new()
		{
			MinWidth = 150,
			Value = Bind(vm => vm.SymbolValue)
				.TwoWay((vm, val) => vm.SymbolValue = val),
			Minimum = 0,
			Maximum = 1
		};

		Picker<ShowcaseOption<SymbolEffect>> effect = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Effects,
			SelectedItem = Bind(vm => vm.SelectedEffect)
				.TwoWay((vm, val) => vm.SelectedEffect = val!),
			SelectionChanged = option => symbol.SymbolEffect = option.Value
		};

		AddShowcase(
			"Variable symbols & effects",
			"Drive a variable symbol, run an ambient effect, and trigger a one-shot effect.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(symbol, 170),
				SettingRow("Symbol value", value),
				SettingRow("Ambient effect", effect),
				SettingRow(
					"One-shot effect",
					new Button
					{
						Text = "Bounce",
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Small,
						Command = new RelayCommand(() => symbol.PlaySymbolEffect(SymbolEffect.Bounce))
					})),
			Code(vm => vm.EffectsCode));
	}
}
