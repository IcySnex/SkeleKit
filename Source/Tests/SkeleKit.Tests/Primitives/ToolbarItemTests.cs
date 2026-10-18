using Xunit;

namespace SkeleKit.Tests.Primitives;

public class ToolbarItemTests
{
	[Fact]
	public void IsEnabled_DefaultsTrueAndNotifiesChanges()
	{
		ToolbarButton item = new();
		int changes = 0;
		item.Changed += _ => changes++;

		Assert.True(item.IsEnabled.Value);

		item.IsEnabled = false;

		Assert.False(item.IsEnabled.Value);
		Assert.Equal(1, changes);
	}

	[Fact]
	public void Command_ChangesNotify()
	{
		ToolbarButton item = new();
		int changes = 0;
		item.Changed += _ => changes++;

		item.Command = Command.From(() => { });

		Assert.Equal(1, changes);
	}

	[Fact]
	public void VisibilityPriority_DefaultsToStandardAndNotifiesChanges()
	{
		ToolbarButton item = new();
		ToolbarItem? sender = null;
		int changes = 0;
		item.Changed += changed =>
		{
			sender = changed;
			changes++;
		};

		Assert.Equal(ToolbarVisibilityPriority.Standard, item.VisibilityPriority);

		item.VisibilityPriority = ToolbarVisibilityPriority.High;

		Assert.Equal(ToolbarVisibilityPriority.High, item.VisibilityPriority);
		Assert.Equal(1, changes);
		Assert.Same(item, sender);
	}
}
