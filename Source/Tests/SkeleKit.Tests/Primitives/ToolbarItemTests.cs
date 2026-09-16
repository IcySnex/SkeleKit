using Xunit;

namespace SkeleKit.Tests.Primitives;

public class ToolbarItemTests
{
	[Fact]
	public void VisibilityPriority_DefaultsToStandardAndNotifiesChanges()
	{
		ToolbarItem item = new();
		int changes = 0;
		item.Changed += () => changes++;

		Assert.Equal(ToolbarVisibilityPriority.Standard, item.VisibilityPriority);

		item.VisibilityPriority = ToolbarVisibilityPriority.High;

		Assert.Equal(ToolbarVisibilityPriority.High, item.VisibilityPriority);
		Assert.Equal(1, changes);
	}
}
