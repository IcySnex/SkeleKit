using System.Runtime.CompilerServices;

namespace BareUI;

internal sealed record BindingSegment(
	string Name,
	Func<object, object?>? Step);

/// <summary>
/// A binding described by <c>Bind(...)</c>, not yet attached to a source. Assign it to a <see cref="Bindable{T}"/> property.
/// </summary>
/// <typeparam name="T">The data type produced by the binding expression.</typeparam>
public sealed class BindingExpression<T>
{
	internal BindingSegment[] Segments { get; }
	internal Func<object, T?> Getter { get; }
	internal Action<object, T?>? Setter { get; }
	internal BindingMode Mode { get; }
	internal UpdateTrigger Trigger { get; private set; } = UpdateTrigger.PropertyChanged;

	internal BindingExpression(
		BindingSegment[] segments,
		Func<object, T?> getter,
		Action<object, T?>? setter,
		BindingMode mode)
	{
		Segments = segments;
		Getter = getter;
		Setter = setter;
		Mode = mode;
	}

	/// <summary>
	/// Chooses when a two-way binding writes back to the source.
	/// </summary>
	/// <param name="trigger">The event condition that forces a source update.</param>
	/// <returns>The updated binding expression instance.</returns>
	public BindingExpression<T> On(
		UpdateTrigger trigger)
	{
		Trigger = trigger;
		return this;
	}
}

/// <summary>
/// Builds <see cref="BindingExpression{T}"/> values. Prefer the <c>Bind(...)</c> helper on <c>ContentView&lt;TViewModel&gt;</c>.
/// </summary>
public static class BindingFactory
{
	/// <summary>
	/// A one-way binding reading <paramref name="getter"/> from the source.
	/// </summary>
	/// <typeparam name="TSource">The data type of the source object.</typeparam>
	/// <typeparam name="T">The data type of the bound property.</typeparam>
	/// <param name="getter">The reader function to evaluate.</param>
	/// <param name="path">The automatically captured string expression of the lambda path.</param>
	/// <returns>A configured binding expression setup for one-way streaming.</returns>
	public static BindingExpression<T?> Bind<TSource, T>(
		Func<TSource, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) where TSource : class =>
		new(ParsePath(path), source => getter((TSource)source), null, BindingMode.OneWay);

	/// <summary>
	/// A two-way binding: <paramref name="setter"/> writes the control's value back to the source.
	/// </summary>
	/// <typeparam name="TSource">The data type of the source object.</typeparam>
	/// <typeparam name="T">The data type of the bound property.</typeparam>
	/// <param name="getter">The reader function to evaluate.</param>
	/// <param name="setter">The writer action to execute when updating the source.</param>
	/// <param name="path">The automatically captured string expression of the lambda path.</param>
	/// <returns>A configured binding expression setup for bi-directional streaming.</returns>
	public static BindingExpression<T?> Bind<TSource, T>(
		Func<TSource, T> getter,
		Action<TSource, T?> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) where TSource : class =>
		new(ParsePath(path), source => getter((TSource)source), (source, value) => setter((TSource)source, value), BindingMode.TwoWay);

	/// <summary>
	/// A one-way binding that converts the source value with <paramref name="format"/>.
	/// </summary>
	/// <typeparam name="TSource">The data type of the source object.</typeparam>
	/// <typeparam name="TValue">The intermediate value type resolved from the source.</typeparam>
	/// <typeparam name="T">The final output target data type.</typeparam>
	/// <param name="getter">The reader function to evaluate.</param>
	/// <param name="format">The mapping rule to transform the data.</param>
	/// <param name="path">The automatically captured string expression of the lambda path.</param>
	/// <returns>A configured binding expression featuring read-only formatting transformations.</returns>
	public static BindingExpression<T?> Bind<TSource, TValue, T>(
		Func<TSource, TValue> getter,
		Func<TValue, T> format,
		[CallerArgumentExpression(nameof(getter))] string? path = null) where TSource : class =>
		new(ParsePath(path), source => format(getter((TSource)source)), null, BindingMode.OneWay);

	/// <summary>
	/// A two-way binding that converts both ways: <paramref name="format"/> out, <paramref name="parse"/> back in.
	/// </summary>
	/// <typeparam name="TSource">The data type of the source object.</typeparam>
	/// <typeparam name="TValue">The intermediate value type resolved from the source.</typeparam>
	/// <typeparam name="T">The final output target data type.</typeparam>
	/// <param name="getter">The reader function to evaluate.</param>
	/// <param name="setter">The writer action to execute when updating the source.</param>
	/// <param name="format">The mapping rule to transform the data for presentation.</param>
	/// <param name="parse">The mapping rule to decode the presentation back into source values.</param>
	/// <param name="path">The automatically captured string expression of the lambda path.</param>
	/// <returns>A configured binding expression featuring bi-directional transformations.</returns>
	public static BindingExpression<T?> Bind<TSource, TValue, T>(
		Func<TSource, TValue> getter,
		Action<TSource, TValue> setter,
		Func<TValue, T> format,
		Func<T, TValue> parse,
		[CallerArgumentExpression(nameof(getter))] string? path = null) where TSource : class =>
		new(ParsePath(path), source => format(getter((TSource)source)), (source, value) => setter((TSource)source, parse(value!)), BindingMode.TwoWay);

