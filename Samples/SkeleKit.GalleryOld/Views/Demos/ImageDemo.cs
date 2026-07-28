using SkeleKit;
using SkeleKit.Gallery.Views;
using SkeleKit.Gallery.ViewModels.Demos;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Image"/>: symbol styling, effects, tinting, and URL loading UX.
/// </summary>
[Page(Singleton = true)]
public class ImageDemo : ContentView<ImageDemoViewModel>
{
	public ImageDemo(
		ImageDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Image";

		Image heart = new()
		{
			Source = ImageSource.Symbol("heart.fill"),
			SymbolSize = 44,
			Tint = Colors.Pink
		};

		Content =
			new ScrollView
			{
				Content = new StackPanel
				{
					Spacing = 20,
					Margin = new Thickness(16),
					Children =
					{
						new Label { Style = Styles.Caption, Text = "Symbol styling: plain, hierarchical, palette, multicolor, black weight" },
						new StackPanel
						{
							Orientation = Orientation.Horizontal,
							Spacing = 20,
							Children =
							{
								new Image
								{
									Source = ImageSource.Symbol("square.stack.3d.up.fill"),
									SymbolSize = 44
								},
								new Image
								{
									Source = ImageSource.Symbol("square.stack.3d.up.fill"),
									SymbolSize = 44,
									SymbolColors = { Colors.Indigo }
								},
								new Image
								{
									Source = ImageSource.Symbol("folder.badge.plus"),
									SymbolSize = 44,
									SymbolColors = { Colors.Orange, Colors.Teal }
								},
								new Image
								{
									Source = ImageSource.Symbol("cloud.sun.rain.fill"),
									SymbolSize = 44,
									PrefersMulticolor = true
								},
								new Image
								{
									Source = ImageSource.Symbol("star"),
									SymbolSize = 44,
									SymbolWeight = FontWeight.Black
								}
							}
						},

						new Label { Style = Styles.Caption, Text = "Variable value: drag the slider" },
						new StackPanel
						{
							Orientation = Orientation.Horizontal,
							Spacing = 20,
							Children =
							{
								new Image
								{
									Source = ImageSource.Symbol("speaker.wave.3.fill"),
									SymbolSize = 44,
									SymbolValue = Bind(vm => vm.Level)
								},
								new Image
								{
									Source = ImageSource.Symbol("wifi"),
									SymbolSize = 44,
									SymbolValue = Bind(vm => vm.Level)
								},
								new Slider
								{
									Width = 180,
									VerticalAlignment = VerticalAlignment.Center,
									Value = Bind(vm => vm.Level, (vm, value) => vm.Level = value)
								}
							}
						},

						new Label { Style = Styles.Caption, Text = "Effects: ambient variable-color / wiggle, bounce on tap" },
						new StackPanel
						{
							Orientation = Orientation.Horizontal,
							Spacing = 20,
							Children =
							{
								new Image
								{
									Source = ImageSource.Symbol("antenna.radiowaves.left.and.right"),
									SymbolSize = 44,
									SymbolEffect = SymbolEffect.VariableColor
								},
								new Image
								{
									Source = ImageSource.Symbol("bell.fill"),
									SymbolSize = 44,
									SymbolEffect = SymbolEffect.Wiggle
								},
								heart,
								new Button
								{
									Text = "Bounce",
									VerticalAlignment = VerticalAlignment.Center,
									Command = Command.From(() => heart.PlaySymbolEffect(SymbolEffect.Bounce))
								}
							}
						},

						new Label { Style = Styles.Caption, Text = "URL with placeholder + fade-in" },
						new Image
						{
							Source = ImageSource.Url("https://picsum.photos/300/200"),
							Placeholder = ImageSource.Symbol("photo"),
							FadesIn = true,
							Width = 300,
							Height = 200,
							Stretch = Stretch.UniformToFill
						},

						new Label { Style = Styles.Caption, Text = "Broken URL: falls back to an error image" },
						new Image
						{
							Source = ImageSource.Url("https://invalid.skelekit.example/missing.png"),
							Placeholder = ImageSource.Symbol("photo"),
							Fallback = ImageSource.Symbol("wifi.slash"),
							FadesIn = true,
							Width = 300,
							Height = 80
						},

						new Label { Style = Styles.Caption, Text = "Tinted URL image: the raster renders as a template" },
						new Image
						{
							Source = ImageSource.Url("https://picsum.photos/120/120"),
							Tint = Colors.Indigo,
							Width = 120,
							Height = 120
						},

						new Label { Style = Styles.Caption, Text = "Long-press the poster: a context menu on a plain view" },
						new Image
						{
							Source = ImageSource.Url("https://picsum.photos/id/1043/240/360"),
							CornerRadius = 12,
							Width = 160,
							Height = 240,
							Stretch = Stretch.UniformToFill,
							HorizontalAlignment = HorizontalAlignment.Start,
							ContextMenu =
							{
								new() { Text = "Copy link", Icon = "link", Command = ViewModel.CopyLinkCommand },
								new() { Text = "Save", Icon = "square.and.arrow.down", Command = ViewModel.SaveCommand },
								new() { Text = "Remove", Icon = "trash", IsDestructive = true, Command = ViewModel.RemoveCommand }
							}
						}
					}
				}
			};
	}
}
