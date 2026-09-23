using System.Collections.ObjectModel;
using SkeleKit;
using Xunit;

namespace SkeleKit.Tests.Primitives;

public sealed class SelectionRemappingTests
{
	[Fact]
	public void ReadOnlySelection_RequiresStructuralChangeForSelectedItem()
	{
		object selected = new();
		ReadOnlyCollection<object> selection = new(new List<object> { selected });

		Assert.False(SelectionRemapping.CanReplace(selection, selected));
		Assert.True(SelectionRemapping.CanReplace(selection, new object()));
	}

	[Fact]
	public void MutableSelection_CanRemapSelectedItem()
	{
		object selected = new();
		List<object> selection = [selected];

		Assert.True(SelectionRemapping.CanReplace(selection, selected));
	}
}
