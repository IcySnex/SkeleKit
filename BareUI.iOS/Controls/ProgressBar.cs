namespace BareUI;

/// <summary>
/// A progress bar.
/// </summary>
public class ProgressBar : Control
{
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
	/// The progress bar tint color, or null for the system default.
	/// </summary>
	public Bindable<Color?> Tint
	{
		get => tint;
		set => tintBinding = Register(tintBinding, value, value => Set(ref tint, value, ApplyTint, affectsMeasure: false));
	}
	Color? tint;
	Binding<Color?>? tintBinding;

	/// <summary>
	/// The unfilled track color, or null for the system default.
	/// </summary>
	public Color? TrackColor
	{
		get => trackColor;
		set => Set(ref trackColor, value, ApplyTint, affectsMeasure: false);
	}
	Color? trackColor;


	private protected override UIView CreateNative() =>
		new UIProgressView(UIProgressViewStyle.Default);

	private protected override void ApplyProperties()
	{
		ApplyProgress();
		ApplyTint();
	}

	UIProgressView Ui => (UIProgressView)Native;

	void ApplyProgress() =>
		Ui.Progress = (float)progress;

	void ApplyTint()
	{
		if (tint is { } color)
			Ui.ProgressTintColor = color.ToUIColor();

		if (trackColor is { } track)
			Ui.TrackTintColor = track.ToUIColor();
	}
}
