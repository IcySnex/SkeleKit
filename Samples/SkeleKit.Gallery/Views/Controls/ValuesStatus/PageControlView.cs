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
			Count = Bind(model => model.Count),
			Current = Bind(
				model => model.Current,
				static (model, value) => model.Current = value),
			DotColor = Colors.Red.WithAlpha(0.25),
			CurrentDotColor = Colors.Red,
			HidesForSinglePage = viewModel.HidesForSinglePage,
			AllowsScrubbing = viewModel.AllowsScrubbing,
			PageChanged = viewModel.RecordPage
		};

		Picker<ShowcaseOption<int>> count = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.Counts,
			SelectedItem = viewModel.SelectedCount,
			SelectionChanged = viewModel.SelectCount
		};

		Switch hiding = new()
		{
			IsOn = viewModel.HidesForSinglePage,
			Toggled = value =>
			{
				viewModel.HidesForSinglePage = value;
				pages.HidesForSinglePage = value;
			}
		};

		Switch scrubbing = new()
		{
			IsOn = viewModel.AllowsScrubbing,
			Toggled = value =>
			{
				viewModel.AllowsScrubbing = value;
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
							Status(Bind(model => model.StateLabel), FontWeight.Medium),
							Status(Bind(model => model.ChangeStatus))
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
			ShowcaseBox.Code(Bind(model => model.PageControlCode)));
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
