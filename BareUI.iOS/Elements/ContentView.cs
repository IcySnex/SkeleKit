using System.Runtime.CompilerServices;

namespace BareUI;

/// <summary>
/// A full screen: compose its tree into <see cref="Content"/> in the constructor.
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
	/// The page's element tree.
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

	/// <summary>
	/// Raised once the ViewModel is attached, after construction.
	/// </summary>
	protected virtual void OnViewModelAttached()
	{ }

	/// <summary>
	/// Raised after the page appears on screen.
	/// </summary>
	protected virtual void OnAppearing()
	{ }

	/// <summary>
	/// Raised as the page leaves the screen.
	/// </summary>
	protected virtual void OnDisappearing()
	{ }

	// the host controller and the registry drive these
	internal void NotifyAppearing() =>
		OnAppearing();

	internal void NotifyDisappearing() =>
		OnDisappearing();

	internal abstract void AttachViewModel(
		object viewModel);

	internal abstract Type ViewModelType { get; }

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
/// A page bound to a typed ViewModel: bind with <c>Bind(...)</c>.
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

	internal override Type ViewModelType =>
		typeof(TViewModel);

	internal override void AttachViewModel(
		object viewModel)
	{
		ViewModel = (TViewModel)viewModel;

		OnViewModelAttached();
	}


	/// <summary>
	/// Binds one way to a ViewModel property.
	/// </summary>
	protected static BindingExpression<T?> Bind<T>(
		Func<TViewModel, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, path);

	/// <summary>
	/// Binds two ways; <paramref name="setter"/> writes the control's value back.
	/// </summary>
	protected static BindingExpression<T?> Bind<T>(
		Func<TViewModel, T> getter,
		Action<TViewModel, T?> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, setter, path);

	/// <summary>
	/// Binds one way through a converter.
	/// </summary>
	protected static BindingExpression<T?> Bind<TValue, T>(
		Func<TViewModel, TValue> getter,
		Func<TValue, T> format,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, format, path);
}
