using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace BareUI;

/// <summary>
/// A list source: any list literal, or a <c>Bind(...)</c> expression.
/// </summary>
/// <remarks>
/// Changes animate when the list is an <c>ObservableCollection</c>.
/// </remarks>
/// <typeparam name="TItem">The element type of the list.</typeparam>
[CollectionBuilder(typeof(BindableList), nameof(BindableList.Create))]
public readonly struct BindableList<TItem>
{
	// C# forbids conversions from interface types, so the concrete lists convert

	/// <summary>
	/// Wraps an array literal.
	/// </summary>
	/// <param name="value">The items to show.</param>
	public static implicit operator BindableList<TItem>(
		TItem[] value) =>
		new(value);

	/// <summary>
	/// Wraps a list literal.
	/// </summary>
	/// <param name="value">The items to show.</param>
	public static implicit operator BindableList<TItem>(
		List<TItem> value) =>
		new(value);

	/// <summary>
	/// Wraps an observable collection, whose changes animate into place.
	/// </summary>
	/// <param name="value">The live items to show.</param>
	public static implicit operator BindableList<TItem>(
		ObservableCollection<TItem> value) =>
		new(value);

	/// <summary>
	/// Wraps an active binding to a list-typed source property.
	/// </summary>
	/// <param name="expression">The evaluation rule for the property.</param>
	public static implicit operator BindableList<TItem>(
		BindingExpression<IReadOnlyList<TItem>?> expression) =>
		new(expression);

	/// <summary>
	/// Wraps an active binding to a <c>List</c>-typed source property.
	/// </summary>
	/// <param name="expression">The evaluation rule for the property.</param>
	public static implicit operator BindableList<TItem>(
		BindingExpression<List<TItem>?> expression) =>
		new(Widen(expression));

	/// <summary>
	/// Wraps an active binding to an <c>ObservableCollection</c>-typed source property.
	/// </summary>
	/// <param name="expression">The evaluation rule for the property.</param>
	public static implicit operator BindableList<TItem>(
		BindingExpression<ObservableCollection<TItem>?> expression) =>
		new(Widen(expression));

	static BindingExpression<IReadOnlyList<TItem>?> Widen<TList>(
		BindingExpression<TList?> expression) where TList : class, IReadOnlyList<TItem> =>
		new(expression.Segments, source => expression.Getter(source), null, expression.Mode);


	internal BindableList(
		IReadOnlyList<TItem>? value)
	{
		Value = value;
		Expression = null;
	}

	BindableList(
		BindingExpression<IReadOnlyList<TItem>?> expression)
	{
		Value = null;
		Expression = expression;
	}


	internal IReadOnlyList<TItem>? Value { get; }

	internal BindingExpression<IReadOnlyList<TItem>?>? Expression { get; }


	// lets collection expressions infer the element type; current items only, never a live source
	public IEnumerator<TItem> GetEnumerator() =>
		(Value ?? []).GetEnumerator();
}

/// <summary>
/// Builds <see cref="BindableList{TItem}"/> values from collection expressions (<c>[a, b, c]</c>).
/// </summary>
public static class BindableList
{
	/// <summary>
	/// Wraps the elements of a collection expression as a list source.
	/// </summary>
	/// <typeparam name="TItem">The element type of the list.</typeparam>
	/// <param name="items">The items to show.</param>
	/// <returns>A list source over a copy of the items.</returns>
	public static BindableList<TItem> Create<TItem>(
		ReadOnlySpan<TItem> items) =>
		new(items.ToArray());
}
