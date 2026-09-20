namespace SkeleKit;

/// <summary>
/// A continuous value picker.
/// </summary>
public class Slider : Control
{
	UISlider Ui => (UISlider)Native;


	/// <summary>
	/// The current value.
	/// </summary>
	public Bindable<double> Value
	{
		get => current;
		set => valueBinding = Register(valueBinding, value, value => Set(ref current, value, ApplyValue, affectsMeasure: false));
	}
	double current;
	Binding<double>? valueBinding;

	/// <summary>
	/// The minimum selectable value.
	/// </summary>
	public Bindable<double> Minimum
	{
		get => minimum;
		set => minimumBinding = Register(minimumBinding, value, value => Set(ref minimum, value, ApplyRange, affectsMeasure: false));
	}
	double minimum;
	Binding<double>? minimumBinding;

	/// <summary>
	/// The maximum selectable value.
	/// </summary>
	public Bindable<double> Maximum
	{
		get => maximum;
		set => maximumBinding = Register(maximumBinding, value, value => Set(ref maximum, value, ApplyRange, affectsMeasure: false));
	}
	double maximum = 1;
	Binding<double>? maximumBinding;

	/// <summary>
	/// The increment the value snaps to, or 0 for continuous.
	/// </summary>
	/// <remarks>
	/// Stepping requires iOS 26 or later.
	/// </remarks>
	public Bindable<double> Step
	{
		get => step;
		set => stepBinding = Register(stepBinding, value, value => Set(ref step, value, ApplyStep, affectsMeasure: false));
	}
	double step;
	Binding<double>? stepBinding;

	/// <summary>
	/// Whether the value updates all through the drag, rather than only when the thumb is released.
	/// </summary>
	public bool Continuous
	{
		get => continuous;
		set => Set(ref continuous, value, ApplyRange, affectsMeasure: false);
	}
	bool continuous = true;

	/// <summary>
	/// The color of the filled part of the track, or null for the system tint.
	/// </summary>
	public Bindable<Color?> TrackColor
	{
		get => trackColor;
		set => trackColorBinding = Register(trackColorBinding, value, value => Set(ref trackColor, value, ApplyStyle, affectsMeasure: false));
	}
	Color? trackColor;
	Binding<Color?>? trackColorBinding;

	/// <summary>
	/// The color of the unfilled part of the track, or null for the system default.
	/// </summary>
	public Bindable<Color?> EmptyTrackColor
	{
		get => emptyTrackColor;
		set => emptyTrackColorBinding = Register(emptyTrackColorBinding, value, value => Set(ref emptyTrackColor, value, ApplyStyle, affectsMeasure: false));
	}
	Color? emptyTrackColor;
	Binding<Color?>? emptyTrackColorBinding;

	/// <summary>
	/// The thumb color, or null for the system default.
	/// </summary>
	public Bindable<Color?> ThumbColor
	{
		get => thumbColor;
		set => thumbColorBinding = Register(thumbColorBinding, value, value => Set(ref thumbColor, value, ApplyStyle, affectsMeasure: false));
	}
	Color? thumbColor;
	Binding<Color?>? thumbColorBinding;

	/// <summary>
	/// The local icon shown at the minimum end, or null for none.
	/// </summary>
	public Bindable<ImageSource?> MinIcon
	{
		get => minIcon;
		set => minIconBinding = Register(minIconBinding, value, value => Set(ref minIcon, value, ApplyStyle));
	}
	ImageSource? minIcon;
	Binding<ImageSource?>? minIconBinding;

	/// <summary>
	/// The local icon shown at the maximum end, or null for none.
	/// </summary>
	public Bindable<ImageSource?> MaxIcon
	{
		get => maxIcon;
		set => maxIconBinding = Register(maxIconBinding, value, value => Set(ref maxIcon, value, ApplyStyle));
	}
	ImageSource? maxIcon;
	Binding<ImageSource?>? maxIconBinding;

	/// <summary>
	/// Invoked with the new value whenever the user moves the slider.
	/// </summary>
	public Action<double>? ValueChanged { get; set; }


	void ApplyRange()
	{
		Ui.MinValue = (float)minimum;
		Ui.MaxValue = (float)maximum;

		Ui.Continuous = continuous;
	}

	void ApplyStep()
	{
		if (!OperatingSystem.IsIOSVersionAtLeast(26))
			return;

		int ticks = step <= 0 ? 0 : (int)((maximum - minimum) / step) + 1;
		Ui.TrackConfiguration = UISliderTrackConfiguration.Create(ticks);
	}

	void ApplyValue() =>
		Ui.Value = (float)current;

	void ApplyStyle()
	{
		Ui.MinimumTrackTintColor = trackColor?.ToUIColor();
		Ui.MaximumTrackTintColor = emptyTrackColor?.ToUIColor();
		Ui.ThumbTintColor = thumbColor?.ToUIColor();

		Ui.MinValueImage = minIcon?.ResolveLocal();
		Ui.MaxValueImage = maxIcon?.ResolveLocal();
	}

	void OnValueChanged()
	{
		double value = Ui.Value;

		Set(ref current, value, affectsMeasure: false);
		valueBinding?.PushToSource(value);
		ValueChanged?.Invoke(value);
	}


	private protected override UIView CreateNative()
	{
		UISlider slider = new();
		slider.ValueChanged += (_, _) => OnValueChanged();

		return slider;
	}

	private protected override void ApplyProperties()
	{
		ApplyStep();
		ApplyRange();
		ApplyValue();
		ApplyStyle();
	}
}
