using UIKit;

namespace BareUI;

/// <summary>
/// A continuous value picker wrapping <c>UISlider</c>.
/// </summary>
public class Slider : Control
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
	public double Maximum { get; set; } = 1;

	/// <summary>
	/// Invoked with the new value whenever the user moves the slider.
	/// </summary>
	public Action<double>? ValueChanged { get; set; }

	private protected override UIView CreateNative()
	{
		UISlider slider = new()
		{
			MinValue = (float)Minimum,
			MaxValue = (float)Maximum,
			Value = (float)Value
		};

		slider.ValueChanged += (sender, e) =>
		{
			Value = slider.Value;
			ValueChanged?.Invoke(Value);
		};

		return slider;
	}
}
