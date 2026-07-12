using System.Runtime.CompilerServices;

namespace BareUI;

/// <summary>
/// The element tree for one item in a <c>CollectionView</c>.
/// </summary>
/// <typeparam name="TItem">The structural data type backing the item container context.</typeparam>
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
	/// <typeparam name="T">The underlying type of the bound target element.</typeparam>
	/// <param name="getter">The property selection expression.</param>
	/// <param name="path">The automatically captured string representation of the expression property.</param>
	/// <returns>A one-way tracking data stream context configuration.</returns>
	protected static BindingExpression<T?> Bind<T>(
		Func<TItem, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, path);

	/// <summary>
	/// Binds one way through a converter.
	/// </summary>
	/// <typeparam name="TValue">The intermediate value node processed from the state tier.</typeparam>
	/// <typeparam name="T">The targeted structural output presentation type.</typeparam>
	/// <param name="getter">The property selection expression.</param>
	/// <param name="format">The mapper rule converting source parameters to targets.</param>
	/// <param name="path">The automatically captured string representation of the expression property.</param>
	/// <returns>A converted one-way tracking data stream context configuration.</returns>
	protected static BindingExpression<T?> Bind<TValue, T>(
		Func<TItem, TValue> getter,
		Func<TValue, T> format,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, format, path);
}
