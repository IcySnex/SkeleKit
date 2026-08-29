using System.Runtime.CompilerServices;

namespace SkeleKit;

internal sealed record BindingSegment(
	string Name,
	Func<object, object?>? Step);

/// <summary>
/// A binding described by <c>Bind(...)</c>, not yet attached to a source.
/// </summary>
/// <remarks>
/// Assign it to a <see cref="Bindable{T}"/> property.
/// </remarks>
/// <typeparam name="T">The value type the binding produces.</typeparam>
public class BindingExpression<T>
{
	internal BindingSegment[] Segments { get; }
	internal Func<object, T?> Getter { get; }
	internal Action<object, T?>? Setter { get; }
	internal BindingMode Mode { get; }
	internal UpdateTrigger Trigger { get; }

	internal BindingExpression(
		BindingSegment[] segments,
		Func<object, T?> getter,
		Action<object, T?>? setter,
		BindingMode mode,
		UpdateTrigger trigger = UpdateTrigger.PropertyChanged)
	{
		Segments = segments;
		Getter = getter;
		Setter = setter;
		Mode = mode;
		Trigger = trigger;
	}
}

/// <summary>
/// A typed binding configured fluently before it attaches to a control.
/// </summary>
/// <typeparam name="TSource">The binding source type.</typeparam>
/// <typeparam name="TValue">The value type on the source.</typeparam>
/// <typeparam name="TTarget">The value type presented to the control.</typeparam>
public sealed class BindingExpression<TSource, TValue, TTarget> : BindingExpression<TTarget?>
	where TSource : class
{
	readonly Func<TSource, TValue> read;
	readonly Action<TSource, TValue?>? write;
	readonly Func<TValue, TTarget> convertTo;
	readonly Func<TTarget, TValue>? convertFrom;
	readonly bool canAppendPath;
	readonly Func<object, object?>? pathStep;

	internal Func<TSource, TValue> Read => read;
	internal bool CanAppendPath => canAppendPath && Mode is BindingMode.OneWay;
	internal Func<object, object?> CreatePathStep() =>
		pathStep ?? (source => read((TSource)source));


	internal BindingExpression(
		BindingSegment[] segments,
		Func<TSource, TValue> read,
		Action<TSource, TValue?>? write,
		Func<TValue, TTarget> convertTo,
		Func<TTarget, TValue>? convertFrom,
		BindingMode mode,
		UpdateTrigger trigger = UpdateTrigger.PropertyChanged,
		bool canAppendPath = false,
		Func<object, object?>? pathStep = null) : base(
			segments,
			source => convertTo(read((TSource)source)),
			write is null || convertFrom is null
				? null
				: (source, val) => write((TSource)source, convertFrom(val!)),
			mode,
			trigger)
	{
		this.read = read;
		this.write = write;
		this.convertTo = convertTo;
		this.convertFrom = convertFrom;
		this.canAppendPath = canAppendPath;
		this.pathStep = pathStep;
	}


	/// <summary>
	/// Reads once whenever a binding context attaches, without observing later changes.
	/// </summary>
	/// <returns>The one-time binding.</returns>
	public BindingExpression<TSource, TValue, TTarget> Once()
	{
		if (Mode is not BindingMode.OneWay)
			throw new InvalidOperationException("Once() can only configure a one-way binding.");

		return Copy(mode: BindingMode.OneTime);
	}

	/// <summary>
	/// Adds control-to-source updates to this binding.
	/// </summary>
	/// <param name="write">Writes the control value back to the source.</param>
	/// <returns>The two-way binding.</returns>
	public BindingExpression<TSource, TValue, TTarget> TwoWay(
		Action<TSource, TValue?> write)
	{
		ArgumentNullException.ThrowIfNull(write);

		if (Mode is not BindingMode.OneWay)
			throw new InvalidOperationException("TwoWay(...) can only configure a one-way binding.");

		return Copy(write: write, mode: BindingMode.TwoWay);
	}

	/// <summary>
	/// Uses this source property only as the destination for control changes.
	/// </summary>
	/// <param name="write">Writes the control value to the source.</param>
	/// <returns>The control-to-source binding.</returns>
	public BindingExpression<TSource, TValue, TTarget> ToSource(
		Action<TSource, TValue?> write)
	{
		ArgumentNullException.ThrowIfNull(write);

		if (Mode is not BindingMode.OneWay)
			throw new InvalidOperationException("ToSource(...) can only configure a one-way binding.");

		return Copy(write: write, mode: BindingMode.OneWayToSource);
	}

	/// <summary>
	/// Converts source values before applying them to the control.
	/// </summary>
	/// <typeparam name="TConverted">The control value type after conversion.</typeparam>
	/// <param name="converter">Converts source values to control values.</param>
	/// <returns>The converted binding.</returns>
	public BindingExpression<TSource, TValue, TConverted> ConvertTo<TConverted>(
		Func<TValue, TConverted> converter)
	{
		ArgumentNullException.ThrowIfNull(converter);

		return new(
			Segments,
			read,
			write,
			converter,
			null,
			Mode,
			Trigger,
			pathStep: pathStep);
	}

	/// <summary>
	/// Converts control values before writing them to the source.
	/// </summary>
	/// <param name="converter">Converts control values to source values.</param>
	/// <returns>The converted binding.</returns>
	public BindingExpression<TSource, TValue, TTarget> ConvertFrom(
		Func<TTarget, TValue> converter)
	{
		ArgumentNullException.ThrowIfNull(converter);

		if (write is null)
			throw new InvalidOperationException("ConvertFrom(...) needs TwoWay(...) or ToSource(...).");

		return Copy(convertFrom: converter);
	}

	/// <summary>
	/// Sets the control value type and converts it before writing to a source-only binding.
	/// </summary>
	/// <typeparam name="TConverted">The value type supplied by the control.</typeparam>
	/// <param name="converter">Converts control values to source values.</param>
	/// <returns>The converted source-only binding.</returns>
	public BindingExpression<TSource, TValue, TConverted> ConvertFrom<TConverted>(
		Func<TConverted, TValue> converter)
	{
		ArgumentNullException.ThrowIfNull(converter);

		if (Mode is not BindingMode.OneWayToSource || write is null)
			throw new InvalidOperationException("Changing the control value type with ConvertFrom(...) is only supported after ToSource(...).");

		return new(
			Segments,
			read,
			write,
			_ => default!,
			converter,
			Mode,
			Trigger,
			pathStep: pathStep);
	}

	/// <summary>
	/// Chooses when control changes are written to the source.
	/// </summary>
	/// <param name="trigger">When to write the control value back.</param>
	/// <returns>The binding with the selected update trigger.</returns>
	public BindingExpression<TSource, TValue, TTarget> UpdateOn(
		UpdateTrigger trigger)
	{
		if (write is null)
			throw new InvalidOperationException("UpdateOn(...) needs TwoWay(...) or ToSource(...).");

		return Copy(trigger: trigger);
	}


	BindingExpression<TSource, TValue, TTarget> Copy(
		Action<TSource, TValue?>? write = null,
		Func<TTarget, TValue>? convertFrom = null,
		BindingMode? mode = null,
		UpdateTrigger? trigger = null) =>
		new(
			Segments,
			read,
			write ?? this.write,
			convertTo,
			convertFrom ?? this.convertFrom,
			mode ?? Mode,
			trigger ?? Trigger,
			canAppendPath,
			pathStep);
}

