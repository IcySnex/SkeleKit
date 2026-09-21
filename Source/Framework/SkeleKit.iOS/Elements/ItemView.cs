using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace SkeleKit;

internal interface ICollectionItemView
{
	View View { get; }

	object? CurrentItem { get; }

	Brush? HighlightBackground { get; }

	IReadOnlyList<ItemAccessory> Accessories { get; }

	void ObserveHighlightBackground(Action<Brush?> changed);

	void ObserveAccessories(Action changed);

	void SetItem(object item);

	void SetContentInsets(
		double leading,
		double trailing);
}

/// <summary>
/// The element tree for one item in a <c>CollectionView</c>.
/// </summary>
/// <typeparam name="TItem">The item type the cell shows.</typeparam>
public abstract class ItemView<TItem> : ContentHost, ICollectionItemView, IItemAccessoryHost
	where TItem : class
{
	sealed class AccessoryCollection(
		ItemView<TItem> owner) : Collection<ItemAccessory>
	{
		protected override void InsertItem(
			int index,
			ItemAccessory item)
		{
			item.Attach(owner);
			base.InsertItem(index, item);
			owner.NotifyAccessoriesChanged();
		}

		protected override void SetItem(
			int index,
			ItemAccessory item)
		{
			if (ReferenceEquals(this[index], item))
				return;

			ItemAccessory previous = this[index];
			item.Attach(owner);
			base.SetItem(index, item);
			previous.Detach();
			owner.NotifyAccessoriesChanged();
		}

		protected override void RemoveItem(
			int index)
		{
			ItemAccessory item = this[index];
			base.RemoveItem(index);
			item.Detach();
			owner.NotifyAccessoriesChanged();
		}

		protected override void ClearItems()
		{
			ItemAccessory[] items = [.. this];
			base.ClearItems();

			foreach (ItemAccessory item in items)
				item.Detach();

			owner.NotifyAccessoriesChanged();
		}
	}


	readonly AccessoryCollection accessories;

	Action? accessoriesChanged;

	double contentInsetLeading;
	double contentInsetTrailing;


	protected ItemView() =>
		accessories = new(this);


	/// <summary>
	/// The native affordances pinned to this item's edges.
	/// </summary>
	/// <remarks>
	/// Configure the collection in the template constructor. Later changes update the cell in place.
	/// </remarks>
	public IList<ItemAccessory> Accessories => accessories;


	void NotifyAccessoriesChanged() =>
		accessoriesChanged?.Invoke();


	/// <summary>
	/// The background shown while the cell is pressed, and while selected when the collection retains the highlight. Null disables the highlight.
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

		// space the native accessories reserved, which the content keeps clear of
		double insets = contentInsetLeading + contentInsetTrailing;

		content.Measure(new(Math.Max(0, availableSize.Width - insets), availableSize.Height));
		Size desired = content.DesiredSize;

		return new(desired.Width + insets, desired.Height);
	}

	/// <inheritdoc/>
	protected override Size ArrangeOverride(
		Size finalSize)
	{
		double insets = contentInsetLeading + contentInsetTrailing;
		Content?.Arrange(new(contentInsetLeading, 0, Math.Max(0, finalSize.Width - insets), finalSize.Height));

		return finalSize;
	}


	void ICollectionItemView.SetContentInsets(
		double leading,
		double trailing)
	{
		if (contentInsetLeading == leading && contentInsetTrailing == trailing)
			return;

		contentInsetLeading = leading;
		contentInsetTrailing = trailing;
		InvalidateMeasure();
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
	object? ICollectionItemView.CurrentItem => Item;
	IReadOnlyList<ItemAccessory> ICollectionItemView.Accessories => accessories;
	Brush? ICollectionItemView.HighlightBackground => highlightBackground;
	void ICollectionItemView.ObserveHighlightBackground(
		Action<Brush?> changed) =>
		highlightBackgroundChanged = changed;
	void ICollectionItemView.ObserveAccessories(
		Action changed) =>
		accessoriesChanged = changed;

	void IItemAccessoryHost.Track(
		BindingBase binding) =>
		TrackBinding(binding);

	void IItemAccessoryHost.Untrack(
		BindingBase binding) =>
		UnregisterBinding(binding);

	void IItemAccessoryHost.NotifyChanged() =>
		NotifyAccessoriesChanged();

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
