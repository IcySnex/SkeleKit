namespace SkeleKit;

/// <summary>
/// An activity indicator spinner.
/// </summary>
public class ActivityIndicator : Control
{
	UIActivityIndicatorView Ui => (UIActivityIndicatorView)Native;


	/// <summary>
	/// Whether the spinner is animating.
	/// </summary>
	public Bindable<bool> IsAnimating
	{
		get => isAnimating;
		set => isAnimatingBinding = Register(isAnimatingBinding, value, value => Set(ref isAnimating, value, ApplyIsAnimating, affectsMeasure: false));
	}
	bool isAnimating = true;
	Binding<bool>? isAnimatingBinding;

	/// <summary>
	/// Whether to use the large style instead of medium.
	/// </summary>
	public bool IsLarge
	{
		get => isLarge;
		set => Set(ref isLarge, value, ApplyStyle);
	}
	bool isLarge;


	void ApplyIsAnimating()
	{
		if (isAnimating)
			Ui.StartAnimating();
		else
			Ui.StopAnimating();
	}

	void ApplyStyle() =>
		Ui.ActivityIndicatorViewStyle = isLarge ? UIActivityIndicatorViewStyle.Large : UIActivityIndicatorViewStyle.Medium;

	void ApplyColor()
	{
		// UIActivityIndicatorView doesn't derive its spinner color from UIView.TintColor,
		// so apply the effective tint explicitly.
		Ui.Color = EffectiveTint?.ToUIColor();
	}


	private protected override UIView CreateNative() =>
		new UIActivityIndicatorView(IsLarge ? UIActivityIndicatorViewStyle.Large : UIActivityIndicatorViewStyle.Medium)
		{
			HidesWhenStopped = true
		};

	private protected override void ApplyProperties()
	{
		ApplyStyle();
		ApplyColor();
		ApplyIsAnimating();
	}


	internal override void TintChanged()
	{
		if (IsRealized)
			ApplyColor();
	}
}
