using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.MediaContent;

internal sealed partial class NativeViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	string selectionSummary = "Choose a date in the UIKit calendar.";

	public IReadOnlyList<Span> CalendarCode { get; } =
	[
		new(
			"""
			sealed class CalendarDelegate : UICalendarSelectionSingleDateDelegate
			{
				readonly Action<DateTime> selected;

				public CalendarDelegate(
					Action<DateTime> selected)
				{
					this.selected = selected;
				}

				public override void DidSelectDate(
					UICalendarSelectionSingleDate selection,
					NSDateComponents? date)
				{
					if (date is not null)
						selected(new((int)date.Year, (int)date.Month, (int)date.Day));
				}
			}

			CalendarDelegate selectionDelegate = new(viewModel.SelectDate);
			UICalendarSelectionSingleDate selection = new(selectionDelegate);

			UICalendarView calendar = new()
			{
				SelectionBehavior = selection
			};

			new NativeView(calendar)
			{
				Height = 360
			};
			""")
	];


	internal void SelectDate(
		DateTime date) =>
		SelectionSummary = date.ToString("D");
}
