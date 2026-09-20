using System.Runtime.CompilerServices;

namespace SkeleKit;

internal interface ICollectionItemView
{
	View View { get; }

	Brush? HighlightBackground { get; }

	void ObserveHighlightBackground(Action<Brush?> changed);

	void SetItem(object item);
}

/// <summary>
/// The element tree for one item in a <c>CollectionView</c>.
/// </summary>
/// <typeparam name="TItem">The item type the cell shows.</typeparam>
public abstract class ItemView<TItem> : ContentHost, ICollectionItemView
	where TItem : class
{
	/// <summary>
	/// The background shown while the cell is pressed or selected, or null for no highlight.
	/// </summary>
	public BindableBrush? HighlightBackground
	{
		get => highlightBackground is null ? null : new(highlightBackground);
		set => highlightBackgroundBinding = Register(
			highlightBackgroundBinding,
			value?.Expression,
			value?.Value,
			SetHighlightBackground);
	}
	Brush? highlightBackground = Colors.Gray4;
	Binding<Brush?>? highlightBackgroundBinding;
	Action<Brush?>? highlightBackgroundChanged;

	/// <summary>
	/// The item this cell shows, or null before its first assignment. Swapped by the owning collection on reuse; the bindings re-fire.
	/// </summary>
	public TItem? Item => BindingContext as TItem;

	internal void SetItem(
		TItem item)
	{
		ArgumentNullException.ThrowIfNull(item);

		if (ReferenceEquals(Item, item))
			return;

		BindingContext = item;
		OnItemChanged(item);
	}

	/// <summary>
	/// Raised whenever this recycled view receives a different item.
	/// </summary>
	/// <param name="item">The item now represented by the view.</param>
	protected virtual void OnItemChanged(
		TItem item)
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
	/// Starts a binding that can only write control values to the item.
	/// </summary>
	/// <returns>A source-only binding builder.</returns>
	protected static ToSourceBindingBuilder<TItem> Bind() =>
		BindingFactory.Bind<TItem>();


	/// <summary>
	/// Binds one way to an item property.
	/// </summary>
	/// <typeparam name="T">The bound value type.</typeparam>
	/// <param name="read">The item property to read.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<TItem, TItem, T> Bind<T>(
		Func<TItem, T> read,
		[CallerArgumentExpression(nameof(read))] string? path = null) =>
		BindingFactory.Bind(read, path);


	View ICollectionItemView.View => this;
	Brush? ICollectionItemView.HighlightBackground => highlightBackground;
	void ICollectionItemView.ObserveHighlightBackground(
		Action<Brush?> changed) =>
		highlightBackgroundChanged = changed;

	void SetHighlightBackground(
		Brush? value)
	{
		if (ReferenceEquals(highlightBackground, value))
			return;

		highlightBackground = value;
		highlightBackgroundChanged?.Invoke(value);
	}

	void ICollectionItemView.SetItem(
		object item)
	{
		ArgumentNullException.ThrowIfNull(item);

		if (item is not TItem typed)
			throw new ArgumentException($"An ItemView<{typeof(TItem).Name}> cannot display {item.GetType().Name}.", nameof(item));

		SetItem(typed);
	}
}
