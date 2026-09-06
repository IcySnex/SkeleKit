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
/// A typed binding whose observable path can still be extended or configured.
/// </summary>
/// <typeparam name="TSource">The root binding source type.</typeparam>
/// <typeparam name="TOwner">The object that owns the selected value.</typeparam>
/// <typeparam name="TValue">The selected source value type.</typeparam>
public sealed class BindingExpression<TSource, TOwner, TValue> : BindingExpression<TValue?>
	where TSource : class
	where TOwner : class?
{
	readonly Func<TSource, TValue> read;
	readonly Func<TSource, TOwner?> resolveOwner;
	readonly Func<object, object?>? pathStep;

	internal Func<TSource, TValue> Read => read;
	internal Func<object, object?> CreatePathStep() =>
		pathStep ?? (source => read((TSource)source));


	internal BindingExpression(
		BindingSegment[] segments,
		Func<TSource, TValue> read,
		Func<TSource, TOwner?> resolveOwner,
		Func<object, object?>? pathStep = null) : base(
			segments,
			source => read((TSource)source),
			null,
			BindingMode.OneWay)
	{
		this.read = read;
		this.resolveOwner = resolveOwner;
		this.pathStep = pathStep;
	}


	/// <summary>
	/// Reads once whenever a binding context attaches, without observing later changes.
	/// </summary>
	/// <returns>A one-time binding that can still convert its control value.</returns>
	public OneTimeBindingExpression<TSource, TValue> Once() =>
		new(Segments, read);

	/// <summary>
	/// Adds control-to-source updates to this binding.
	/// </summary>
	/// <param name="write">Writes the control value to the object that owns the selected property.</param>
	/// <returns>A two-way binding that can still convert its control value.</returns>
	public TwoWayBindingExpression<TSource, TOwner, TValue> TwoWay(
		Action<TOwner, TValue> write)
	{
		ArgumentNullException.ThrowIfNull(write);

		return new(Segments, read, resolveOwner, write);
	}

	/// <summary>
	/// Converts source values before applying them to the control.
	/// </summary>
	/// <typeparam name="TTarget">The control value type after conversion.</typeparam>
	/// <param name="convert">Converts source values to control values.</param>
	/// <returns>The converted one-way binding.</returns>
	public BindingExpression<TTarget?> ConvertTo<TTarget>(
		Func<TValue, TTarget> convert)
	{
		ArgumentNullException.ThrowIfNull(convert);

		return new(
			Segments,
			source => convert(read((TSource)source)),
			null,
			BindingMode.OneWay);
	}
}

/// <summary>
/// A one-time binding whose value can still be converted.
/// </summary>
/// <typeparam name="TSource">The root binding source type.</typeparam>
/// <typeparam name="TValue">The selected source value type.</typeparam>
public sealed class OneTimeBindingExpression<TSource, TValue> : BindingExpression<TValue?>
	where TSource : class
{
	readonly Func<TSource, TValue> read;

	internal OneTimeBindingExpression(
		BindingSegment[] segments,
		Func<TSource, TValue> read) : base(
			segments,
			source => read((TSource)source),
			null,
			BindingMode.OneTime)
	{
		this.read = read;
	}


	/// <summary>
	/// Converts the value before applying it to the control.
	/// </summary>
	/// <typeparam name="TTarget">The control value type after conversion.</typeparam>
	/// <param name="convert">Converts source values to control values.</param>
	/// <returns>The converted one-time binding.</returns>
	public BindingExpression<TTarget?> ConvertTo<TTarget>(
		Func<TValue, TTarget> convert)
	{
		ArgumentNullException.ThrowIfNull(convert);

		return new(
			Segments,
			source => convert(read((TSource)source)),
			null,
			BindingMode.OneTime);
	}
}

