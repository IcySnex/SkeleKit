namespace SkeleKit;

/// <summary>
/// Controls the currently active native split view from a ViewModel.
/// </summary>
public interface ISplitView
{
	/// <summary>
	/// Whether the active split view is currently collapsed to a single navigation surface.
	/// </summary>
	bool IsCollapsed { get; }

	/// <summary>
	/// Returns whether the given column is currently visible.
	/// </summary>
	/// <remarks>
	/// Requires iOS 26 or later. On earlier versions use <see cref="Show"/> and <see cref="Hide"/>,
	/// which are available since iOS 18.
	/// </remarks>
	bool IsShowing(
		SplitViewColumn column);

	/// <summary>
	/// Shows the given column using UIKit's adaptive presentation.
	/// </summary>
	void Show(
		SplitViewColumn column);

	/// <summary>
	/// Hides the given column using UIKit's adaptive presentation.
	/// </summary>
	void Hide(
		SplitViewColumn column);

	/// <summary>
	/// Shows a hidden column or hides a visible one.
	/// </summary>
	/// <remarks>
	/// Requires iOS 26 or later because it depends on <see cref="IsShowing"/>. On earlier versions
	/// use <see cref="Show"/> and <see cref="Hide"/>.
	/// </remarks>
	void Toggle(
		SplitViewColumn column);
}
