using Microsoft.Extensions.DependencyInjection;
using SkeleKit.Gallery.ViewModels.Platform;

namespace SkeleKit.Gallery.Views;

public sealed class GalleryTabAccessory : Overlay
{
	public GalleryTabAccessory()
	{
		TabsIpadViewModel viewModel = SkeleApplication.Current!.Services.GetRequiredService<TabsIpadViewModel>();
		BindingContext = viewModel;
		IsVisible = BindingFactory.Bind((TabsIpadViewModel model) => model.AccessoryVisible);

		Children.Add(new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Spacing = 11,
			Margin = new(14, 9),
			HorizontalAlignment = HorizontalAlignment.Start,
			VerticalAlignment = VerticalAlignment.Center,

			Children =
			{
				new Image
				{
					Source = ImageSource.Symbol("waveform"),
					SymbolSize = 22,
					Width = 30,
					Height = 30,
					Tint = Colors.Green,
					VerticalAlignment = VerticalAlignment.Center
				},

				new StackPanel
				{
					Spacing = 1,
					VerticalAlignment = VerticalAlignment.Center,

					Children =
					{
						new Label
						{
							Text = "SkeleKit",
							TextStyle = TextStyle.Subheadline,
							FontWeight = FontWeight.Semibold
						},

						new Label
						{
							Text = "Native tab accessory",
							TextStyle = TextStyle.Caption1,
							TextColor = Colors.SecondaryLabel
						}
					}
				}
			}
		});

		Children.Add(new Button
		{
			Icon = BindingFactory.Bind(
				(TabsIpadViewModel model) => model.PlayerIcon,
				static icon => (ImageSource?)ImageSource.Symbol(icon)),
			Kind = ButtonStyle.Plain,
			Size = ButtonSize.Small,
			Margin = new(0, 0, 12, 0),
			HorizontalAlignment = HorizontalAlignment.End,
			VerticalAlignment = VerticalAlignment.Center,
			Command = viewModel.TogglePlaybackCommand
		});
	}
}
