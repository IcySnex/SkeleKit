namespace BareUI;

public partial class Border
{
	void ApplyStroke()
	{
		if (Stroke is Color stroke && StrokeThickness > 0)
		{
			Native.Layer.BorderWidth = (nfloat)StrokeThickness;
			Native.Layer.BorderColor = stroke.ToUIColor().CGColor;
		}
	}

	
	private protected override void OnRealized()
	{
		base.OnRealized();

		ApplyStroke();
	}


	internal override void ReapplyVisuals()
	{
		base.ReapplyVisuals();

		// CGColor is a snapshot, so a theme change re-resolves it here
		if (IsRealized)
			ApplyStroke();
	}
}
