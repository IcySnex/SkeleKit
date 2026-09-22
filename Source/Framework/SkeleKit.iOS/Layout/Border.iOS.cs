using CoreAnimation;

namespace SkeleKit;

public partial class Border
{
	CAShapeLayer? strokeLayer;

	bool strokeGeometryApplied;
	nfloat strokeGeometryWidth = -1;
	nfloat strokeGeometryHeight = -1;
	nfloat strokeGeometryInset = -1;
	nfloat strokeGeometryRadius = -1;


	partial void ApplyStrokeCore()
	{
		double thickness = Math.Max(0, strokeThickness);

		if (strokeDashPattern is not { Count: > 0 } pattern)
		{
			DropStrokeLayer();
			Native.Layer.BorderWidth = (nfloat)thickness;
			Native.Layer.BorderColor = stroke?.ToUIColor().CGColor;
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
			strokeGeometryApplied = false;
		}

		CATransaction.Begin();
		CATransaction.DisableActions = true;
		strokeLayer.Hidden = stroke is null || thickness <= 0;
		strokeLayer.StrokeColor = stroke?.ToUIColor().CGColor;
		strokeLayer.LineWidth = (nfloat)thickness;
		strokeLayer.LineCap = strokeLineCap switch
		{
			SkeleKit.StrokeLineCap.Round => CAShapeLayer.CapRound,
			SkeleKit.StrokeLineCap.Square => CAShapeLayer.CapSquare,
			_ => CAShapeLayer.CapButt
		};
		strokeLayer.LineDashPattern = [.. pattern.Select(NSNumber.FromDouble)];
		CATransaction.Commit();

		SyncStrokeGeometry(new(Native.Bounds.Width, Native.Bounds.Height));
	}

	partial void ApplyStrokeDashPatternCore()
	{
		if (IsRealized)
			ApplyStrokeCore();
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
			Math.Max(0, strokeThickness) / 2,
			Math.Min(width, height) / 2);
		nfloat radius = (nfloat)Math.Max(0, ResolveCornerRadius(size) - inset);

		// arranging runs on every rebind; rebuilding the path only when the shape changed
		// keeps the dashed empty cells from allocating a UIBezierPath per pass
		if (strokeGeometryApplied
			&& width == strokeGeometryWidth
			&& height == strokeGeometryHeight
			&& inset == strokeGeometryInset
			&& radius == strokeGeometryRadius)
			return;

		CGRect bounds = new(
			inset,
			inset,
			Math.Max(0, width - inset * 2),
			Math.Max(0, height - inset * 2));

		using UIBezierPath path = UIBezierPath.FromRoundedRect(bounds, radius);

		CATransaction.Begin();
		CATransaction.DisableActions = true;
		strokeLayer.Frame = new(0, 0, width, height);
		strokeLayer.Path = path.CGPath;
		CATransaction.Commit();

		strokeGeometryApplied = true;
		strokeGeometryWidth = width;
		strokeGeometryHeight = height;
		strokeGeometryInset = inset;
		strokeGeometryRadius = radius;
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

		HookStrokeDashPattern();
		ApplyStrokeCore();
	}

	private protected override void OnUnrealized()
	{
		UnhookStrokeDashPattern();
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
