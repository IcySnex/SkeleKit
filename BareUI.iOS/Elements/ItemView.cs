using System.Runtime.CompilerServices;

namespace BareUI;

/// <summary>
/// The element tree for one item in a <c>CollectionView</c>. Compose it into <see cref="Content"/> in the constructor and bind with <c>Bind(...)</c>.
/// </summary>
public abstract class ItemView<TItem> : Panel
	where TItem : class
{
	/// <summary>
	/// The item this cell shows. Swapped on reuse; the bindings re-fire.
	/// </summary>
	public TItem? Item
	{
		get => BindingContext is TItem item ? item : default;
		set => BindingContext = value;
	}

	/// <summary>
	/// The cell's element tree.
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
	/// Binds one way to an item property.
	/// </summary>
	protected static BindingExpression<T?> Bind<T>(
		Func<TItem, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, path);

	/// <summary>
	/// Binds one way through a converter.
	/// </summary>
	protected static BindingExpression<T?> Bind<TValue, T>(
		Func<TItem, TValue> getter,
		Func<TValue, T> format,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, format, path);
}
