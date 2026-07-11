using UIKit;

namespace BareUI;

/// <summary>
/// A binary on/off toggle wrapping <c>UISwitch</c>.
/// </summary>
public class Switch : Control
{
	/// <summary>
	/// Whether the switch is on.
	/// </summary>
	public bool IsOn { get; set; }

	/// <summary>
	/// Invoked with the new value whenever the user toggles the switch.
	/// </summary>
	public Action<bool>? Toggled { get; set; }

	private protected override UIView CreateNative()
	{
		UISwitch @switch = new()
		{
			On = IsOn
		};

		@switch.ValueChanged += (sender, e) =>
		{
			IsOn = @switch.On;
			Toggled?.Invoke(IsOn);
		};

		return @switch;
	}
}
