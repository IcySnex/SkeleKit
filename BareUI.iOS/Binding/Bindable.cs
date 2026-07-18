namespace BareUI;

/// <summary>
/// A control property that takes either a literal or a <c>Bind(...)</c> expression.
/// </summary>
/// <typeparam name="T">The underlying data type of the property.</typeparam>
public readonly struct Bindable<T>
{
	/// <summary>
	/// The literal value, or the last value a binding produced.
	/// </summary>
	public T? Value { get; }

	internal BindingExpression<T>? Expression { get; }

	/// <summary>
	/// Wraps a literal value. Needed for interface-typed properties.
	/// </summary>
	/// <param name="value">The literal value to encapsulate.</param>
	public Bindable(
		T? value)
	{
		Value = value;
		Expression = null;
	}

	Bindable(
		BindingExpression<T> expression)
	{
		Value = default;
		Expression = expression;
	}


	/// <summary>
	/// Creates a bindable container from a constant value.
	/// </summary>
	/// <param name="value">The raw value to wrap.</param>
	public static implicit operator Bindable<T>(
		T value) =>
		new(value);

	/// <summary>
	/// Creates a bindable container from an active binding expression.
	/// </summary>
	/// <param name="expression">The evaluation rule for the property.</param>
	public static implicit operator Bindable<T>(
		BindingExpression<T> expression) =>
		new(expression);
}

/// <summary>
/// Creates <see cref="Bindable{T}"/> values from literals.
/// </summary>
public static class Bindable
{
	/// <summary>
	/// Wraps a literal, for property types C# will not implicitly convert (interfaces).
	/// </summary>
	/// <typeparam name="T">The type of value being encapsulated.</typeparam>
	/// <param name="value">The raw value to wrap.</param>
	/// <returns>The wrapped literal.</returns>
	public static Bindable<T> From<T>(
		T? value) =>
		new(value);
}
