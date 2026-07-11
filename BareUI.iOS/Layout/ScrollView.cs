namespace BareUI;

/// <summary>
/// A scrolling container for a single child, backed by <c>UIScrollView</c>.
/// </summary>
public partial class ScrollView : Panel
{
	/// <summary>
	/// The scroll axis.
	/// </summary>
	public Orientation Orientation { get; set; } = Orientation.Vertical;

	/// <summary>
	/// Whether the content is inset so the keyboard never covers the focused control. On by default.
	/// </summary>
	public bool AvoidsKeyboard { get; set; } = true;

	/// <summary>
	/// How dragging the scroll view dismisses the keyboard.
	/// </summary>
	public KeyboardDismiss KeyboardDismiss
	{
		get => keyboardDismiss;
		set => Set(ref keyboardDismiss, value, ApplyKeyboardDismiss, affectsMeasure: false);
	}
	KeyboardDismiss keyboardDismiss = KeyboardDismiss.Interactive;

	void ApplyKeyboardDismiss() =>
		ApplyKeyboardDismissCore();

	partial void ApplyKeyboardDismissCore();

	/// <summary>
	/// The single scrollable child.
	/// </summary>
	public View? Content
	{
		get => Children.Count > 0 ? Children[0] : null;
		set
		{
			Children.Clear();
			if (value is not null)
				Children.Add(value);
		}
	}


	protected override Size MeasureOverride(
		Size availableSize)
	{
		View? content = Content;
		if (content is null)
			return Size.Zero;

		bool vertical = Orientation == Orientation.Vertical;
		Size probe = vertical
			? new(availableSize.Width, double.PositiveInfinity)
			: new(double.PositiveInfinity, availableSize.Height);

		content.Measure(probe);
		Size desired = content.DesiredSize;

		// fill finite dimension, else size to content
		double width = vertical
			? Fill(availableSize.Width, desired.Width)
			: desired.Width;
		double height = vertical
			? desired.Height
			: Fill(availableSize.Height, desired.Height);

		return new(width, height);
	}

	// the engine lays the content out itself: UIKit only calls ScrollHost.LayoutSubviews when the
	// scroll view's own bounds change, which would leave content stale after a binding update
	protected override Size ArrangeOverride(
		Size finalSize)
	{
		ArrangeContent(finalSize);

		return finalSize;
	}

	partial void ArrangeContent(
		Size viewport);

	static double Fill(
		double available,
		double desired) =>
		double.IsFinite(available) ? available : desired;
}
