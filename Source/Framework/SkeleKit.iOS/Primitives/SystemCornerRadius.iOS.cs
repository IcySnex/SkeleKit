using CoreGraphics;
using Foundation;

namespace SkeleKit;

public static partial class SystemCornerRadius
{
	static double? groupedList;


	static partial void ResolveGroupedList(
		ref double radius)
	{
		if (groupedList is double resolved)
		{
			radius = resolved;
			return;
		}

		using GroupedListSource source = new();
		using UITableView table = new(new CGRect(0, 0, 320, 100), UITableViewStyle.InsetGrouped)
		{
			Source = source
		};

		table.ReloadData();
		table.LayoutIfNeeded();

		UITableViewCell? cell = table.CellAt(NSIndexPath.FromRowSection(0, 0));
		if (cell is null)
			return;

		resolved = OperatingSystem.IsIOSVersionAtLeast(26)
			? cell.SetEffectiveRadius(UIRectCorner.AllCorners)
			: cell.Layer.CornerRadius;

		if (resolved <= 0)
			return;

		groupedList = resolved;
		radius = resolved;
	}


	sealed class GroupedListSource : UITableViewSource
	{
		const string ReuseIdentifier = "SkeleKit.SystemCornerRadius.GroupedList";


		public override nint RowsInSection(
			UITableView tableView,
			nint section) => 1;


		public override UITableViewCell GetCell(
			UITableView tableView,
			NSIndexPath indexPath) =>
			tableView.DequeueReusableCell(ReuseIdentifier)
			?? new UITableViewCell(UITableViewCellStyle.Default, ReuseIdentifier);
	}
}