	/// <summary>
	/// A control-to-source binding: the control writes to the source and never reads from it.
	/// </summary>
	/// <typeparam name="TSource">The data type of the source object.</typeparam>
	/// <typeparam name="T">The data type of the bound property.</typeparam>
	/// <param name="getter">The reader function to evaluate initial states.</param>
	/// <param name="setter">The writer action to execute when updating the source.</param>
	/// <param name="path">The automatically captured string expression of the lambda path.</param>
	/// <returns>A configured binding expression setup for source-only streaming updates.</returns>
	public static BindingExpression<T?> BindToSource<TSource, T>(
		Func<TSource, T> getter,
		Action<TSource, T?> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) where TSource : class =>
		new(ParsePath(path), source => getter((TSource)source), (source, value) => setter((TSource)source, value), BindingMode.OneWayToSource);

	/// <summary>
	/// A one-time binding: read once when the context attaches, then never again.
	/// </summary>
	/// <typeparam name="TSource">The data type of the source object.</typeparam>
	/// <typeparam name="T">The data type of the bound property.</typeparam>
	/// <param name="getter">The reader function to evaluate.</param>
	/// <param name="path">The automatically captured string expression of the lambda path.</param>
	/// <returns>A configured binding expression locked to a single assessment phase.</returns>
	public static BindingExpression<T?> BindOnce<TSource, T>(
		Func<TSource, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) where TSource : class =>
		new(ParsePath(path), source => getter((TSource)source), null, BindingMode.OneTime);

	/// <summary>
	/// A nested one-way binding. Each segment is subscribed on its own, so replacing an intermediate re-resolves the rest.
	/// </summary>
	/// <typeparam name="TSource">The data type of the base source object.</typeparam>
	/// <typeparam name="TMiddle">The data type of the bridge intermediate node.</typeparam>
	/// <typeparam name="T">The data type of the final bound property.</typeparam>
	/// <param name="first">The structural jump to the intermediate container.</param>
	/// <param name="second">The evaluation step within the intermediate target context.</param>
	/// <param name="firstPath">The automatically captured string expression of the first branch layer.</param>
	/// <param name="secondPath">The automatically captured string expression of the second branch layer.</param>
	/// <returns>A multi-tiered hierarchical tracking structure for one-way tracking.</returns>
	public static BindingExpression<T?> BindPath<TSource, TMiddle, T>(
		Func<TSource, TMiddle?> first,
		Func<TMiddle, T> second,
		[CallerArgumentExpression(nameof(first))] string? firstPath = null,
		[CallerArgumentExpression(nameof(second))] string? secondPath = null) where TSource : class where TMiddle : class =>
		new(
			[
				new(LeafName(firstPath), source => first((TSource)source)),
				new(LeafName(secondPath), null)
			],
			source => first((TSource)source) is { } middle ? second(middle) : default,
			null,
			BindingMode.OneWay);

	/// <summary>
	/// A nested two-way binding; <paramref name="setter"/> runs against the resolved intermediate.
	/// </summary>
	/// <typeparam name="TSource">The data type of the base source object.</typeparam>
	/// <typeparam name="TMiddle">The data type of the bridge intermediate node.</typeparam>
	/// <typeparam name="T">The data type of the final bound property.</typeparam>
	/// <param name="first">The structural jump to the intermediate container.</param>
	/// <param name="second">The evaluation step within the intermediate target context.</param>
	/// <param name="setter">The writeback mechanism operating on the targeted child node properties.</param>
	/// <param name="firstPath">The automatically captured string expression of the first branch layer.</param>
	/// <param name="secondPath">The automatically captured string expression of the second branch layer.</param>
	/// <returns>A multi-tiered hierarchical tracking structure for bi-directional tracking.</returns>
	public static BindingExpression<T?> BindPath<TSource, TMiddle, T>(
		Func<TSource, TMiddle?> first,
		Func<TMiddle, T> second,
		Action<TMiddle, T?> setter,
		[CallerArgumentExpression(nameof(first))] string? firstPath = null,
		[CallerArgumentExpression(nameof(second))] string? secondPath = null)
		where TSource : class
		where TMiddle : class =>
		new(
			[
				new(LeafName(firstPath), source => first((TSource)source)),
				new(LeafName(secondPath), null)
			],
			source => first((TSource)source) is { } middle ? second(middle) : default,
			(source, value) =>
			{
				if (first((TSource)source) is { } middle)
					setter(middle, value);
			},
			BindingMode.TwoWay);


	// "vm => vm.Movie.Title" -> ["Movie", "Title"]
	internal static BindingSegment[] ParsePath(
		string? expression)
	{
		string[] names = SplitPath(expression);
		BindingSegment[] segments = new BindingSegment[names.Length];

		for (int i = 0; i < names.Length; i++)
			segments[i] = new(names[i], null);

		return segments;
	}

	// last property name of a single-segment lambda
	static string LeafName(
		string? expression)
	{
		string[] names = SplitPath(expression);

		return names[^1];
	}

	static string[] SplitPath(
		string? expression)
	{
		if (expression is null)
			throw new ArgumentException("Binding path is missing. Pass a lambda so CallerArgumentExpression can capture it.");

		int arrow = expression.IndexOf("=>", StringComparison.Ordinal);
		if (arrow < 0)
			throw new ArgumentException($"Binding path '{expression}' is not a lambda.");

		string body = expression[(arrow + 2)..].Trim();
		string[] parts = body.Split('.');
		if (parts.Length < 2)
			throw new ArgumentException($"Binding path '{expression}' must access at least one property.");

		string[] names = parts[1..];
		foreach (string name in names)
		{
			if (!IsIdentifier(name))
				throw new ArgumentException($"Binding path '{expression}' must be plain member access (no calls, indexers or casts).");
		}

		return names;
	}

	static bool IsIdentifier(
		string value)
	{
		if (value.Length == 0 || !char.IsLetter(value[0]) && value[0] != '_')
			return false;

		foreach (char character in value)
		{
			if (!char.IsLetterOrDigit(character) && character != '_')
				return false;
		}

		return true;
	}
}
