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

	[Fact]
	public void HighlightBackground_DefaultsToSystemGray()
	{
		TestItemView view = new();
		SolidBrush brush = Assert.IsType<SolidBrush>(view.HighlightBackground);

		Assert.Equal(Colors.Gray4, brush.Color);
	}

	[Fact]
	public void HighlightBackground_CanBeDisabled()
	{
		TestItemView view = new() { HighlightBackground = null };

		Assert.Null(view.HighlightBackground);
	}
}
