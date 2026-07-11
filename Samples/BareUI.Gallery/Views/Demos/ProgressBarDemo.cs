using BareUI;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="ProgressBar"/> at different progress values and with a custom tint.
/// </summary>
public static class ProgressBarDemo
{
	public static View Build() =>
		new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					Theme.Caption("0% progress"),
					new ProgressBar { Progress = 0 },

					Theme.Caption("30% progress"),
					new ProgressBar { Progress = 0.3 },

					Theme.Caption("70% progress"),
					new ProgressBar { Progress = 0.7 },

					Theme.Caption("100% progress"),
					new ProgressBar { Progress = 1 },

					Theme.Caption("With tint"),
					new ProgressBar { Progress = 0.5, Tint = Color.FromHex(0x34C759) }
				}
			}
		};
}
