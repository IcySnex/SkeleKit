namespace SkeleKit;

/// <summary>
/// Chooses a strongly typed item template from an item's runtime type.
/// </summary>
/// <typeparam name="TItem">The common item type of the collection.</typeparam>
public sealed class ItemTemplateSelector<TItem>
	where TItem : class
{
	readonly Dictionary<Type, ItemTemplateRegistration<TItem>> templates = [];


	/// <summary>
	/// Registers the template used for one concrete item type.
	/// </summary>
	/// <typeparam name="TDerived">The runtime item type this template displays.</typeparam>
	/// <param name="template">Builds the element tree for a reusable cell.</param>
	/// <returns>This selector, for fluent registration.</returns>
	public ItemTemplateSelector<TItem> Add<TDerived>(
		Func<ItemView<TDerived>> template)
		where TDerived : class, TItem
	{
		ArgumentNullException.ThrowIfNull(template);

		Type type = typeof(TDerived);
		if (templates.ContainsKey(type))
			throw new ArgumentException($"An item template is already registered for {type.Name}.", nameof(template));

		templates.Add(type, ItemTemplateRegistration<TItem>.Create(templates.Count, template));
		return this;
	}


	internal IReadOnlyCollection<ItemTemplateRegistration<TItem>> Templates => templates.Values;

	internal ItemTemplateRegistration<TItem> Select(
		TItem item)
	{
		ArgumentNullException.ThrowIfNull(item);

		Type type = item.GetType();
		return templates.TryGetValue(type, out ItemTemplateRegistration<TItem>? template)
			? template
			: throw new InvalidOperationException(
				$"ItemTemplateSelector<{typeof(TItem).Name}> has no template for {type.Name}.");
	}
}

internal sealed class ItemTemplateRegistration<TItem>
	where TItem : class
{
	readonly Func<ICollectionItemView> build;


	ItemTemplateRegistration(
		Type itemType,
		string reuseIdentifier,
		Func<ICollectionItemView> build)
	{
		ItemType = itemType;
		ReuseIdentifier = reuseIdentifier;
		this.build = build;
	}


	internal Type ItemType { get; }

	internal string ReuseIdentifier { get; }


	internal static ItemTemplateRegistration<TItem> Create<TDerived>(
		int index,
		Func<ItemView<TDerived>> build)
		where TDerived : class, TItem =>
		new(
			typeof(TDerived),
			$"SkeleCell.Template.{index}",
			() => build()
				?? throw new InvalidOperationException($"The item template for {typeof(TDerived).Name} returned null."));

	internal static ItemTemplateRegistration<TItem> CreateDefault(
		string reuseIdentifier,
		Func<ItemView<TItem>> build) =>
		new(
			typeof(TItem),
			reuseIdentifier,
			() => build()
				?? throw new InvalidOperationException($"The item template for {typeof(TItem).Name} returned null."));

	internal ICollectionItemView Build() =>
		build();
}
