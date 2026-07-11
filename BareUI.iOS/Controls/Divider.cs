using UIKit;

namespace BareUI;

/// <summary>
/// A hairline separator view.
/// </summary>
public class Divider : View
{
	/// <summary>
	/// The divider color, or null for the system separator color.
	/// </summary>
	public Color? Color { get; set; }

	private protected override UIView CreateNative()
	{
		return new()
		{
			BackgroundColor = Color?.ToUIColor() ?? UIColor.Separator
		};
	}

	protected override Size MeasureOverride(
		Size availableSize) =>
		new(0, 1.0 / UIScreen.MainScreen.Scale);
}
