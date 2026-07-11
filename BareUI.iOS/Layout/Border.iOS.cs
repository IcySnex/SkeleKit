namespace BareUI;

public partial class Border
{
	private protected override void OnRealized()
	{
		base.OnRealized();

		if (Stroke is { } stroke && StrokeThickness > 0)
		{
			Native.Layer.BorderWidth = (nfloat)StrokeThickness;
			Native.Layer.BorderColor = stroke.ToUIColor().CGColor;
		}
	}
}
