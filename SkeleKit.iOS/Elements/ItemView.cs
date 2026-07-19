using System.Runtime.CompilerServices;

namespace SkeleKit;

/// <summary>
/// The element tree for one item in a <c>CollectionView</c>.
/// </summary>
/// <typeparam name="TItem">The item type the cell shows.</typeparam>
public abstract class ItemView<TItem> : Panel
	where TItem : class
{
	/// <summary>
	/// The item this cell shows. Swapped on reuse; the bindings re-fire.
	/// </summary>
	public TItem? Item
	{
		get => BindingContext as TItem;
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
		if (Content is not View content)
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
	/// <typeparam name="T">The bound value type.</typeparam>
	/// <param name="getter">The item property to read.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<T?> Bind<T>(
		Func<TItem, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, path);

	/// <summary>
	/// Binds one way through a converter.
	/// </summary>
	/// <typeparam name="TValue">The value type read from the item.</typeparam>
	/// <typeparam name="T">The converted value type.</typeparam>
	/// <param name="getter">The item property to read.</param>
	/// <param name="format">Converts the item value for display.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<T?> Bind<TValue, T>(
		Func<TItem, TValue> getter,
		Func<TValue, T> format,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, format, path);
}
