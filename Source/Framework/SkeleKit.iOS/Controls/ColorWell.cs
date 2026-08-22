namespace SkeleKit;

/// <summary>
/// A swatch that opens the system color picker.
/// </summary>
public class ColorWell : Control
{
	UIColorWell Ui => (UIColorWell)Native;


	/// <summary>
	/// The picked color. Two-way: the picker writes it back as the user drags.
	/// </summary>
	public Bindable<Color> Selected
	{
		get => selected;
		set => selectedBinding = Register(selectedBinding, value, value => Set(ref selected, value, ApplySelected, affectsMeasure: false));
	}
	Color selected = Colors.Blue;
	Binding<Color>? selectedBinding;

	/// <summary>
	/// The title shown above the picker.
	/// </summary>
	public string? Title
	{
		get => title;
		set => Set(ref title, value, ApplyTitle, affectsMeasure: false);
	}
	string? title;

	/// <summary>
	/// Whether the picker offers an opacity slider.
	/// </summary>
	public bool SupportsAlpha
	{
		get => supportsAlpha;
		set => Set(ref supportsAlpha, value, ApplyAlpha, affectsMeasure: false);
	}
	bool supportsAlpha = true;

	/// <summary>
	/// Invoked with the new color whenever the user picks one.
	/// </summary>
	public Action<Color>? SelectionChanged { get; set; }


	void ApplySelected() =>
		Ui.SelectedColor = selected.ToUIColor();

	void ApplyTitle() =>
		Ui.Title = title;

	void ApplyAlpha() =>
		Ui.SupportsAlpha = supportsAlpha;

	void OnSelectionChanged()
	{
		if (Ui.SelectedColor is not UIColor picked)
			return;

		Color value = picked.ToColor();

		Set(ref selected, value, affectsMeasure: false);
		selectedBinding?.PushToSource(value);
		SelectionChanged?.Invoke(value);
	}


	private protected override UIView CreateNative()
	{
		UIColorWell well = new();
		well.ValueChanged += (_, _) => OnSelectionChanged();

		return well;
	}

	private protected override void ApplyProperties()
	{
		ApplySelected();
		ApplyTitle();
		ApplyAlpha();
	}
}
