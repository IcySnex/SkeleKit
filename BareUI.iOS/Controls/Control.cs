namespace BareUI;

/// <summary>
/// Base for native control wrappers: measurement delegates to the control's own SizeThatFits.
/// </summary>
public abstract class Control : View
{
	protected override Size MeasureOverride(
		Size availableSize)
	{
		CGSize fit = Native.SizeThatFits(ClampToFinite(availableSize));
		return new(fit.Width, fit.Height);
	}

	/// <summary>
	/// Clamps an open-ended constraint boundary size to finite maximum points acceptable by native measurement signatures.
	/// </summary>
	/// <param name="availableSize">The logical layout boundaries supplied by the nesting view group context.</param>
	/// <returns>A concrete sizing platform structure containing fallback scalar values where infinity was present.</returns>
	private protected static CGSize ClampToFinite(
		Size availableSize)
	{
		nfloat width = double.IsFinite(availableSize.Width) ? (nfloat)availableSize.Width : nfloat.MaxValue;
		nfloat height = double.IsFinite(availableSize.Height) ? (nfloat)availableSize.Height : nfloat.MaxValue;

		return new(width, height);
	}
}
