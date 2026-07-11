using UIKit;

namespace BareUI;

/// <summary>
/// An increment/decrement control wrapping <c>UIStepper</c>.
/// </summary>
public class Stepper : Control
{
	/// <summary>
	/// The current value.
	/// </summary>
	public double Value { get; set; }

	/// <summary>
	/// The minimum selectable value.
	/// </summary>
	public double Minimum { get; set; } = 0;

	/// <summary>
	/// The maximum selectable value.
	/// </summary>
	public double Maximum { get; set; } = 100;

	/// <summary>
	/// The amount added or subtracted per tap.
	/// </summary>
	public double Step { get; set; } = 1;

	/// <summary>
	/// Invoked with the new value whenever the user taps the stepper.
	/// </summary>
	public Action<double>? ValueChanged { get; set; }

	private protected override UIView CreateNative()
	{
		UIStepper stepper = new()
		{
			MinimumValue = Minimum,
			MaximumValue = Maximum,
			StepValue = Step,
			Value = Value
		};

		stepper.ValueChanged += (sender, e) =>
		{
			Value = stepper.Value;
			ValueChanged?.Invoke(Value);
		};

		return stepper;
	}
}