/// <summary>
/// A two-way binding whose value can still be converted.
/// </summary>
/// <typeparam name="TSource">The root binding source type.</typeparam>
/// <typeparam name="TOwner">The object that owns the selected value.</typeparam>
/// <typeparam name="TValue">The selected source value type.</typeparam>
public sealed class TwoWayBindingExpression<TSource, TOwner, TValue> : BindingExpression<TValue?>
	where TSource : class
	where TOwner : class?
{
	readonly Func<TSource, TValue> read;
	readonly Func<TSource, TOwner?> resolveOwner;
	readonly Action<TOwner, TValue> write;

	internal TwoWayBindingExpression(
		BindingSegment[] segments,
		Func<TSource, TValue> read,
		Func<TSource, TOwner?> resolveOwner,
		Action<TOwner, TValue> write) : base(
			segments,
			source => read((TSource)source),
			(source, value) =>
			{
				if (resolveOwner((TSource)source) is TOwner owner)
					write(owner, value!);
			},
			BindingMode.TwoWay)
	{
		this.read = read;
		this.resolveOwner = resolveOwner;
		this.write = write;
	}


	/// <summary>
	/// Converts source values before applying them to the control.
	/// </summary>
	/// <typeparam name="TTarget">The control value type after conversion.</typeparam>
	/// <param name="convert">Converts source values to control values.</param>
	/// <returns>An incomplete converted binding awaiting <c>ConvertFrom(...)</c>.</returns>
	public TwoWayConversionBuilder<TSource, TOwner, TValue, TTarget> ConvertTo<TTarget>(
		Func<TValue, TTarget> convert)
	{
		ArgumentNullException.ThrowIfNull(convert);

		return new(Segments, read, resolveOwner, write, convert);
	}

	/// <summary>
	/// Chooses when control changes are written to the source.
	/// </summary>
	/// <param name="trigger">When to write control changes.</param>
	/// <returns>The two-way binding with the selected trigger.</returns>
	public WritableBindingExpression<TValue?> UpdateOn(
		UpdateTrigger trigger) =>
		new(Segments, Getter, Setter!, Mode, trigger);
}

/// <summary>
/// A converted two-way binding awaiting its required control-to-source conversion.
/// </summary>
/// <typeparam name="TSource">The root binding source type.</typeparam>
/// <typeparam name="TOwner">The object that owns the selected value.</typeparam>
/// <typeparam name="TValue">The selected source value type.</typeparam>
/// <typeparam name="TTarget">The value type presented to the control.</typeparam>
public sealed class TwoWayConversionBuilder<TSource, TOwner, TValue, TTarget>
	where TSource : class
	where TOwner : class?
{
	readonly BindingSegment[] segments;
	readonly Func<object, TTarget?> getter;
	readonly Func<TSource, TOwner?> resolveOwner;
	readonly Action<TOwner, TValue> write;

	internal TwoWayConversionBuilder(
		BindingSegment[] segments,
		Func<TSource, TValue> read,
		Func<TSource, TOwner?> resolveOwner,
		Action<TOwner, TValue> write,
		Func<TValue, TTarget> convert)
	{
		this.segments = segments;
		getter = source => convert(read((TSource)source));
		this.resolveOwner = resolveOwner;
		this.write = write;
	}


	/// <summary>
	/// Converts control values back to source values and completes the two-way binding.
	/// </summary>
	/// <param name="convert">Converts nullable control values back to source values.</param>
	/// <returns>The completed writable binding.</returns>
	public WritableBindingExpression<TTarget?> ConvertFrom(
		Func<TTarget?, TValue> convert)
	{
		ArgumentNullException.ThrowIfNull(convert);

		return new(
			segments,
			getter,
			(source, value) =>
			{
				if (resolveOwner((TSource)source) is TOwner owner)
					write(owner, convert(value));
			},
			BindingMode.TwoWay);
	}
}

/// <summary>
/// Starts a binding that only writes control values to its root source.
/// </summary>
/// <typeparam name="TSource">The root binding source type.</typeparam>
public sealed class ToSourceBindingBuilder<TSource>
	where TSource : class
{
	internal ToSourceBindingBuilder()
	{ }


	/// <summary>
	/// Uses the control value only as input to the source.
	/// </summary>
	/// <typeparam name="TValue">The source value type.</typeparam>
	/// <param name="write">Writes the control value to the root source.</param>
	/// <returns>The source-only binding.</returns>
	public ToSourceBindingExpression<TSource, TValue> ToSource<TValue>(
		Action<TSource, TValue> write)
	{
		ArgumentNullException.ThrowIfNull(write);

		return new(write);
	}
}

