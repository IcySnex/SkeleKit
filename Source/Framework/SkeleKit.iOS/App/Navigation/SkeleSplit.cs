namespace SkeleKit;

internal sealed class SkeleSplit : UISplitViewController
{
	readonly SplitViewColumn navigationColumn;


	public SkeleSplit(
		SplitViewStyle style,
		SplitViewColumn navigationColumn) : base(Native(style))
	{
		this.navigationColumn = navigationColumn;
	}

	public SkeleSplit(
		ObjCRuntime.NativeHandle handle) : base(handle)
	{ }


	internal UINavigationController? NavigationStack
	{
		get
		{
			if (Collapsed
				&& GetViewController(UISplitViewControllerColumn.Compact) is UINavigationController compact)
				return compact;

			return GetViewController(NavigationColumn) as UINavigationController;
		}
	}

	UISplitViewControllerColumn NavigationColumn =>
		navigationColumn is SplitViewColumn.Inspector
			&& !OperatingSystem.IsIOSVersionAtLeast(26)
				? UISplitViewControllerColumn.Secondary
				: Native(navigationColumn);

	internal static UISplitViewControllerColumn Native(
		SplitViewColumn column)
	{
		if (column is SplitViewColumn.Inspector)
		{
			if (!OperatingSystem.IsIOSVersionAtLeast(26))
				throw new PlatformNotSupportedException("The split view inspector column requires iOS 26 or later.");

			return UISplitViewControllerColumn.Inspector;
		}

		return column switch
		{
			SplitViewColumn.Primary => UISplitViewControllerColumn.Primary,
			SplitViewColumn.Supplementary => UISplitViewControllerColumn.Supplementary,
			SplitViewColumn.Secondary => UISplitViewControllerColumn.Secondary,
			SplitViewColumn.Compact => UISplitViewControllerColumn.Compact,
			_ => UISplitViewControllerColumn.Secondary
		};
	}

	static UISplitViewControllerStyle Native(
		SplitViewStyle style) =>
		style is SplitViewStyle.TripleColumn
			? UISplitViewControllerStyle.TripleColumn
			: UISplitViewControllerStyle.DoubleColumn;
}
