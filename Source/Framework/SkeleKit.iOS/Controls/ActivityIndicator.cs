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

	/// <summary>
	/// The spinner color, or null for the system default.
	/// </summary>
	public Bindable<Color?> Color
	{
		get => color;
		set => colorBinding = Register(colorBinding, value, value => Set(ref color, value, ApplyColor, affectsMeasure: false));
	}
	Color? color;
	Binding<Color?>? colorBinding;


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
		// ignores the view tint, needs its own Color
		Ui.Color = (color ?? Tint)?.ToUIColor();
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
