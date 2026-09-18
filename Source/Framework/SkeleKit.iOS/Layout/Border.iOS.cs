using CoreAnimation;

namespace SkeleKit;

public partial class Border
{
	CAShapeLayer? strokeLayer;


	partial void ApplyStrokeCore()
	{
		double thickness = Math.Max(0, StrokeThickness);

		if (StrokeDashPattern is not { Length: > 0 } pattern)
		{
			DropStrokeLayer();
			Native.Layer.BorderWidth = (nfloat)thickness;
			Native.Layer.BorderColor = Stroke?.ToUIColor().CGColor;
			return;
		}

		Native.Layer.BorderWidth = 0;
		Native.Layer.BorderColor = null;

		if (strokeLayer is null)
		{
			strokeLayer = new()
			{
				FillColor = UIColor.Clear.CGColor,
				ContentsScale = Native.Layer.ContentsScale,
				ZPosition = 1
			};
			Native.Layer.AddSublayer(strokeLayer);
		}

		CATransaction.Begin();
		CATransaction.DisableActions = true;
		strokeLayer.Hidden = Stroke is null || thickness <= 0;
		strokeLayer.StrokeColor = Stroke?.ToUIColor().CGColor;
		strokeLayer.LineWidth = (nfloat)thickness;
		strokeLayer.LineCap = StrokeLineCap switch
		{
			StrokeLineCap.Round => CAShapeLayer.CapRound,
			StrokeLineCap.Square => CAShapeLayer.CapSquare,
			_ => CAShapeLayer.CapButt
		};
		strokeLayer.LineDashPattern = [.. pattern.Select(NSNumber.FromDouble)];
		CATransaction.Commit();

		SyncStrokeGeometry(new(Native.Bounds.Width, Native.Bounds.Height));
	}

	partial void ArrangeStrokeCore(
		Size finalSize) =>
		SyncStrokeGeometry(finalSize);

	void SyncStrokeGeometry(
		Size size)
	{
		if (strokeLayer is null)
			return;

		nfloat width = (nfloat)Math.Max(0, size.Width);
		nfloat height = (nfloat)Math.Max(0, size.Height);
		nfloat inset = (nfloat)Math.Min(
			Math.Max(0, StrokeThickness) / 2,
			Math.Min(width, height) / 2);
		CGRect bounds = new(
			inset,
			inset,
			Math.Max(0, width - inset * 2),
			Math.Max(0, height - inset * 2));
		nfloat radius = (nfloat)Math.Max(0, CornerRadius - inset);

		using UIBezierPath path = UIBezierPath.FromRoundedRect(bounds, radius);

		CATransaction.Begin();
		CATransaction.DisableActions = true;
		strokeLayer.Frame = new(0, 0, width, height);
		strokeLayer.Path = path.CGPath;
		CATransaction.Commit();
	}

	void DropStrokeLayer()
	{
		strokeLayer?.RemoveFromSuperLayer();
		strokeLayer?.Dispose();
		strokeLayer = null;
	}

	private protected override void OnVisualStateApplied()
	{
		base.OnVisualStateApplied();

		if (strokeLayer is not null && IsRealized)
			SyncStrokeGeometry(new(Native.Bounds.Width, Native.Bounds.Height));
	}
	
	private protected override void OnRealized()
	{
		base.OnRealized();

		ApplyStrokeCore();
	}

	private protected override void OnUnrealized()
	{
		DropStrokeLayer();
		base.OnUnrealized();
	}


	internal override void ReapplyVisuals()
	{
		base.ReapplyVisuals();

		// CGColor is a snapshot, so a theme change re-resolves it here
		if (IsRealized)
			ApplyStrokeCore();
	}
}
