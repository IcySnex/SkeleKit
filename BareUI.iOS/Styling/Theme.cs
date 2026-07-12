namespace BareUI;

/// <summary>
/// The app's implicit styles: every view gets the styles registered for its type and its base types.
/// </summary>
public sealed class Theme
{
	internal static Theme? Current { get; private set; }


	internal static void Use(
		Action<Theme> configure)
	{
		if (Current is not null)
			throw new InvalidOperationException("The app already has a theme.");

		Theme theme = new();
		configure(theme);
		theme.frozen = true;

		Current = theme;
	}

	internal static void Reset() =>
		Current = null;

	internal static void ApplyTo(
		View view)
	{
		if (Current is not { } theme)
			return;

		foreach (IStyle style in theme.Chain(view.GetType()))
			style.Apply(view);
	}


	readonly Dictionary<Type, List<IStyle>> registered = [];
	readonly Dictionary<Type, IStyle[]> chains = [];

	bool frozen;


	IStyle[] Chain(
		Type? type)
	{
		if (type is null)
			return [];

		if (chains.TryGetValue(type, out IStyle[]? chain))
			return chain;

		List<IStyle> collected = [];
		Collect(type, collected);

		return chains[type] = [.. collected];
	}

	void Collect(
		Type type,
		List<IStyle> collected)
	{
		if (type.BaseType is { } baseType && typeof(View).IsAssignableFrom(baseType))
			Collect(baseType, collected);

		if (registered.TryGetValue(type, out List<IStyle>? styles))
			collected.AddRange(styles);
	}


	/// <summary>
	/// Registers a style applied to every view of its target type, including subtypes.
	/// </summary>
	/// <param name="style">The implicit style to register.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the theme has already been frozen and is in use.</exception>
	public Theme Style(
		IStyle style)
	{
		if (frozen)
			throw new InvalidOperationException("A theme cannot be changed once it is in use.");

		if (!registered.TryGetValue(style.TargetType, out List<IStyle>? styles))
			registered[style.TargetType] = styles = [];

		styles.Add(style);

		return this;
	}
}
