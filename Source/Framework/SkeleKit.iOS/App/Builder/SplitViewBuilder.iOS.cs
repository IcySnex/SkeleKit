namespace SkeleKit;

public sealed partial class SplitViewBuilder
{
	internal Action<UISplitViewController>? NativeConfiguration { get; private set; }


	/// <summary>
	/// Configures the native split view controller after SkeleKit installs its columns.
	/// </summary>
	/// <remarks>
	/// Use this escape hatch for display mode, split behavior, gestures, column widths, delegates,
	/// and other UIKit options that SkeleKit doesn't need to duplicate.
	/// </remarks>
	/// <param name="configure">Configures the native controller.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SplitViewBuilder ConfigureNative(
		Action<UISplitViewController> configure)
	{
		ArgumentNullException.ThrowIfNull(configure);
		NativeConfiguration = configure;

		return this;
	}
}
