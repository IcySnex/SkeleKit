using System.Runtime.CompilerServices;

namespace BareUI;

// one level of a bound path: the property name to watch, and how to step to the next level
sealed record BindingSegment(
	string Name,
	Func<object, object?>? Step);

/// <summary>
/// A binding described by <c>Bind(...)</c>, not yet attached to a source. Assign it to a <see cref="Bindable{T}"/> property.
/// </summary>
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
	public static BindingExpression<T> Bind<TSource, T>(
		Func<TSource, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null)
		where TSource : class =>
		new(
			ParsePath(path),
			source => getter((TSource)source),
			null,
			BindingMode.OneWay);

	/// <summary>
	/// A two-way binding: <paramref name="setter"/> writes the control's value back to the source.
	/// </summary>
	public static BindingExpression<T> Bind<TSource, T>(
		Func<TSource, T> getter,
		Action<TSource, T?> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null)
		where TSource : class =>
		new(
			ParsePath(path),
			source => getter((TSource)source),
			(source, value) => setter((TSource)source, value),
			BindingMode.TwoWay);

	/// <summary>
	/// A one-way binding that converts the source value with <paramref name="format"/>.
	/// </summary>
	public static BindingExpression<T> Bind<TSource, TValue, T>(
		Func<TSource, TValue> getter,
		Func<TValue, T> format,
		[CallerArgumentExpression(nameof(getter))] string? path = null)
		where TSource : class =>
		new(
			ParsePath(path),
			source => format(getter((TSource)source)),
			null,
			BindingMode.OneWay);

	/// <summary>
	/// A two-way binding that converts both ways: <paramref name="format"/> out, <paramref name="parse"/> back in.
	/// </summary>
	public static BindingExpression<T> Bind<TSource, TValue, T>(
		Func<TSource, TValue> getter,
		Action<TSource, TValue> setter,
		Func<TValue, T> format,
		Func<T, TValue> parse,
		[CallerArgumentExpression(nameof(getter))] string? path = null)
		where TSource : class =>
		new(
			ParsePath(path),
			source => format(getter((TSource)source)),
			(source, value) => setter((TSource)source, parse(value!)),
			BindingMode.TwoWay);

	/// <summary>
	/// A nested one-way binding. Each segment is subscribed on its own, so replacing an intermediate re-resolves the rest.
	/// </summary>
	public static BindingExpression<T> BindPath<TSource, TMiddle, T>(
		Func<TSource, TMiddle?> first,
		Func<TMiddle, T> second,
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
			null,
			BindingMode.OneWay);

	/// <summary>
	/// A nested two-way binding; <paramref name="setter"/> runs against the resolved intermediate.
	/// </summary>
	public static BindingExpression<T> BindPath<TSource, TMiddle, T>(
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
			if (!IsIdentifier(name))
				throw new ArgumentException(
					$"Binding path '{expression}' must be plain member access (no calls, indexers or casts).");

		return names;
	}

	static bool IsIdentifier(
		string value)
	{
		if (value.Length == 0 || (!char.IsLetter(value[0]) && value[0] != '_'))
			return false;

		foreach (char character in value)
			if (!char.IsLetterOrDigit(character) && character != '_')
				return false;

		return true;
	}
}
