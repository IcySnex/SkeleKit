using CoreGraphics;
using Foundation;

namespace SkeleKit;

internal static class SystemListMetrics
{
	static readonly Dictionary<(bool Grouped, string ContentSize), double> minimumRowHeights = [];


	public static double MinimumRowHeight(
		bool grouped)
	{
		string contentSize = UIApplication.SharedApplication.PreferredContentSizeCategory.ToString();
		var key = (grouped, contentSize);

		if (minimumRowHeights.TryGetValue(key, out double resolved))
			return resolved;

		using ListSource source = new();
		using UITableView table = new(
			new CGRect(0, 0, 320, 100),
			grouped ? UITableViewStyle.InsetGrouped : UITableViewStyle.Plain)
		{
			Source = source
		};

		table.ReloadData();
		table.LayoutIfNeeded();

		resolved = table.RectForRowAtIndexPath(NSIndexPath.FromRowSection(0, 0)).Height;
		if (resolved <= 0)
			resolved = OperatingSystem.IsIOSVersionAtLeast(26) ? 52 : 44;

		minimumRowHeights[key] = resolved;

		return resolved;
	}


	sealed class ListSource : UITableViewSource
	{
		const string ReuseIdentifier = "SkeleKit.SystemListMetrics.Row";


		public override nint RowsInSection(
			UITableView tableView,
			nint section) => 1;


		public override UITableViewCell GetCell(
			UITableView tableView,
			NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(ReuseIdentifier)
				?? new UITableViewCell(UITableViewCellStyle.Default, ReuseIdentifier);
			UIListContentConfiguration content = cell.DefaultContentConfiguration;
			content.Text = " ";
			cell.ContentConfiguration = content;

			return cell;
		}
	}
}
