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
		set
		{
			if (ReferenceEquals(Item, value))
				return;

			BindingContext = value;
			OnItemChanged(value);
		}
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


	/// <summary>
	/// Raised whenever this recycled view receives a different item.
	/// </summary>
	/// <param name="item">The item now represented by the view, or null when it is cleared.</param>
	protected virtual void OnItemChanged(
		TItem? item)
	{ }


	/// <inheritdoc/>
	protected override Size MeasureOverride(
		Size availableSize)
	{
		if (Content is not View content)
			return Size.Zero;

		content.Measure(availableSize);

		return content.DesiredSize;
	}

	/// <inheritdoc/>
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
	/// <param name="read">The item property to read.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<TItem, T, T> Bind<T>(
		Func<TItem, T> read,
		[CallerArgumentExpression(nameof(read))] string? path = null) =>
		BindingFactory.Bind(read, path);
}
