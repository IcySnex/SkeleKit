namespace BareUI;

/// <summary>
/// A continuous value picker.
/// </summary>
public class Slider : Control
{
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
	public double Minimum
	{
		get => minimum;
		set => Set(ref minimum, value, ApplyRange, affectsMeasure: false);
	}
	double minimum;

	/// <summary>
	/// The maximum selectable value.
	/// </summary>
	public double Maximum
	{
		get => maximum;
		set => Set(ref maximum, value, ApplyRange, affectsMeasure: false);
	}
	double maximum = 1;

	/// <summary>
	/// The increment the value snaps to, or 0 for continuous.
	/// </summary>
	public double Step
	{
		get => step;
		set => Set(ref step, value, affectsMeasure: false);
	}
	double step;

	/// <summary>
	/// Whether the value updates all through the drag, rather than only when the thumb is released.
	/// </summary>
	public bool Continuous
	{
		get => continuous;
		set => Set(ref continuous, value, ApplyStyle, affectsMeasure: false);
	}
	bool continuous = true;

	/// <summary>
	/// The color of the filled part of the track, or null for the system tint.
	/// </summary>
	public Color? TrackColor
	{
		get => trackColor;
		set => Set(ref trackColor, value, ApplyStyle, affectsMeasure: false);
	}
	Color? trackColor;

	/// <summary>
	/// The color of the unfilled part of the track, or null for the system default.
	/// </summary>
	public Color? EmptyTrackColor
	{
		get => emptyTrackColor;
		set => Set(ref emptyTrackColor, value, ApplyStyle, affectsMeasure: false);
	}
	Color? emptyTrackColor;

	/// <summary>
	/// The thumb color, or null for the system default.
	/// </summary>
	public Color? ThumbColor
	{
		get => thumbColor;
		set => Set(ref thumbColor, value, ApplyStyle, affectsMeasure: false);
	}
	Color? thumbColor;

	/// <summary>
	/// The SF Symbol shown at the minimum end, or null for none.
	/// </summary>
	public string? MinIcon
	{
		get => minIcon;
		set => Set(ref minIcon, value, ApplyStyle);
	}
	string? minIcon;

	/// <summary>
	/// The SF Symbol shown at the maximum end, or null for none.
	/// </summary>
	public string? MaxIcon
	{
		get => maxIcon;
		set => Set(ref maxIcon, value, ApplyStyle);
	}
	string? maxIcon;

	/// <summary>
	/// Invoked with the new value whenever the user moves the slider.
	/// </summary>
	public Action<double>? ValueChanged { get; set; }


	private protected override UIView CreateNative()
	{
		UISlider slider = new();
		slider.ValueChanged += (_, _) => OnValueChanged();

		return slider;
	}

	private protected override void ApplyProperties()
	{
		ApplyRange();
		ApplyValue();
		ApplyStyle();
	}

	UISlider Ui =>
		(UISlider)Native;

	void ApplyRange()
	{
		Ui.MinValue = (float)minimum;
		Ui.MaxValue = (float)maximum;
	}

	void ApplyValue() =>
		Ui.Value = (float)current;

	void ApplyStyle()
	{
		if (trackColor is { } track)
			Ui.MinimumTrackTintColor = track.ToUIColor();

		if (emptyTrackColor is { } empty)
			Ui.MaximumTrackTintColor = empty.ToUIColor();

		if (thumbColor is { } thumb)
			Ui.ThumbTintColor = thumb.ToUIColor();

		Ui.MinValueImage = minIcon is { } min ? UIImage.GetSystemImage(min) : null;
		Ui.MaxValueImage = maxIcon is { } max ? UIImage.GetSystemImage(max) : null;

		Ui.Continuous = continuous;
	}

	void OnValueChanged()
	{
		double value = Ui.Value;

		if (step > 0)
		{
			value = minimum + (Math.Round((value - minimum) / step) * step);
			Ui.Value = (float)value;
		}

		Set(ref current, value, affectsMeasure: false);
		valueBinding?.PushToSource(value);
		ValueChanged?.Invoke(value);
	}
}
