namespace SkeleKit;

internal sealed class SplitViewService(
	Func<SkeleSplit?> activeSplit) : ISplitView
{
	SkeleSplit Active() =>
		activeSplit()
		?? throw new InvalidOperationException("There is no active split view controller.");

	static void EnsureAvailable(
		SplitViewColumn column)
	{
		if (column is SplitViewColumn.Inspector
			&& !OperatingSystem.IsIOSVersionAtLeast(26))
			throw new PlatformNotSupportedException("The split view inspector column requires iOS 26 or later.");
	}


	public bool IsCollapsed =>
		Active().Collapsed;

	public bool IsShowing(
		SplitViewColumn column)
	{
		EnsureAvailable(column);

		if (!OperatingSystem.IsIOSVersionAtLeast(26))
			throw new PlatformNotSupportedException("Querying split view column visibility requires iOS 26 or later. Use Show or Hide instead.");

		return Active().IsShowingColumn(SkeleSplit.Native(column));
	}

	public void Show(
		SplitViewColumn column)
	{
		EnsureAvailable(column);
		Active().ShowColumn(SkeleSplit.Native(column));
	}

	public void Hide(
		SplitViewColumn column)
	{
		EnsureAvailable(column);
		Active().HideColumn(SkeleSplit.Native(column));
	}

	public void Toggle(
		SplitViewColumn column)
	{
		EnsureAvailable(column);

		if (!OperatingSystem.IsIOSVersionAtLeast(26))
			throw new PlatformNotSupportedException("Toggling a split view column requires iOS 26 or later. Use Show or Hide instead.");

		if (Active().IsShowingColumn(SkeleSplit.Native(column)))
			Hide(column);
		else
			Show(column);
	}
}
