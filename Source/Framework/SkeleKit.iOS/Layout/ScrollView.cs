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
	public Orientation Orientation
	{
		get;
		set => Set(ref field, value, ApplyBehavior);
	} = Orientation.Vertical;

	/// <summary>
	/// Whether the content is inset so the keyboard never covers the focused control.
	/// </summary>
	public bool AvoidsKeyboard { get; set; } = true;

	/// <summary>
	/// Command invoked when the user pulls to refresh.
	/// </summary>
	/// <remarks>
	/// Setting it installs the refresh control.
	/// The <see cref="ICommand.CanExecute(object?)"/> controls whether the user can pull to refresh.
	/// </remarks>
	public ICommand? RefreshCommand
	{
		get => refreshCommand;
		set
		{
			if (ReferenceEquals(refreshCommand, value))
				return;

			refreshCommand = value;
			ApplyRefreshCommand();
		}
	}
	ICommand? refreshCommand;

	/// <summary>
	/// The parameter passed to <see cref="RefreshCommand"/>.
	/// </summary>
	public object? RefreshCommandParameter
	{
		get;
		set => Set(ref field, value, ApplyRefreshCommand, affectsMeasure: false);
	}

	/// <summary>
	/// Whether the refresh spinner is showing.
	/// </summary>
	/// <remarks>
	/// With a two-way binding, pulling sets it to true and the ViewModel sets it to false when done.
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

	void ApplyRefreshCommand() =>
		ApplyRefreshCommandCore();

	partial void ApplyKeyboardDismissCore();

	partial void ApplyBehaviorCore();

	partial void ApplyRefreshingCore();

	partial void ApplyRefreshCommandCore();

	partial void ArrangeContent(
		Size viewport);

	partial void ScrollToCore(
		double offset,
		bool animated);


	/// <inheritdoc/>
	protected override Size MeasureOverride(
		Size availableSize)
	{
		Size inner = availableSize.Deflate(Padding);
		View? content = Content;
		if (content is null)
			return new(Padding.Horizontal, Padding.Vertical);

		bool vertical = Orientation == Orientation.Vertical;
		Size probe = vertical
			? new(inner.Width, double.PositiveInfinity)
			: new(double.PositiveInfinity, inner.Height);

		content.Measure(probe);
		Size desired = content.DesiredSize;

		// fill finite dimension, else size to content
		double width = vertical
			? Fill(inner.Width, desired.Width)
			: desired.Width;
		double height = vertical
			? desired.Height
			: Fill(inner.Height, desired.Height);

		return new Size(width, height).Inflate(Padding);
	}

	/// <inheritdoc/>
	protected override Size ArrangeOverride(
		Size finalSize)
	{
		ArrangeContent(finalSize);

		return finalSize;
	}


	internal void OnRefreshTriggered()
	{
		object? parameter = RefreshCommandParameter;

		if (RefreshCommand is not ICommand command || !command.CanExecute(parameter))
		{
			Set(ref isRefreshing, false, affectsMeasure: false);
			isRefreshingBinding?.PushToSource(false);
			ApplyRefreshing();
			return;
		}

		Set(ref isRefreshing, true, affectsMeasure: false);
		isRefreshingBinding?.PushToSource(true);
		command.Execute(parameter);
	}


	/// <summary>
	/// Scrolls to an offset along the scroll axis, in points.
	/// </summary>
	/// <param name="offset">The target offset in points.</param>
	/// <param name="animated">Whether to animate the scroll.</param>
	public void ScrollTo(
		double offset,
		bool animated = true) =>
		ScrollToCore(offset, animated);
}
