using System.Windows.Input;

namespace SkeleKit;

/// <summary>
/// A scrolling container for a single child.
/// </summary>
public partial class ScrollView : Panel
{
	static double Fill(
		double available,
		double desired) =>
		double.IsFinite(available) ? available : desired;


	private protected override bool ClipsByDefault => true;

	internal override bool Scrolls => true;


	/// <summary>
	/// The scroll axis.
	/// </summary>
	public Orientation Orientation { get; set; } = Orientation.Vertical;

	/// <summary>
	/// Whether the content is inset so the keyboard never covers the focused control.
	/// </summary>
	public bool AvoidsKeyboard { get; set; } = true;

	/// <summary>
	/// Command invoked when the user pulls to refresh.
	/// </summary>
	/// <remarks>
	/// Setting it enables the refresh control.
	/// </remarks>
	public ICommand? RefreshCommand { get; set; }

	/// <summary>
	/// Whether the refresh spinner is showing.
	/// </summary>
	/// <remarks>
	/// Two-way: the pull sets it true, the ViewModel sets it false when done.
	/// </remarks>
	public Bindable<bool> IsRefreshing
	{
		get => isRefreshing;
		set => isRefreshingBinding = Register(isRefreshingBinding, value, value => Set(ref isRefreshing, value, ApplyRefreshing, affectsMeasure: false));
	}
	bool isRefreshing;
	Binding<bool>? isRefreshingBinding;

	/// <summary>
	/// Invoked as the view scrolls, with the offset in points.
	/// </summary>
	public Action<double>? Scrolled { get; set; }

	/// <summary>
	/// How dragging the scroll view dismisses the keyboard.
	/// </summary>
	public KeyboardDismiss KeyboardDismiss
	{
		get => keyboardDismiss;
		set => Set(ref keyboardDismiss, value, ApplyKeyboardDismiss, affectsMeasure: false);
	}
	KeyboardDismiss keyboardDismiss = KeyboardDismiss.Interactive;

	/// <summary>
	/// Whether scrolling snaps to whole viewport pages.
	/// </summary>
	public bool Paging
	{
		get;
		set => Set(ref field, value, ApplyBehavior, affectsMeasure: false);
	}

	/// <summary>
	/// Whether the scroll indicator is shown.
	/// </summary>
	public bool ShowsIndicator
	{
		get;
		set => Set(ref field, value, ApplyBehavior, affectsMeasure: false);
	} = true;

	/// <summary>
	/// The color of the scroll indicator.
	/// </summary>
	public IndicatorStyle IndicatorStyle
	{
		get;
		set => Set(ref field, value, ApplyBehavior, affectsMeasure: false);
	} = IndicatorStyle.Default;

	/// <summary>
	/// Insets the scroll indicator from the edges, or null to track the content insets.
	/// </summary>
	public Thickness? IndicatorInsets
	{
		get;
		set => Set(ref field, value, ApplyBehavior, affectsMeasure: false);
	}

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


	void ApplyKeyboardDismiss() =>
		ApplyKeyboardDismissCore();

	void ApplyBehavior() =>
		ApplyBehaviorCore();

	void ApplyRefreshing() =>
		ApplyRefreshingCore();

	partial void ApplyKeyboardDismissCore();

	partial void ApplyBehaviorCore();

	partial void ApplyRefreshingCore();

	partial void ArrangeContent(
		Size viewport);

	partial void ScrollToCore(
		double offset,
		bool animated);


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

	protected override Size ArrangeOverride(
		Size finalSize)
	{
		ArrangeContent(finalSize);

		return finalSize;
	}


	internal void OnRefreshTriggered()
	{
		Set(ref isRefreshing, true, affectsMeasure: false);
		isRefreshingBinding?.PushToSource(true);

		if (RefreshCommand is ICommand command && command.CanExecute(null))
			command.Execute(null);
	}


	/// <summary>
	/// Scrolls to an offset along the scroll axis, in points.
	/// </summary>
	public void ScrollTo(
		double offset,
		bool animated = true) =>
		ScrollToCore(offset, animated);
}
