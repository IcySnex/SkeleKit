namespace BareUI;

/// <summary>
/// A named, reusable set of property setters for views of type <typeparamref name="T"/>. Immutable once created.
/// </summary>
public sealed class Style<T> : IStyle
	where T : View
{
	readonly IStyle? basedOn;
	readonly Action<T> setters;

	/// <summary>
	/// Creates a style from a block of setters over the typed view.
	/// </summary>
	public Style(
		Action<T> setters)
	{
		this.setters = setters;
	}

	/// <summary>
	/// Creates a style that runs <paramref name="basedOn"/> first, then its own setters over the top.
	/// </summary>
	public Style(
		IStyle basedOn,
		Action<T> setters)
	{
		if (!basedOn.TargetType.IsAssignableFrom(typeof(T)))
			throw new ArgumentException($"A Style<{typeof(T).Name}> cannot be based on a style for {basedOn.TargetType.Name}.", nameof(basedOn));

		this.basedOn = basedOn;
		this.setters = setters;
	}


	/// <inheritdoc/>
	public Type TargetType =>
		typeof(T);

	/// <inheritdoc/>
	public void Apply(
		View view)
	{
		if (view is not T target)
			throw new InvalidOperationException($"A Style<{typeof(T).Name}> cannot be applied to a {view.GetType().Name}.");

		basedOn?.Apply(view);
		setters(target);
	}
}
