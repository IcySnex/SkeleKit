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

	Bindable(
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
