using BareUI.Primitives;
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
		nfloat width = double.IsFinite(availableSize.Width) ? (nfloat)availableSize.Width : nfloat.MaxValue;
		nfloat height = double.IsFinite(availableSize.Height) ? (nfloat)availableSize.Height : nfloat.MaxValue;

		CGSize fit = Native.SizeThatFits(new(width, height));
		return new(fit.Width, fit.Height);
	}
#endif
}
