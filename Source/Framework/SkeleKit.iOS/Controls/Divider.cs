namespace SkeleKit;

/// <summary>
/// A hairline separator view.
/// </summary>
public class Divider : View
{
	/// <summary>
	/// The divider color, or null for the system separator color.
	/// </summary>
	public Bindable<Color?> Color
	{
		get => color;
		set => colorBinding = Register(colorBinding, value, value => Set(ref color, value, ApplyColor, affectsMeasure: false));
	}
	Color? color;
	Binding<Color?>? colorBinding;


	void ApplyColor() =>
		Native.BackgroundColor = color?.ToUIColor() ?? UIColor.Separator;


	private protected override UIView CreateNative() =>
		new();

	private protected override void ApplyProperties() =>
		ApplyColor();


	protected override Size MeasureOverride(
		Size availableSize) =>
		new(0, 1.0 / UIScreen.MainScreen.Scale);
}
