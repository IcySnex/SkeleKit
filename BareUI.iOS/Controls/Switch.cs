namespace BareUI;

/// <summary>
/// A binary on/off toggle.
/// </summary>
public class Switch : Control
{
	/// <summary>
	/// Whether the switch is on.
	/// </summary>
	public Bindable<bool> IsOn
	{
		get => isOn;
		set => isOnBinding = Register(isOnBinding, value, value => Set(ref isOn, value, ApplyIsOn, affectsMeasure: false));
	}
	bool isOn;
	Binding<bool>? isOnBinding;

	/// <summary>
	/// Invoked with the new value whenever the user toggles the switch.
	/// </summary>
	public Action<bool>? Toggled { get; set; }


	private protected override UIView CreateNative()
	{
		UISwitch @switch = new();
		@switch.ValueChanged += (_, _) => OnToggled();

		return @switch;
	}

	private protected override void ApplyProperties() =>
		ApplyIsOn();

	UISwitch Ui =>
		(UISwitch)Native;

	void ApplyIsOn() =>
		Ui.On = isOn;

	void OnToggled()
	{
		bool value = Ui.On;

		Set(ref isOn, value, affectsMeasure: false);
		isOnBinding?.PushToSource(value);
		Toggled?.Invoke(value);
	}
}
