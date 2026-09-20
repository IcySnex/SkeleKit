namespace SkeleKit;

/// <summary>
/// An increment/decrement control.
/// </summary>
public class Stepper : Control
{
	UIStepper Ui => (UIStepper)Native;


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
	double maximum = 100;
	Binding<double>? maximumBinding;

	/// <summary>
	/// The amount added or subtracted per tap.
	/// </summary>
	public Bindable<double> Step
	{
		get => step;
		set => stepBinding = Register(stepBinding, value, value => Set(ref step, value, ApplyRange, affectsMeasure: false));
	}
	double step = 1;
	Binding<double>? stepBinding;

	/// <summary>
	/// Invoked with the new value whenever the user taps the stepper.
	/// </summary>
	public Action<double>? ValueChanged { get; set; }


	void ApplyRange()
	{
		Ui.MinimumValue = minimum;
		Ui.MaximumValue = maximum;
		Ui.StepValue = step;
	}

	void ApplyValue() =>
		Ui.Value = current;

	void OnValueChanged()
	{
		double value = Ui.Value;

		Set(ref current, value, affectsMeasure: false);
		valueBinding?.PushToSource(value);
		ValueChanged?.Invoke(value);
	}


	private protected override UIView CreateNative()
	{
		UIStepper stepper = new();
		stepper.ValueChanged += (_, _) => OnValueChanged();

		return stepper;
	}

	private protected override void ApplyProperties()
	{
		ApplyRange();
		ApplyValue();
	}
}
