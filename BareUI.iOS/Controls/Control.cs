#if IOS
using CoreGraphics;
#endif

namespace BareUI;

/// <summary>
/// Base for native control wrappers: measurement delegates to the control's own SizeThatFits.
/// </summary>
public abstract class Control : View
{
#if IOS
	protected override Size MeasureOverride(
		Size availableSize)
	{
		CGSize fit = Native.SizeThatFits(ClampToFinite(availableSize));
		return new(fit.Width, fit.Height);
	}

	// Converts a possibly-infinite available size into a finite CGSize UIKit's SizeThatFits accepts.
	private protected static CGSize ClampToFinite(
		Size availableSize)
	{
		nfloat width = double.IsFinite(availableSize.Width) ? (nfloat)availableSize.Width : nfloat.MaxValue;
		nfloat height = double.IsFinite(availableSize.Height) ? (nfloat)availableSize.Height : nfloat.MaxValue;

		return new(width, height);
	}
#endif
}
