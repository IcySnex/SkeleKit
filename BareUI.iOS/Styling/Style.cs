namespace BareUI;

/// <summary>
/// A named, reusable set of property setters for views of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The view type the style configures.</typeparam>
public sealed class Style<T> : IStyle
	where T : View
{
	readonly IStyle? basedOn;
	readonly Action<T> setters;

	/// <summary>
	/// Creates a style from a block of setters over the typed view.
	/// </summary>
	/// <param name="setters">The block of setters to run against the view.</param>
	public Style(
		Action<T> setters)
	{
		this.setters = setters;
	}

	/// <summary>
	/// Creates a style that runs <paramref name="basedOn"/> first, then its own setters over the top.
	/// </summary>
	/// <param name="basedOn">The base style to inherit from.</param>
	/// <param name="setters">The block of setters to run over the base style.</param>
	/// <exception cref="ArgumentException">Thrown if the base style target type is incompatible with <typeparamref name="T"/>.</exception>
	public Style(
		IStyle basedOn,
		Action<T> setters)
	{
		if (!basedOn.TargetType.IsAssignableFrom(typeof(T)))
			throw new ArgumentException($"A Style<{typeof(T).Name}> cannot be based on a style for {basedOn.TargetType.Name}.", nameof(basedOn));

		this.basedOn = basedOn;
		this.setters = setters;
	}


	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public Type TargetType =>
		typeof(T);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public void Apply(
		View view)
	{
		if (view is not T target)
			throw new InvalidOperationException($"A Style<{typeof(T).Name}> cannot be applied to a {view.GetType()?.Name}.");

		basedOn?.Apply(view);
		setters(target);
	}
}
