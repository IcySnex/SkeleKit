using System.Runtime.Versioning;

namespace SkeleKit;

/// <summary>
/// A continuous value picker.
/// </summary>
public class Slider : Control
{
	const int MaxNativeTicks = 1024;

	static readonly UIImage HiddenTick = new UIGraphicsImageRenderer(new(1, 1))
		.CreateImage(static _ => { });


	UISlider Ui => (UISlider)Native;

	bool nativeSteps;


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
	/// <remarks>
	/// User changes are reported once per snapped value.
	/// iOS 26 uses native slider ticks for representable stepped ranges.
	/// </remarks>
	public double Step
	{
		get => step;
		set => Set(ref step, value, ApplyStep, affectsMeasure: false);
	}
	double step;

	/// <summary>
	/// Whether native step tick marks are visible on iOS 26.
	/// </summary>
	public bool ShowsTicks
	{
		get;
		set => Set(ref field, value, ApplyStep);
	} = true;

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


	void ApplyRange()
	{
		Ui.MinValue = (float)minimum;
		Ui.MaxValue = (float)maximum;
		ApplyStep();
	}

	void ApplyValue() =>
		Ui.Value = (float)current;

	void ApplyStep()
	{
		nativeSteps = false;

		if (OperatingSystem.IsIOSVersionAtLeast(26))
		{
			UISliderTrackConfiguration? configuration = NativeStepConfiguration();
			Ui.TrackConfiguration = configuration;
			nativeSteps = configuration is not null;
		}

		ApplyValue();
	}

	[SupportedOSPlatform("ios26.0")]
	UISliderTrackConfiguration? NativeStepConfiguration()
	{
		double span = maximum - minimum;
		if (step <= 0 || span <= 0)
			return null;

		double intervals = span / step;
		if (!double.IsFinite(intervals) || intervals + 2 > MaxNativeTicks)
			return null;

		int whole = (int)Math.Floor(intervals);
		double remainder = span - whole * step;
		int count = whole + 1 + (remainder > span * 1e-9 ? 1 : 0);

		List<UISliderTick> ticks = new(count);
		UIImage? image = ShowsTicks ? null : HiddenTick;

		for (int index = 0; index <= whole; index++)
		{
			float position = (float)(index * step / span);
			ticks.Add(UISliderTick.Create(position, null, image));
		}

		if (ticks.Count < count)
			ticks.Add(UISliderTick.Create(1, null, image));

		UISliderTrackConfiguration configuration = UISliderTrackConfiguration.Create([.. ticks]);
		configuration.AllowsTickValuesOnly = true;

		return configuration;
	}

	void ApplyStyle()
	{
		if (trackColor is Color track)
			Ui.MinimumTrackTintColor = track.ToUIColor();

		if (emptyTrackColor is Color empty)
			Ui.MaximumTrackTintColor = empty.ToUIColor();

		if (thumbColor is Color thumb)
			Ui.ThumbTintColor = thumb.ToUIColor();

		Ui.MinValueImage = minIcon is string min ? UIImage.GetSystemImage(min) : null;
		Ui.MaxValueImage = maxIcon is string max ? UIImage.GetSystemImage(max) : null;

		Ui.Continuous = continuous;
	}

	void OnValueChanged()
	{
		double value = Ui.Value;

		if (step > 0 && !nativeSteps)
			value = Math.Clamp(
				minimum + Math.Round((value - minimum) / step) * step,
				Math.Min(minimum, maximum),
				Math.Max(minimum, maximum));

		if (value == current)
			return;

		if (step > 0 && !nativeSteps)
			Ui.Value = (float)value;

		Set(ref current, value, affectsMeasure: false);
		valueBinding?.PushToSource(value);
		ValueChanged?.Invoke(value);
	}

	void SettleValue()
	{
		if (!nativeSteps)
			ApplyValue();
	}


	private protected override UIView CreateNative()
	{
		UISlider slider = new();
		slider.ValueChanged += (_, _) => OnValueChanged();
		slider.TouchUpInside += (_, _) => SettleValue();
		slider.TouchUpOutside += (_, _) => SettleValue();
		slider.TouchCancel += (_, _) => SettleValue();

		return slider;
	}

	private protected override void ApplyProperties()
	{
		ApplyRange();
		ApplyValue();
		ApplyStyle();
	}
}
