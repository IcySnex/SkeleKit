using SkeleKit.Gallery.ViewModels.Demos;
using SkeleKit.Gallery.Views;

namespace SkeleKit.Gallery.Views.Demos;

[Page]
public class SystemPickerDemo : ContentView<SystemPickerDemoViewModel>
{
	public SystemPickerDemo(
		SystemPickerDemoViewModel viewModel) : base(viewModel)
	{
		Title = "System Pickers";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "System photo & file pickers" },

					new Image
					{
						Source = Bind(vm => vm.Photo),
						Width = 240,
						Height = 240,
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center
					},

					new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Spacing = 12,
						HorizontalAlignment = HorizontalAlignment.Center,
						Children =
						{
							new Button { Text = "Pick Photo", Command = ViewModel.PickPhotoCommand },
							new Button { Text = "Pick File", Command = ViewModel.PickFileCommand }
						}
					},


					new Label { Text = Bind(vm => vm.Status) }
				}
			}
		};
	}
}
