namespace SkeleKit;

/// <summary>
/// The app's implicit styles.
/// </summary>
public sealed class Styles
{
	internal static Styles? Current { get; private set; }


	internal static void Use(
		Action<Styles> configure)
	{
		if (Current is not null)
			throw new InvalidOperationException("The app already has styles.");

		Styles styles = new();
		configure(styles);
		styles.frozen = true;

		Current = styles;
	}

	internal static void Reset() =>
		Current = null;

	internal static void ApplyTo(
		View view)
	{
		if (Current is not Styles styles)
			return;

		foreach (IStyle style in styles.Chain(view.GetType()))
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
		if (type.BaseType is Type baseType && typeof(View).IsAssignableFrom(baseType))
			Collect(baseType, collected);

		if (registered.TryGetValue(type, out List<IStyle>? styles))
			collected.AddRange(styles);
	}


	/// <summary>
	/// Registers a style applied to every view of its target type, including subtypes.
	/// </summary>
	/// <param name="style">The implicit style to register.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the styles have already been frozen and are in use.</exception>
	public Styles Add(
		IStyle style)
	{
		if (frozen)
			throw new InvalidOperationException("Styles cannot be changed once they are in use.");

		if (!registered.TryGetValue(style.TargetType, out List<IStyle>? styles))
			registered[style.TargetType] = styles = [];

		styles.Add(style);

		return this;
	}
}
