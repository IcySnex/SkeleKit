namespace BareUI;

public partial class Border
{
	private protected override void OnRealized()
	{
		base.OnRealized();

		ApplyStroke();
	}

	// CGColor is a snapshot, so a theme change re-resolves it here
	internal override void ReapplyVisuals()
	{
		base.ReapplyVisuals();

		if (IsRealized)
			ApplyStroke();
	}

	void ApplyStroke()
	{
		if (Stroke is { } stroke && StrokeThickness > 0)
		{
			Native.Layer.BorderWidth = (nfloat)StrokeThickness;
			Native.Layer.BorderColor = stroke.ToUIColor().CGColor;
		}
	}
}
