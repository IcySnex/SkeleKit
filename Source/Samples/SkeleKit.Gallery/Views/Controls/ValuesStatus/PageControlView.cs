using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ValuesStatus;

[Page]
internal sealed class PageControlView : ShowcaseView<PageControlViewModel>
{
	public PageControlView(
		PageControlViewModel viewModel) : base(viewModel, "Page Control", Colors.Red)
	{
		AddPageShowcase(viewModel);
	}


	void AddPageShowcase(
		PageControlViewModel viewModel)
	{
		PageControl pages = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Count = Bind(vm => vm.Count),
			Current = Bind(vm => vm.Current)
				.TwoWay((vm, val) => vm.Current = val),
			DotColor = Colors.Red.WithAlpha(0.25),
			CurrentDotColor = Colors.Red,
			HidesForSinglePage = viewModel.HidesForSinglePage,
			AllowsScrubbing = viewModel.AllowsScrubbing
		};

		Picker<ShowcaseOption<int>> count = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.Counts,
			SelectedItem = Bind(vm => vm.SelectedCount)
				.TwoWay((vm, val) => vm.SelectedCount = val!)
		};

		Switch hiding = new()
		{
			IsOn = Bind(vm => vm.HidesForSinglePage)
				.TwoWay((vm, val) => vm.HidesForSinglePage = val),
			Toggled = value =>
			{
				pages.HidesForSinglePage = value;
			}
		};

		Switch scrubbing = new()
		{
			IsOn = Bind(vm => vm.AllowsScrubbing)
				.TwoWay((vm, val) => vm.AllowsScrubbing = val),
			Toggled = value =>
			{
				pages.AllowsScrubbing = value;
			}
		};

		AddShowcase(
			"Pages & interaction",
			"Change the page count, tap or scrub the dots, and compare single-page visibility.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 8,

						Children =
						{
							pages,
							Status(Bind(vm => vm.StateLabel), FontWeight.Medium)
						}
					},
					180),
				SettingRow("Pages", count),
				SettingRow(
					"Current page",
					new Button
					{
						Text = "Advance",
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Small,
						Command = viewModel.AdvancePageCommand
					}),
				SettingRow("Hide single page", hiding),
				SettingRow("Scrubbing", scrubbing)),
			Code(vm => vm.PageControlCode));
	}


	static Label Status(
		BindingExpression<string?> text,
		FontWeight weight = FontWeight.Regular) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = text,
			TextStyle = TextStyle.Footnote,
			FontWeight = weight,
			TextColor = weight is FontWeight.Regular ? Colors.SecondaryLabel : (Color?)null,
			MaxLines = 2,
			TextAlignment = TextAlignment.Center
		};
}
