using SkeleKit;
using Xunit;

namespace SkeleKit.Tests.Elements;

public sealed class ItemViewTests
{
	sealed class TestItem;

	sealed class TestItemView : ItemView<TestItem>
	{
		public TestItem? ChangedTo { get; private set; }


		protected override void OnItemChanged(
			TestItem? item) =>
			ChangedTo = item;
	}


	[Fact]
	public void Item_NotifiesRecycledView()
	{
		TestItem item = new();
		TestItemView view = new() { Item = item };

		Assert.Same(item, view.ChangedTo);
		Assert.Same(item, view.BindingContext);
	}
}