/// <summary>
/// Adds observable intermediate objects to typed binding paths.
/// </summary>
public static class BindingExpressionPathExtensions
{
	/// <summary>
	/// Continues a binding through an observable reference and observes the returned property.
	/// </summary>
	/// <typeparam name="TSource">The root binding source type.</typeparam>
	/// <typeparam name="TMiddle">The current reference type.</typeparam>
	/// <typeparam name="TNext">The property type returned by the next path segment.</typeparam>
	/// <param name="expression">The binding to continue.</param>
	/// <param name="next">Reads the next property from the current reference.</param>
	/// <param name="path">The path lambda, captured automatically to derive its property name.</param>
	/// <returns>A binding that observes the added path segment.</returns>
	public static BindingExpression<TSource, TNext?, TNext?> Path<TSource, TMiddle, TNext>(
		this BindingExpression<TSource, TMiddle, TMiddle> expression,
		Func<TMiddle, TNext> next,
		[CallerArgumentExpression(nameof(next))] string? path = null)
		where TSource : class
	{
		ArgumentNullException.ThrowIfNull(expression);
		ArgumentNullException.ThrowIfNull(next);

		if (!expression.CanAppendPath)
			throw new InvalidOperationException("Path(...) must come before binding modes and converters.");
		if (typeof(TMiddle).IsValueType)
			throw new InvalidOperationException("Path(...) requires a reference-valued intermediate object.");

		BindingSegment[] segments = [.. expression.Segments, new(BindingFactory.LeafName(path), null)];
		segments[^2] = segments[^2] with { Step = expression.CreatePathStep() };

		return new(
			segments,
			source => expression.Read(source) is TMiddle middle ? next(middle) : default,
			null,
			static val => val,
			static val => val,
			BindingMode.OneWay,
			canAppendPath: true,
			pathStep: source => next((TMiddle)source));
	}
}

/// <summary>
/// Builds binding expressions for typed sources.
/// </summary>
/// <remarks>
/// Prefer the <c>Bind(...)</c> helper on <c>ContentView&lt;TViewModel&gt;</c>.
/// </remarks>
public static class BindingFactory
{
	internal static string LeafName(
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

	internal static BindingSegment[] ParsePath(
		string? expression)
	{
		string[] names = SplitPath(expression);
		BindingSegment[] segments = new BindingSegment[names.Length];

		for (int i = 0; i < names.Length; i++)
			segments[i] = new(names[i], null);

		return segments;
	}


	/// <summary>
	/// Starts a one-way binding that reads a source property.
	/// </summary>
	public static BindingExpression<TSource, T, T> Bind<TSource, T>(
		Func<TSource, T> read,
		[CallerArgumentExpression(nameof(read))] string? path = null) where TSource : class =>
		new(
			ParsePath(path),
			read,
			null,
			static val => val,
			static val => val,
			BindingMode.OneWay,
			canAppendPath: true);
}
