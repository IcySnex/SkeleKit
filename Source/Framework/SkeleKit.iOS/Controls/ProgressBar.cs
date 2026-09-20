namespace SkeleKit;

/// <summary>
/// A progress bar.
/// </summary>
public class ProgressBar : Control
{
	UIProgressView Ui => (UIProgressView)Native;


	/// <summary>
	/// The progress value from 0 (empty) to 1 (full).
	/// </summary>
	public Bindable<double> Progress
	{
		get => progress;
		set => progressBinding = Register(progressBinding, value, value => Set(ref progress, value, ApplyProgress, affectsMeasure: false));
	}
	double progress;
	Binding<double>? progressBinding;

	/// <summary>
	/// The filled track color, or null for the system default.
	/// </summary>
	public Bindable<Color?> FillColor
	{
		get => fillColor;
		set => fillColorBinding = Register(fillColorBinding, value, value => Set(ref fillColor, value, ApplyColors, affectsMeasure: false));
	}
	Color? fillColor;
	Binding<Color?>? fillColorBinding;

	/// <summary>
	/// The unfilled track color, or null for the system default.
	/// </summary>
	public Bindable<Color?> TrackColor
	{
		get => trackColor;
		set => trackColorBinding = Register(trackColorBinding, value, value => Set(ref trackColor, value, ApplyColors, affectsMeasure: false));
	}
	Color? trackColor;
	Binding<Color?>? trackColorBinding;


	void ApplyProgress() =>
		Ui.Progress = (float)progress;

	void ApplyColors()
	{
		Ui.ProgressTintColor = fillColor?.ToUIColor();
		Ui.TrackTintColor = trackColor?.ToUIColor();
	}


	private protected override UIView CreateNative() =>
		new UIProgressView(UIProgressViewStyle.Default);

	private protected override void ApplyProperties()
	{
		ApplyProgress();
		ApplyColors();
	}
}
