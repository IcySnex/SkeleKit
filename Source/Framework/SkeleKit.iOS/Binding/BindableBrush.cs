namespace SkeleKit;

/// <summary>
/// A background brush that takes a color, brush, or active <c>Bind(...)</c> expression.
/// </summary>
/// <remarks>
/// This is a reference type so assigning <c>null</c> continues to clear a background directly.
/// </remarks>
public sealed class BindableBrush
{
	static BindingExpression<Brush?> Widen<TBrush>(
		BindingExpression<TBrush?> expression) where TBrush : Brush =>
		new(expression.Segments, source => expression.Getter(source), null, expression.Mode, expression.Trigger, expression.Source);

	static BindingExpression<Brush?> Widen(
		BindingExpression<Color?> expression) =>
		new(
			expression.Segments,
			source => expression.Getter(source) is Color color ? (Brush)color : null,
			null,
			expression.Mode,
			expression.Trigger,
			expression.Source);

	static BindingExpression<Brush?> Widen(
		BindingExpression<Color> expression) =>
		new(
			expression.Segments,
			source => expression.Getter(source),
			null,
			expression.Mode,
			expression.Trigger,
			expression.Source);


	/// <summary>
	/// Wraps a solid color.
	/// </summary>
	public static implicit operator BindableBrush(
		Color color) =>
		new((Brush)color);

	/// <summary>
	/// Wraps an optional solid color.
	/// </summary>
	public static implicit operator BindableBrush(
		Color? color) =>
		new(color is Color value ? (Brush)value : null);

	/// <summary>
	/// Wraps a brush such as a gradient or material.
	/// </summary>
	public static implicit operator BindableBrush(
		Brush? brush) =>
		new(brush);

	/// <summary>
	/// Wraps an active color binding as a solid brush binding.
	/// </summary>
	public static implicit operator BindableBrush(
		BindingExpression<Color?> expression) =>
		new(Widen(expression));

	/// <summary>
	/// Wraps an active non-nullable color binding as a solid brush binding.
	/// </summary>
	public static implicit operator BindableBrush(
		BindingExpression<Color> expression) =>
		new(Widen(expression));

	/// <summary>
	/// Wraps an active brush binding.
	/// </summary>
	public static implicit operator BindableBrush(
		BindingExpression<Brush?> expression) =>
		new(expression);

	/// <summary>
	/// Wraps an active solid-brush binding.
	/// </summary>
	public static implicit operator BindableBrush(
		BindingExpression<SolidBrush?> expression) =>
		new(Widen(expression));

	/// <summary>
	/// Wraps an active gradient binding.
	/// </summary>
	public static implicit operator BindableBrush(
		BindingExpression<LinearGradient?> expression) =>
		new(Widen(expression));

	/// <summary>
	/// Wraps an active material binding.
	/// </summary>
	public static implicit operator BindableBrush(
		BindingExpression<Material?> expression) =>
		new(Widen(expression));


	internal BindableBrush(
		Brush? value)
	{
		Value = value;
		Expression = null;
	}

	BindableBrush(
		BindingExpression<Brush?> expression)
	{
		Value = null;
		Expression = expression;
	}


	/// <summary>
	/// The literal brush, or the last brush produced by a binding.
	/// </summary>
	public Brush? Value { get; }

	internal BindingExpression<Brush?>? Expression { get; }
}
