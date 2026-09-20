namespace SkeleKit;

/// <summary>
/// A row of dots marking the current page of a paging scroll or a carousel.
/// </summary>
public class PageControl : Control
{
	UIPageControl Ui => (UIPageControl)Native;


	/// <summary>
	/// How many dots are shown.
	/// </summary>
	public Bindable<int> Count
	{
		get => count;
		set => countBinding = Register(countBinding, value, value => Set(ref count, value, ApplyCount));
	}
	int count;
	Binding<int>? countBinding;

	/// <summary>
	/// The filled dot, updated when the user taps or scrubs the dots.
	/// </summary>
	public Bindable<int> Current
	{
		get => current;
		set => currentBinding = Register(currentBinding, value, value => Set(ref current, value, ApplyCurrent, affectsMeasure: false));
	}
	int current;
	Binding<int>? currentBinding;

	/// <summary>
	/// The color of the unfilled dots, or null for the system default.
	/// </summary>
	public Bindable<Color?> DotColor
	{
		get => dotColor;
		set => dotColorBinding = Register(dotColorBinding, value, value => Set(ref dotColor, value, ApplyColors, affectsMeasure: false));
	}
	Color? dotColor;
	Binding<Color?>? dotColorBinding;

	/// <summary>
	/// The color of the filled dot, or null for the system default.
	/// </summary>
	public Bindable<Color?> CurrentDotColor
	{
		get => currentDotColor;
		set => currentDotColorBinding = Register(currentDotColorBinding, value, value => Set(ref currentDotColor, value, ApplyColors, affectsMeasure: false));
	}
	Color? currentDotColor;
	Binding<Color?>? currentDotColorBinding;

	/// <summary>
	/// Whether the control hides itself while there is only one page.
	/// </summary>
	public bool HidesForSinglePage
	{
		get => hidesForSinglePage;
		set => Set(ref hidesForSinglePage, value, ApplyBehavior);
	}
	bool hidesForSinglePage = true;

	/// <summary>
	/// Whether dragging across the dots scrubs through the pages, rather than only tapping them.
	/// </summary>
	public bool AllowsScrubbing
	{
		get => allowsScrubbing;
		set => Set(ref allowsScrubbing, value, ApplyBehavior, affectsMeasure: false);
	}
	bool allowsScrubbing = true;

	/// <summary>
	/// Invoked with the new page whenever the user taps or scrubs the dots.
	/// </summary>
	public Action<int>? PageChanged { get; set; }


	void ApplyCount() =>
		Ui.Pages = count;

	void ApplyCurrent() =>
		Ui.CurrentPage = current;

	void ApplyColors()
	{
		Ui.PageIndicatorTintColor = dotColor?.ToUIColor();
		Ui.CurrentPageIndicatorTintColor = (currentDotColor ?? EffectiveTint)?.ToUIColor();
	}

	void ApplyBehavior()
	{
		Ui.HidesForSinglePage = hidesForSinglePage;
		Ui.AllowsContinuousInteraction = allowsScrubbing;
	}

	void OnPageChanged()
	{
		int value = (int)Ui.CurrentPage;

		Set(ref current, value, affectsMeasure: false);
		currentBinding?.PushToSource(value);
		PageChanged?.Invoke(value);
	}


	private protected override UIView CreateNative()
	{
		UIPageControl control = new();
		control.ValueChanged += (_, _) => OnPageChanged();

		return control;
	}

	private protected override void ApplyProperties()
	{
		ApplyCount();
		ApplyCurrent();
		ApplyColors();
		ApplyBehavior();
	}


	internal override void TintChanged()
	{
		if (IsRealized)
			ApplyColors();
	}
}
