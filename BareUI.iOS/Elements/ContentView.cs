using System.Runtime.CompilerServices;

namespace BareUI;

/// <summary>
/// A full screen: builds an element tree in <see cref="Build"/> and carries the page chrome.
/// </summary>
public abstract partial class ContentView : Panel
{
	/// <summary>
	/// The navigation bar title.
	/// </summary>
	public Bindable<string?> Title
	{
		get => title;
		set => titleBinding = Register(titleBinding, value, value => Set(ref title, value, ApplyTitle, affectsMeasure: false));
	}
	string? title;
	Binding<string?>? titleBinding;

	/// <summary>
	/// Which edges the page keeps clear of the safe area. Defaults to all of them.
	/// </summary>
	public SafeAreaEdges SafeAreaEdges { get; set; } = SafeAreaEdges.All;

	/// <summary>
	/// Raised after the tree is built and the page is on screen.
	/// </summary>
	protected virtual void OnAppearing()
	{ }

	/// <summary>
	/// Raised as the page leaves the screen.
	/// </summary>
	protected virtual void OnDisappearing()
	{ }

	/// <summary>
	/// The element tree. Called once, the first time the page is measured or realized.
	/// </summary>
	protected abstract View Build();

	// the host controller drives these
	internal void NotifyAppearing() =>
		OnAppearing();

	internal void NotifyDisappearing() =>
		OnDisappearing();

	internal View? Content
	{
		get
		{
			if (Children.Count == 0)
				Children.Add(Build());

			return Children[0];
		}
	}

	void ApplyTitle() =>
		ApplyTitleCore();

	partial void ApplyTitleCore();


	protected override Size MeasureOverride(
		Size availableSize)
	{
		if (Content is not { } content)
			return Size.Zero;

		content.Measure(availableSize);

		return content.DesiredSize;
	}

	protected override Size ArrangeOverride(
		Size finalSize)
	{
		Content?.Arrange(new(Point.Zero, finalSize));

		return finalSize;
	}
}

/// <summary>
/// A <see cref="ContentView"/> bound to a typed ViewModel: bind with <c>Bind(...)</c>.
/// </summary>
public abstract class ContentView<TViewModel> : ContentView
	where TViewModel : class
{
	/// <summary>
	/// The ViewModel bindings resolve against.
	/// </summary>
	public TViewModel? ViewModel
	{
		get => BindingContext as TViewModel;
		set => BindingContext = value;
	}

	/// <summary>
	/// Binds one way to a ViewModel property.
	/// </summary>
	protected static BindingExpression<T> Bind<T>(
		Func<TViewModel, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, path);

	/// <summary>
	/// Binds two ways; <paramref name="setter"/> writes the control's value back.
	/// </summary>
	protected static BindingExpression<T> Bind<T>(
		Func<TViewModel, T> getter,
		Action<TViewModel, T?> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, setter, path);

	/// <summary>
	/// Binds one way through a converter.
	/// </summary>
	protected static BindingExpression<T> Bind<TValue, T>(
		Func<TViewModel, TValue> getter,
		Func<TValue, T> format,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, format, path);
}
