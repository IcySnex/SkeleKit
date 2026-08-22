namespace SkeleKit;

public partial class Border
{
	partial void ApplyStrokeCore()
	{
		Native.Layer.BorderWidth = (nfloat)Math.Max(0, StrokeThickness);
		Native.Layer.BorderColor = Stroke?.ToUIColor().CGColor;
	}

	
	private protected override void OnRealized()
	{
		base.OnRealized();

		ApplyStrokeCore();
	}


	internal override void ReapplyVisuals()
	{
		base.ReapplyVisuals();

		// CGColor is a snapshot, so a theme change re-resolves it here
		if (IsRealized)
			ApplyStrokeCore();
	}
}
