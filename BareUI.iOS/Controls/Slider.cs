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

	void OnValueChanged()
	{
		double value = Ui.Value;

		Set(ref current, value, affectsMeasure: false);
		valueBinding?.PushToSource(value);
		ValueChanged?.Invoke(value);
	}
}
