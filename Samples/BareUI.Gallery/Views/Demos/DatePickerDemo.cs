using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="DatePicker"/> modes and styles, bound two-way.
/// </summary>
public class DatePickerDemo : ContentView<DatePickerDemoViewModel>
{
	public DatePickerDemo(
		DatePickerDemoViewModel viewModel) : base(viewModel)
	{
		Title = "DatePicker";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Compact — two-way, capped at today" },
					new DatePicker
					{
						Maximum = DateTime.Now,
						Date = Bind(vm => vm.Birthday, (vm, value) => vm.Birthday = value),
						HorizontalAlignment = HorizontalAlignment.Start
					},
					new Label
					{
						Text = Bind(vm => vm.Birthday, value => $"Picked: {value:d}"),
						TextColor = Palette.Secondary
					},

					new Label { Style = Styles.Caption, Text = "Time, wheels" },
					new DatePicker
					{
						Mode = DatePickerMode.Time,
						Style = DatePickerStyle.Wheels
					},

					new Label { Style = Styles.Caption, Text = "Inline calendar" },
					new DatePicker
					{
						Style = DatePickerStyle.Inline,
						Date = Bind(vm => vm.Birthday, (vm, value) => vm.Birthday = value)
					}
				}
			}
		};
	}
}
