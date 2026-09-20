namespace SkeleKit;

/// <summary>
/// A binary on/off toggle.
/// </summary>
public class Switch : Control
{
	UISwitch Ui => (UISwitch)Native;


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
	/// The fill color while on, or null for the inherited tint.
	/// </summary>
	public Bindable<Color?> OnColor
	{
		get => onColor;
		set => onColorBinding = Register(onColorBinding, value, value => Set(ref onColor, value, ApplyColors, affectsMeasure: false));
	}
	Color? onColor;
	Binding<Color?>? onColorBinding;

	/// <summary>
	/// The thumb color, or null for the system default.
	/// </summary>
	public Bindable<Color?> ThumbColor
	{
		get => thumbColor;
		set => thumbColorBinding = Register(thumbColorBinding, value, value => Set(ref thumbColor, value, ApplyColors, affectsMeasure: false));
	}
	Color? thumbColor;
	Binding<Color?>? thumbColorBinding;

	/// <summary>
	/// Invoked with the new value whenever the user toggles the switch.
	/// </summary>
	public Action<bool>? Toggled { get; set; }


	void ApplyIsOn() =>
		Ui.On = isOn;

	void ApplyColors()
	{
		// ignores the view tint, needs its own colors
		Ui.OnTintColor = (onColor ?? EffectiveTint)?.ToUIColor();
		Ui.ThumbTintColor = thumbColor?.ToUIColor();
	}

	void OnToggled()
	{
		bool value = Ui.On;

		Set(ref isOn, value, affectsMeasure: false);
		isOnBinding?.PushToSource(value);
		Toggled?.Invoke(value);
	}


	private protected override UIView CreateNative()
	{
		UISwitch @switch = new();
		@switch.ValueChanged += (_, _) => OnToggled();

		return @switch;
	}

	private protected override void ApplyProperties()
	{
		ApplyIsOn();
		ApplyColors();
	}


	internal override void TintChanged()
	{
		if (IsRealized)
			ApplyColors();
	}
}
