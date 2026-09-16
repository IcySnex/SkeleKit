using Xunit;

namespace SkeleKit.Tests.App;

public class ThemeTests
{
	[Fact]
	public void Defaults_AreStable()
	{
		Theme theme = new();

		Assert.Null(theme.Tint);
		Assert.Equal(Appearance.System, theme.Appearance);
		Assert.Equal(ScrollEdgeStyle.Automatic, theme.TopScrollEdgeStyle);
		Assert.Equal(TitleStyle.Inline, theme.NavigationTitleStyle);
		Assert.Equal(TabBarMinimize.Never, theme.TabBarMinimize);
	}

	[Fact]
	public void ChangingValues_RaisesEachFieldOnce()
	{
		Theme theme = new();
		List<ThemeField> changed = [];
		theme.Changed += changed.Add;

		theme.Tint = Colors.Red;
		theme.NavigationTitleStyle = TitleStyle.Large;

		Assert.Equal([ThemeField.Tint, ThemeField.NavigationTitleStyle], changed);
	}

	[Fact]
	public void Builder_SetsValuesAndChains()
	{
		ThemeBuilder builder = new();

		ThemeBuilder chained = builder
			.Tint(Colors.Red)
			.Appearance(Appearance.Dark)
			.TopScrollEdgeStyle(ScrollEdgeStyle.Soft)
			.NavigationTitleStyle(TitleStyle.Large)
			.TabBarMinimize(TabBarMinimize.OnScrollDown);

		Assert.Same(builder, chained);
		Assert.Equal(Colors.Red, builder.Theme.Tint);
		Assert.Equal(Appearance.Dark, builder.Theme.Appearance);
		Assert.Equal(ScrollEdgeStyle.Soft, builder.Theme.TopScrollEdgeStyle);
		Assert.Equal(TitleStyle.Large, builder.Theme.NavigationTitleStyle);
		Assert.Equal(TabBarMinimize.OnScrollDown, builder.Theme.TabBarMinimize);
	}

	[Fact]
	public void SameValue_DoesNotRaise()
	{
		Theme theme = new();
		bool raised = false;
		theme.Changed += _ => raised = true;

		theme.Appearance = Appearance.System;

		Assert.False(raised);
	}
}
