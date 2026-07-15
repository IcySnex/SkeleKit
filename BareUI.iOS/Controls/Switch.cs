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
	/// The fill color while on, or null for the system green.
	/// </summary>
	public Color? OnColor
	{
		get => onColor;
		set => Set(ref onColor, value, ApplyColors, affectsMeasure: false);
	}
	Color? onColor;

	/// <summary>
	/// The thumb color, or null for the system default.
	/// </summary>
	public Color? ThumbColor
	{
		get => thumbColor;
		set => Set(ref thumbColor, value, ApplyColors, affectsMeasure: false);
	}
	Color? thumbColor;

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

	private protected override void ApplyProperties()
	{
		ApplyIsOn();
		ApplyColors();
	}

	UISwitch Ui =>
		(UISwitch)Native;

	void ApplyIsOn() =>
		Ui.On = isOn;

	internal override void TintChanged()
	{
		if (IsRealized)
			ApplyColors();
	}

	void ApplyColors()
	{
		// UISwitch paints its fill green whatever the view tint says
		if ((onColor ?? Tint) is { } on)
			Ui.OnTintColor = on.ToUIColor();

		if (thumbColor is { } thumb)
			Ui.ThumbTintColor = thumb.ToUIColor();
	}

	void OnToggled()
	{
		bool value = Ui.On;

		Set(ref isOn, value, affectsMeasure: false);
		isOnBinding?.PushToSource(value);
		Toggled?.Invoke(value);
	}
}
