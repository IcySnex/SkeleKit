using CoreFoundation;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace SkeleKit;

/// <summary>
/// The members a collection uses on a cell, regardless of the cell's UIKit base class.
/// </summary>
internal interface ISkeleHostCell
{
	View? Hosted { get; }

	int AnimationToken { get; set; }

	void Attach(
		ICollectionItemView item,
		bool fixedSize,
		bool selects,
		bool reorders,
		bool selectionCheckmark);

	void SetAutomaticMinimumHeight(
		double value);

	void SetHighlightBackground(
		Brush? value);

	void SetRetainsHighlight(
		bool value);

	void ApplySelectionTint();
}

/// <summary>
/// The cell for <see cref="CollectionLayout.FixedGrid"/>.
/// </summary>
/// <remarks>
/// A plain <see cref="UICollectionViewCell"/> carries a fixed grid. The list-cell base class runs
/// self-sizing and accessory work that a fixed grid never uses. This cell keeps only the hosted
/// view, the highlight state, and the fixed arrangement.
/// Item accessories, selection checkmarks, edit circles, and the reorder handle are not available.
/// </remarks>
internal sealed class SkelePlainCell(
	NativeHandle handle) : UICollectionViewCell(handle), ISkeleHostCell
{
	public View? Hosted { get; private set; }

	public int AnimationToken { get; set; }

	ICollectionItemView? source;

	Brush? highlight;
	bool retainsHighlight = true;
	bool lit;
	bool fixedSize;

	double automaticMinimumHeight;

	// the last arrangement, so a layout pass that changes nothing does not arrange again
	nfloat arrangedWidth;
	nfloat arrangedLeading;
	nfloat arrangedTrailing;
	nfloat arrangedContentHeight;
	bool arrangementValid;

	public void Attach(
		ICollectionItemView item,
		bool fixedSize,
		bool selects,
		bool reorders,
		bool selectionCheckmark)
	{
		source = item;
		Hosted = item.View;
		highlight = item.HighlightBackground;
		this.fixedSize = fixedSize;
		arrangementValid = false;

		BackgroundConfiguration = UIBackgroundConfiguration.ClearConfiguration;

		ContentView.AddSubview(Hosted.Realize());

		item.ObserveHighlightBackground(SetHighlightBackground);
	}

	public void SetAutomaticMinimumHeight(
		double value) =>
		automaticMinimumHeight = value;

	public void SetHighlightBackground(
		Brush? value)
	{
		highlight = value;

		if (lit)
			Hosted?.SetBackgroundOverride(value);

		SetNeedsUpdateConfiguration();
	}

	public void SetRetainsHighlight(
		bool value)
	{
		if (retainsHighlight == value)
			return;

		retainsHighlight = value;
		SetNeedsUpdateConfiguration();
	}

	public void ApplySelectionTint()
	{
		// no selection affordance to tint
	}

	public override void UpdateConfiguration(
		UICellConfigurationState state)
	{
		if (highlight is null)
			return;

		bool wantsLit = state.Highlighted || state.Selected && (retainsHighlight || state.Editing);
		if (wantsLit == lit && Hosted is not null)
			return;

		// the pressed look lands at once; releasing fades it outside the selection update, where
		// UIKit disables animations
		bool fades = lit && !wantsLit;
		lit = wantsLit;

		if (Hosted is not View hosted)
			return;

		if (!fades)
		{
			hosted.SetBackgroundOverride(wantsLit ? highlight : null);
			return;
		}

		DispatchQueue.MainQueue.DispatchAsync(() =>
		{
			if (!lit)
				UIView.Animate(0.25, () => hosted.SetBackgroundOverride(null));
		});
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		if (Hosted is null)
			return;

		CGRect content = ContentView.Frame;
		nfloat leading = content.X;
		nfloat trailing = (nfloat)Math.Max(0, (double)Bounds.Width - (double)content.GetMaxX());
		nfloat contentHeight = ContentView.Bounds.Height;

		if (arrangementValid
			&& arrangedWidth == Bounds.Width
			&& arrangedLeading == leading
			&& arrangedTrailing == trailing
			&& arrangedContentHeight == contentHeight)
			return;

		arrangementValid = true;
		arrangedWidth = Bounds.Width;
		arrangedLeading = leading;
		arrangedTrailing = trailing;
		arrangedContentHeight = contentHeight;

		source?.SetContentInsets(leading, trailing);
		Hosted.Arrange(new(-leading, 0, Bounds.Width, ContentView.Bounds.Height));
	}

	public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
		UICollectionViewLayoutAttributes layoutAttributes)
	{
		// the fixed layout owns every frame; no content measurement is needed
		if (fixedSize || Hosted is null)
			return layoutAttributes;

		Hosted.Measure(new(layoutAttributes.Frame.Width, double.PositiveInfinity));

		double height = Hosted.DesiredSize.Height;
		if (double.IsNaN(Hosted.Height) && double.IsNaN(Hosted.MinHeight))
		{
			height = Math.Max(height, automaticMinimumHeight);

			if (double.IsFinite(Hosted.MaxHeight))
				height = Math.Min(height, Hosted.MaxHeight + Hosted.Margin.Vertical);
		}

		CGRect frame = layoutAttributes.Frame;
		frame.Height = (nfloat)height;
		layoutAttributes.Frame = frame;

		return layoutAttributes;
	}
}
