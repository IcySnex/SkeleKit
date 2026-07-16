using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A tint set on one panel colors every control under it: UIKit inherits the tint down the view tree.
/// </summary>
[Page]
public class TintDemo : ContentView<TintDemoViewModel>
{
	public TintDemo(
		TintDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Tint";

		StackPanel tinted = new()
		{
			Spacing = 20,
			Tint = viewModel.Accent,
			Children =
			{
				new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Spacing = 16,
					Children =
					{
						new Image { Source = ImageSource.Symbol("paintpalette.fill"), Width = 32, Height = 32 },
						new Image { Source = ImageSource.Symbol("bolt.fill"), Width = 32, Height = 32 },
						new Image { Source = ImageSource.Symbol("heart.fill"), Width = 32, Height = 32 },
						new ActivityIndicator { IsAnimating = true }
					}
				},

				new Switch { IsOn = true, HorizontalAlignment = HorizontalAlignment.Start },
				new Slider { Minimum = 0, Maximum = 1, Value = 0.6 },
				new ProgressBar { Progress = 0.4 },
				new Stepper { Value = 2, HorizontalAlignment = HorizontalAlignment.Start },

				new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Spacing = 12,
					Children =
					{
						new Button { Text = "Plain", Kind = ButtonStyle.Plain },
						new Button { Text = "Tinted", Kind = ButtonStyle.Tinted },
						new Button { Text = "Filled", Kind = ButtonStyle.Filled }
					}
				}
			}
		};

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 24,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Pick an accent — everything below follows it" },

					new ColorWell
					{
						Title = "Accent",
						Selected = Bind(vm => vm.Accent, (vm, value) => vm.Accent = value),
						SelectionChanged = color => tinted.Tint = color,
						HorizontalAlignment = HorizontalAlignment.Start
					},

					tinted,

					new Label
					{
						Style = Styles.Caption,
						Text = "Outside the tinted panel, controls keep the app accent."
					},
					new Button { Text = "Untinted", Kind = ButtonStyle.Tinted }
				}
			}
		};
	}
}
