using SkeleKit;
using SkeleKit.Gallery.Views;
using SkeleKit.Gallery.ViewModels.Demos;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="ProgressBar"/> at different progress values and with a custom tint.
/// </summary>
[Page]
public class ProgressBarDemo : ContentView<ProgressBarDemoViewModel>
{
	public ProgressBarDemo(
		ProgressBarDemoViewModel viewModel) : base(viewModel)
	{
		Title = "ProgressBar";

		Content =
			new ScrollView
			{
				Content = new StackPanel
				{
					Spacing = 20,
					Margin = new Thickness(16),
					Children =
					{
						new Label { Style = Styles.Caption, Text = "0% progress" },
						new ProgressBar { Progress = 0 },

						new Label { Style = Styles.Caption, Text = "30% progress" },
						new ProgressBar { Progress = 0.3 },

						new Label { Style = Styles.Caption, Text = "70% progress" },
						new ProgressBar { Progress = 0.7 },

						new Label { Style = Styles.Caption, Text = "100% progress" },
						new ProgressBar { Progress = 1 },

						new Label { Style = Styles.Caption, Text = "With tint" },
						new ProgressBar { Progress = 0.5, FillColor = Color.FromHex(0x34C759) }
					}
				}
			};
	}
}
