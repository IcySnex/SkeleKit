namespace BareUI;

/// <summary>
/// Base for native control wrappers: measurement delegates to the control's own SizeThatFits.
/// </summary>
public abstract class Control : View
{
	// native SizeThatFits rejects infinity, so open constraints cap at nfloat.MaxValue
	private protected static CGSize ClampToFinite(
		Size availableSize)
	{
		nfloat width = double.IsFinite(availableSize.Width) ? (nfloat)availableSize.Width : nfloat.MaxValue;
		nfloat height = double.IsFinite(availableSize.Height) ? (nfloat)availableSize.Height : nfloat.MaxValue;

		return new(width, height);
	}


	protected override Size MeasureOverride(
		Size availableSize)
	{
		CGSize fit = Native.SizeThatFits(ClampToFinite(availableSize));
		return new(fit.Width, fit.Height);
	}
}
