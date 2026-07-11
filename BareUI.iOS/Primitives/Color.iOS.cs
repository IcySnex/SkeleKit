using UIKit;

namespace BareUI;

static class ColorInterop
{
	/// <summary>
	/// Converts a neutral <see cref="Color"/> to its <c>UIColor</c> equivalent.
	/// </summary>
	public static UIColor ToUIColor(
		this Color color) =>
		UIColor.FromRGBA(
			(nfloat)color.Red,
			(nfloat)color.Green,
			(nfloat)color.Blue,
			(nfloat)color.Alpha);
}
