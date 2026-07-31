using CommunityToolkit.Mvvm.Input;
using Foundation;
using ObjCRuntime;
using SkeleKit.Gallery.ViewModels.Controls.MediaContent;
using SkeleKit.Gallery.Views.Showcase;
using UIKit;

namespace SkeleKit.Gallery.Views.Controls.MediaContent;

[Page]
internal sealed class NativeView : ShowcaseView<NativeViewModel>
{
	sealed class CalendarDelegate : UICalendarSelectionSingleDateDelegate
	{
		readonly Action<DateTime>? selected;

		public CalendarDelegate(
			Action<DateTime> selected)
		{
			this.selected = selected;
		}

		// ReSharper disable once UnusedMember.Local
		public CalendarDelegate(
			NativeHandle handle) : base(handle)
		{ }


		public override void DidSelectDate(
			UICalendarSelectionSingleDate selection,
			NSDateComponents? dateComponents)
		{
			if (dateComponents is null)
				return;

			selected?.Invoke(new(
				(int)dateComponents.Year,
				(int)dateComponents.Month,
				(int)dateComponents.Day));
		}
	}


	readonly CalendarDelegate calendarDelegate;
	readonly UICalendarSelectionSingleDate calendarSelection;


	public NativeView(
		NativeViewModel viewModel) : base(viewModel, "Native View", Colors.Orange)
	{
		calendarDelegate = new(viewModel.SelectDate);
		calendarSelection = new(calendarDelegate);

		AddCalendarShowcase(viewModel);
	}


	void AddCalendarShowcase(
		NativeViewModel viewModel)
	{
		UICalendarView calendar = new()
		{
			SelectionBehavior = calendarSelection
		};

		DateTime today = DateTime.Today;
		NSDateComponents todayComponents = Components(today);

		Button selectToday = new()
		{
			Text = "Select today",
			Icon = "calendar.badge.checkmark",
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Small,
			Command = new RelayCommand(() =>
			{
				calendar.SetVisibleDateComponents(todayComponents, animated: true);
				calendarSelection.SetSelectedDate(todayComponents, animated: true);
				viewModel.SelectDate(today);
			})
		};

		AddShowcase(
			"UIKit calendar",
			"Host an unsupported UIKit control and bridge its native selection delegate into the ViewModel.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 8,

						Children =
						{
							new SkeleKit.NativeView(calendar)
							{
								HorizontalAlignment = HorizontalAlignment.Stretch,
								Height = 340
							},

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.SelectionSummary),
								TextStyle = TextStyle.Caption1,
								TextColor = Colors.SecondaryLabel,
								TextAlignment = TextAlignment.Center
							}
						}
					},
					420),
				SettingRow("Selection", selectToday)),
			ShowcaseBox.Code(Bind(model => model.CalendarCode)));
	}


	static NSDateComponents Components(
		DateTime date) =>
		new()
		{
			Year = date.Year,
			Month = date.Month,
			Day = date.Day
		};
}
