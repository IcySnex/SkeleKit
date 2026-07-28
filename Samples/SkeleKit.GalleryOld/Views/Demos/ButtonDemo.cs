using System.Windows.Input;
using SkeleKit.Gallery.ViewModels.Demos;
using SkeleKit.Gallery.Views;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Button"/> styles, an icon, and a bound command.
/// </summary>
[Page]
public class ButtonDemo : ContentView<ButtonDemoViewModel>
{
	public ButtonDemo(
		ButtonDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Button";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Kinds" },
					new Button { Text = "Plain", Kind = ButtonStyle.Plain },
					new Button { Text = "Gray", Kind = ButtonStyle.Gray },
					new Button { Text = "Tinted", Kind = ButtonStyle.Tinted },
					new Button { Text = "Filled", Kind = ButtonStyle.Filled },
					new Button { Text = "FilledCapsule", Kind = ButtonStyle.FilledCapsule },

					new Label { Style = Styles.Caption, Text = "With an SF Symbol" },
					new Button { Text = "Play", Icon = "play.fill", Kind = ButtonStyle.Filled },
					new Button { Text = "Next", Icon = "chevron.right", IconPlacement = IconPlacement.Trailing },

					new Label { Style = Styles.Caption, Text = "Subtitle, size, destructive, loading" },
					new Button { Text = "Buy now", Subtitle = "Free shipping", Kind = ButtonStyle.Filled, Size = ButtonSize.Large },
					new Button { Text = "Delete", Icon = "trash" , IsDestructive = true, Kind = ButtonStyle.Tinted },
					new Button { Text = "Loading", IsLoading = true, Kind = ButtonStyle.Gray },

					new Label { Style = Styles.Caption, Text = "Menu button — tap opens a pull-down" },
					new Button
					{
						Text = "Sort by",
						Icon = "arrow.up.arrow.down",
						Kind = ButtonStyle.Gray,
						Menu =
						{
							new MenuAction { Text = "Name", Icon = "textformat" },
							new MenuAction { Text = "Date", Icon = "calendar" },
							new MenuAction { Text = "Reset", Icon = "arrow.counterclockwise", IsDestructive = true }
						}
					},

					new Label { Style = Styles.Caption, Text = "Picker button — the choice becomes the title" },
					new Button
					{
						Kind = ButtonStyle.Gray,
						SelectsFromMenu = true,
						Menu =
						{
							new MenuAction { Text = "Small" },
							new MenuAction { Text = "Medium" },
							new MenuAction { Text = "Large" }
						}
					},

					new Label { Style = Styles.Caption, Text = "Bound command" },
					new Button
					{
						Text = "Tap me",
						Kind = ButtonStyle.Filled,
						Command = ViewModel.TapCommand
					},
					new Label { Text = Bind(vm => vm.Status), TextColor = Palette.Secondary }
				}
			}
		};
	}
}