/// <summary>
/// A source-only binding that can optionally convert the incoming control value.
/// </summary>
/// <typeparam name="TSource">The root binding source type.</typeparam>
/// <typeparam name="TValue">The source value type.</typeparam>
public sealed class ToSourceBindingExpression<TSource, TValue> : BindingExpression<TValue?>
	where TSource : class
{
	readonly Action<TSource, TValue> write;

	internal ToSourceBindingExpression(
		Action<TSource, TValue> write) : base(
			[],
			static _ => default,
			(source, value) => write((TSource)source, value!),
			BindingMode.OneWayToSource)
	{
		this.write = write;
	}


	/// <summary>
	/// Converts an incoming control value before writing it to the source.
	/// </summary>
	/// <typeparam name="TTarget">The control value type.</typeparam>
	/// <param name="convert">Converts nullable control values to source values.</param>
	/// <returns>The converted source-only binding.</returns>
	public WritableBindingExpression<TTarget?> ConvertFrom<TTarget>(
		Func<TTarget?, TValue> convert)
	{
		ArgumentNullException.ThrowIfNull(convert);

		return new(
			[],
			static _ => default,
			(source, value) => write((TSource)source, convert(value)),
			BindingMode.OneWayToSource);
	}

	/// <summary>
	/// Chooses when control changes are written to the source.
	/// </summary>
	/// <param name="trigger">When to write control changes.</param>
	/// <returns>The source-only binding with the selected trigger.</returns>
	public WritableBindingExpression<TValue?> UpdateOn(
		UpdateTrigger trigger) =>
		new(Segments, Getter, Setter!, Mode, trigger);
}

/// <summary>
/// A writable binding whose path, conversion, and mode are complete.
/// </summary>
/// <typeparam name="T">The value type presented to the control.</typeparam>
public sealed class WritableBindingExpression<T> : BindingExpression<T>
{
	internal WritableBindingExpression(
		BindingSegment[] segments,
		Func<object, T?> getter,
		Action<object, T?> setter,
		BindingMode mode,
		UpdateTrigger trigger = UpdateTrigger.PropertyChanged) : base(
			segments,
			getter,
			setter,
			mode,
			trigger)
	{ }


	/// <summary>
	/// Chooses when control changes are written to the source.
	/// </summary>
	/// <param name="trigger">When to write control changes.</param>
	/// <returns>The writable binding with the selected trigger.</returns>
	public WritableBindingExpression<T> UpdateOn(
		UpdateTrigger trigger) =>
		new(Segments, Getter, Setter!, Mode, trigger);
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
	/// <typeparam name="TOwner">The object that owns the current value.</typeparam>
	/// <typeparam name="TMiddle">The current reference type.</typeparam>
	/// <typeparam name="TNext">The property type returned by the next path segment.</typeparam>
	/// <param name="expression">The binding to continue.</param>
	/// <param name="next">Reads the next property from the current reference.</param>
	/// <param name="path">The path lambda, captured automatically to derive its property name.</param>
	/// <returns>A binding that observes the added path segment.</returns>
	public static BindingExpression<TSource, TMiddle, TNext> Path<TSource, TOwner, TMiddle, TNext>(
		this BindingExpression<TSource, TOwner, TMiddle> expression,
		Func<TMiddle, TNext> next,
		[CallerArgumentExpression(nameof(next))] string? path = null)
		where TSource : class
		where TOwner : class?
		where TMiddle : class?
	{
		ArgumentNullException.ThrowIfNull(expression);
		ArgumentNullException.ThrowIfNull(next);

		BindingSegment[] segments = [.. expression.Segments, new(BindingFactory.LeafName(path), null)];
		segments[^2] = segments[^2] with { Step = expression.CreatePathStep() };

		return new(
			segments,
			source => expression.Read(source) is TMiddle middle ? next(middle) : default!,
			source => expression.Read(source),
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
		if (!IsIdentifier(parts[0].TrimEnd('?', '!')))
			throw new ArgumentException($"Binding path '{expression}' must be plain member access (no calls, indexers or casts).");

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
	/// Starts a binding that can only write control values to a source.
	/// </summary>
	/// <typeparam name="TSource">The binding source type.</typeparam>
	/// <returns>A source-only binding builder.</returns>
	public static ToSourceBindingBuilder<TSource> Bind<TSource>()
		where TSource : class =>
		new();


	/// <summary>
	/// Starts a one-way binding that reads a source property.
	/// </summary>
	/// <typeparam name="TSource">The binding source type.</typeparam>
	/// <typeparam name="TValue">The property value type.</typeparam>
	/// <param name="read">The source property to read.</param>
	/// <param name="path">The captured source expression used to identify the property path.</param>
	/// <returns>A binding whose path can be extended or converted.</returns>
	public static BindingExpression<TSource, TSource, TValue> Bind<TSource, TValue>(
		Func<TSource, TValue> read,
		[CallerArgumentExpression(nameof(read))] string? path = null)
		where TSource : class =>
		new(ParsePath(path), read, source => source);
}
