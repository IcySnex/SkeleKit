using UIKit;

namespace BareUI;

/// <summary>
/// An activity indicator spinner wrapping <c>UIActivityIndicatorView</c>.
/// </summary>
public class ActivityIndicator : Control
{
	/// <summary>
	/// Whether the spinner is animating.
	/// </summary>
	public bool IsAnimating { get; set; } = true;

	/// <summary>
	/// Whether to use the large style instead of medium.
	/// </summary>
	public bool IsLarge { get; set; }

	/// <summary>
	/// The spinner color, or null for the system default.
	/// </summary>
	public Color? Color { get; set; }

	private protected override UIView CreateNative()
	{
		UIActivityIndicatorView indicator = new(
			IsLarge ? UIActivityIndicatorViewStyle.Large : UIActivityIndicatorViewStyle.Medium)
		{
			HidesWhenStopped = true
		};

		if (Color is { } color)
			indicator.Color = color.ToUIColor();

		if (IsAnimating)
			indicator.StartAnimating();

		return indicator;
	}
}
