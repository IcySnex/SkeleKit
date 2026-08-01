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
			Source = Bind(model => model.RemoteSource),
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
			SelectedItem = Bind(
				model => model.SelectedSource,
				static (model, value) => model.SelectedSource = value!)
		};

		Picker<ShowcaseOption<Stretch>> stretch = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Stretches,
			SelectedItem = Bind(
				model => model.SelectedStretch,
				static (model, value) => model.SelectedStretch = value!),
			SelectionChanged = option => image.Stretch = option.Value
		};

		AddShowcase(
			"Source & layout",
			"Load a remote image with placeholder, fallback, cross-dissolve, and native content modes.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(image, 230),
				SettingRow("Source", source),
				SettingRow("Stretch", stretch)),
			ShowcaseBox.Code(Bind(model => model.SourceCode)));
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
			Value = Bind(
				model => model.SymbolSize,
				static (model, value) => model.SymbolSize = value),
			Minimum = 32,
			Maximum = 96,
			ValueChanged = value => symbol.SymbolSize = value
		};

		Picker<ShowcaseOption<FontWeight>> weight = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Weights,
			SelectedItem = Bind(
				model => model.SelectedWeight,
				static (model, value) => model.SelectedWeight = value!),
			SelectionChanged = option => symbol.SymbolWeight = option.Value
		};

		Picker<ShowcaseOption<SymbolScale>> scale = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Scales,
			SelectedItem = Bind(
				model => model.SelectedScale,
				static (model, value) => model.SelectedScale = value!),
			SelectionChanged = option => symbol.SymbolScale = option.Value
		};

		Switch multicolor = new()
		{
			IsOn = Bind(
				model => model.PrefersMulticolor,
				static (model, value) => model.PrefersMulticolor = value),
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
			ShowcaseBox.Code(Bind(model => model.RenderingCode)));
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
			SymbolValue = Bind(model => model.SymbolValue),
			SymbolEffect = viewModel.SelectedEffect.Value
		};

		Slider value = new()
		{
			MinWidth = 150,
			Value = Bind(
				model => model.SymbolValue,
				static (model, value) => model.SymbolValue = value),
			Minimum = 0,
			Maximum = 1
		};

		Picker<ShowcaseOption<SymbolEffect>> effect = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.Effects,
			SelectedItem = Bind(
				model => model.SelectedEffect,
				static (model, value) => model.SelectedEffect = value!),
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
			ShowcaseBox.Code(Bind(model => model.EffectsCode)));
	}
}
