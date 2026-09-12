using SkeleKit;
using Xunit;

namespace SkeleKit.Tests.Elements;

public sealed class ItemTemplateSelectorTests
{
	abstract class Entry;

	sealed class NavigationEntry : Entry;

	sealed class ToggleEntry : Entry;

	sealed class NavigationView : ItemView<NavigationEntry>;

	sealed class ToggleView : ItemView<ToggleEntry>;


	[Fact]
	public void Select_BuildsStronglyTypedViewAndAssignsItem()
	{
		NavigationEntry item = new();
		ItemTemplateSelector<Entry> selector = new ItemTemplateSelector<Entry>()
			.Add<NavigationEntry>(static () => new NavigationView())
			.Add<ToggleEntry>(static () => new ToggleView());

		ItemTemplateRegistration<Entry> template = selector.Select(item);
		ICollectionItemView view = template.Build();
		view.SetItem(item);

		NavigationView navigation = Assert.IsType<NavigationView>(view.View);
		Assert.Same(item, navigation.Item);
		Assert.Same(item, navigation.BindingContext);
	}

	[Fact]
	public void Add_RejectsDuplicateRuntimeType()
	{
		ItemTemplateSelector<Entry> selector = new ItemTemplateSelector<Entry>()
			.Add<NavigationEntry>(static () => new NavigationView());

		ArgumentException error = Assert.Throws<ArgumentException>(() =>
			selector.Add<NavigationEntry>(static () => new NavigationView()));

		Assert.Contains(nameof(NavigationEntry), error.Message);
	}

	[Fact]
	public void Select_RejectsUnregisteredRuntimeType()
	{
		ItemTemplateSelector<Entry> selector = new ItemTemplateSelector<Entry>()
			.Add<NavigationEntry>(static () => new NavigationView());

		InvalidOperationException error = Assert.Throws<InvalidOperationException>(() =>
			selector.Select(new ToggleEntry()));

		Assert.Contains(nameof(ToggleEntry), error.Message);
	}
}
