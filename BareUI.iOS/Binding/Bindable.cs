namespace BareUI;

/// <summary>
/// A control property that takes either a literal or a <c>Bind(...)</c> expression.
/// </summary>
public readonly struct Bindable<T>
{
	/// <summary>
	/// The literal value, or the last value a binding produced.
	/// </summary>
	public T? Value { get; }

	internal BindingExpression<T>? Expression { get; }

	/// <summary>
	/// Wraps a literal value. Needed for interface-typed properties: C# forbids an implicit conversion from an interface.
	/// </summary>
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


	public static implicit operator Bindable<T>(
		T value) =>
		new(value);

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
	public static Bindable<T> From<T>(
		T? value) =>
		new(value);
}
