using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="ProgressBar"/> at different progress values and with a custom tint.
/// </summary>
public static class ProgressBarPage
{
	static readonly Color Secondary = Color.FromHex(0x8E8E93);

	public static View Build() =>
		new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Text = "0% progress", FontSize = 13, TextColor = Secondary },
					new ProgressBar { Progress = 0 },

					new Label { Text = "30% progress", FontSize = 13, TextColor = Secondary },
					new ProgressBar { Progress = 0.3 },

					new Label { Text = "70% progress", FontSize = 13, TextColor = Secondary },
					new ProgressBar { Progress = 0.7 },

					new Label { Text = "100% progress", FontSize = 13, TextColor = Secondary },
					new ProgressBar { Progress = 1 },

					new Label { Text = "With tint", FontSize = 13, TextColor = Secondary },
					new ProgressBar { Progress = 0.5, Tint = Color.FromHex(0x34C759) }
				}
			}
		};
}
