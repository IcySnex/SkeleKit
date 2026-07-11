using System.Runtime.CompilerServices;

namespace BareUI;

/// <summary>
/// A view built from a typed ViewModel: override <see cref="Build"/> and bind with <c>Bind(...)</c>.
/// </summary>
public abstract class ContentView<TViewModel> : Panel
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
	/// The element tree. Called once, the first time the view is measured or realized.
	/// </summary>
	protected abstract View Build();

	View? Content
	{
		get
		{
			if (Children.Count == 0)
				Children.Add(Build());

			return Children[0];
		}
	}


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
