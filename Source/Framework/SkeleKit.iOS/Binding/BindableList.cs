using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace SkeleKit;

/// <summary>
/// A list source: any list literal, or a <c>Bind(...)</c> expression.
/// </summary>
/// <remarks>
/// Changes animate when the list is an <c>ObservableCollection</c>.
/// </remarks>
/// <typeparam name="TItem">The element type of the list.</typeparam>
[CollectionBuilder(typeof(BindableList), nameof(BindableList.Create))]
public readonly struct BindableList<TItem> : IReadOnlyList<TItem>
{
	static BindingExpression<IReadOnlyList<TItem>?> Widen<TList>(
		BindingExpression<TList?> expression) where TList : class, IReadOnlyList<TItem> =>
		new(expression.Segments, source => expression.Getter(source), null, expression.Mode, source: expression.Source);


	/// <summary>
	/// Wraps an array literal.
	/// </summary>
	/// <param name="value">The items to show.</param>
	/// <returns>A list source containing the array.</returns>
	public static implicit operator BindableList<TItem>(
		TItem[] value) =>
		new(value);

	/// <summary>
	/// Wraps an active binding to an array-typed source property.
	/// </summary>
	/// <param name="expression">The evaluation rule for the property.</param>
	/// <returns>A list source using the binding expression.</returns>
	public static implicit operator BindableList<TItem>(
		BindingExpression<TItem[]?> expression) =>
		new(Widen(expression));

	/// <summary>
	/// Wraps a list literal.
	/// </summary>
	/// <param name="value">The items to show.</param>
	/// <returns>A list source containing the list.</returns>
	public static implicit operator BindableList<TItem>(
		List<TItem> value) =>
		new(value);

	/// <summary>
	/// Wraps an observable collection, whose changes animate into place.
	/// </summary>
	/// <param name="value">The live items to show.</param>
	/// <returns>A live list source containing the collection.</returns>
	public static implicit operator BindableList<TItem>(
		ObservableCollection<TItem> value) =>
		new(value);

	/// <summary>
	/// Wraps an active binding to a list-typed source property.
	/// </summary>
	/// <param name="expression">The evaluation rule for the property.</param>
	/// <returns>A list source using the binding expression.</returns>
	public static implicit operator BindableList<TItem>(
		BindingExpression<IReadOnlyList<TItem>?> expression) =>
		new(expression);

	/// <summary>
	/// Wraps an active binding to a <c>List</c>-typed source property.
	/// </summary>
	/// <param name="expression">The evaluation rule for the property.</param>
	/// <returns>A list source using the binding expression.</returns>
	public static implicit operator BindableList<TItem>(
		BindingExpression<List<TItem>?> expression) =>
		new(Widen(expression));

	/// <summary>
	/// Wraps an active binding to an <c>ObservableCollection</c>-typed source property.
	/// </summary>
	/// <param name="expression">The evaluation rule for the property.</param>
	/// <returns>A live list source using the binding expression.</returns>
	public static implicit operator BindableList<TItem>(
		BindingExpression<ObservableCollection<TItem>?> expression) =>
		new(Widen(expression));

	/// <summary>
	/// Wraps an active binding to a virtualized list source.
	/// </summary>
	/// <param name="expression">The evaluation rule for the property.</param>
	/// <returns>A virtualized list source using the binding expression.</returns>
	public static implicit operator BindableList<TItem>(
		BindingExpression<IVirtualizedList<TItem>?> expression) =>
		new(Widen(expression));


	BindableList(
		BindingExpression<IReadOnlyList<TItem>?> expression)
	{
		Value = null;
		Expression = expression;
	}

	internal BindableList(
		IReadOnlyList<TItem>? value)
	{
		Value = value;
		Expression = null;
	}


	internal IReadOnlyList<TItem>? Value { get; }

	internal BindingExpression<IReadOnlyList<TItem>?>? Expression { get; }

	/// <summary>
	/// The number of current items. A binding that has not been evaluated yet is empty.
	/// </summary>
	public int Count => Value?.Count ?? 0;

	/// <summary>
	/// Gets a current item by index.
	/// </summary>
	public TItem this[int index] => (Value ?? [])[index];

	/// <summary>
	/// Adds an item when the wrapped source is mutable.
	/// </summary>
	/// <remarks>
	/// Bound and read-only sources cannot be mutated through this wrapper.
	/// </remarks>
	public void Add(
		TItem item)
	{
		if (Value is ICollection<TItem> collection)
		{
			collection.Add(item);
			return;
		}

		throw new NotSupportedException("The wrapped list is read-only or supplied by a binding.");
	}


	// lets collection expressions infer the element type; current items only, never a live source
	/// <summary>
	/// Enumerates the current list value.
	/// </summary>
	/// <returns>An enumerator over the current items.</returns>
	public IEnumerator<TItem> GetEnumerator() =>
		(Value ?? []).GetEnumerator();

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
		GetEnumerator();
}

/// <summary>
/// Builds <see cref="BindableList{TItem}"/> values from collections and virtualized sources.
/// </summary>
public static class BindableList
{
	/// <summary>
	/// Wraps a virtualized list for direct assignment to a list source property.
	/// </summary>
	/// <typeparam name="TItem">The element type of the list.</typeparam>
	/// <param name="items">The virtualized list.</param>
	/// <returns>A list source containing the virtualized list.</returns>
	public static BindableList<TItem> Virtualized<TItem>(
		IVirtualizedList<TItem> items)
	{
		ArgumentNullException.ThrowIfNull(items);
		return new(items);
	}

	/// <summary>
	/// Wraps the elements of a collection expression as a list source.
	/// </summary>
	/// <typeparam name="TItem">The element type of the list.</typeparam>
	/// <param name="items">The items to show.</param>
	/// <returns>A list source over a copy of the items.</returns>
	public static BindableList<TItem> Create<TItem>(
		ReadOnlySpan<TItem> items) =>
		new([.. items]);
}
