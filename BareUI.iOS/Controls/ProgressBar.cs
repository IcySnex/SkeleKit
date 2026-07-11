#if IOS
using UIKit;
#endif

namespace BareUI;

/// <summary>
/// A progress bar wrapping <c>UIProgressView</c>.
/// </summary>
public class ProgressBar : Control
{
	/// <summary>
	/// The progress value from 0 (empty) to 1 (full).
	/// </summary>
	public double Progress { get; set; }

	/// <summary>
	/// The progress bar tint color, or null for the system default.
	/// </summary>
	public Color? Tint { get; set; }

#if IOS
	private protected override UIView CreateNative()
	{
		UIProgressView progress = new(UIProgressViewStyle.Default)
		{
			Progress = (float)Progress
		};

		if (Tint is { } tint)
			progress.ProgressTintColor = tint.ToUIColor();

		return progress;
	}
#endif
}
