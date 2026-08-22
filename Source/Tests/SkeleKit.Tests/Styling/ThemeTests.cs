using Xunit;

namespace SkeleKit.Tests.Styling;

public class ThemeTests : IDisposable
{
	public ThemeTests() =>
		Theme.Reset();

	public void Dispose() =>
		Theme.Reset();


	[Fact]
	public void Style_AppliesToEveryView()
	{
		Theme.Use(theme => theme.Style(new Style<StubStyled>(view => view.Opacity = 0.5)));

		Assert.Equal(0.5, new StubStyled().Opacity);
		Assert.Equal(0.5, new StubStyled().Opacity);
	}

	[Fact]
	public void Style_AppliesToSubtypes()
	{
		Theme.Use(theme => theme.Style(new Style<StubStyled>(view => view.CornerRadius = 12)));

		Assert.Equal(12, new StubStyledLeaf().CornerRadius);
	}

	[Fact]
	public void Style_LeavesOtherTypesAlone()
	{
		Theme.Use(theme => theme.Style(new Style<StubStyled>(view => view.CornerRadius = 12)));

		Assert.Equal(0, new StubOther().CornerRadius);
	}

	// base-most first: the most derived style is the last word
	[Fact]
	public void Chain_AppliesBaseTypeStylesFirst()
	{
		Theme.Use(theme => theme
			.Style(new Style<StubStyledLeaf>(view => view.Tag = "leaf"))
			.Style(new Style<StubStyled>(view => view.Tag = "base")));

		Assert.Equal("leaf", new StubStyledLeaf().Tag);
	}

	[Fact]
	public void Chain_AppliesStylesOfOneTypeInRegistrationOrder()
	{
		Theme.Use(theme => theme
			.Style(new Style<StubStyled>(view => view.Tag = "first"))
			.Style(new Style<StubStyled>(view => view.Tag = "second")));

		Assert.Equal("second", new StubStyled().Tag);
	}

	// the whole point of applying in the base ctor: anything the initializer writes still wins
	[Fact]
	public void LocalValue_BeatsTheme()
	{
		Theme.Use(theme => theme.Style(new Style<StubStyled>(view => view.Opacity = 0.5)));

		Assert.Equal(0.25, new StubStyled { Opacity = 0.25 }.Opacity);
	}

	[Fact]
	public void ExplicitStyle_BeatsTheme()
	{
		Theme.Use(theme => theme.Style(new Style<StubStyled>(view => view.Opacity = 0.5)));

		StubStyled view = new() { Style = new Style<StubStyled>(view => view.Opacity = 1) };

		Assert.Equal(1, view.Opacity);
	}

	[Fact]
	public void Use_Twice_Throws()
	{
		Theme.Use(_ => { });

		Assert.Throws<InvalidOperationException>(() => Theme.Use(_ => { }));
	}

	[Fact]
	public void Style_AfterUse_Throws()
	{
		Theme? registered = null;
		Theme.Use(theme => registered = theme);

		Assert.Throws<InvalidOperationException>(() => registered!.Style(new Style<StubStyled>(view => view.Opacity = 0.5)));
	}

	[Fact]
	public void NoTheme_LeavesDefaults()
	{
		Assert.Equal(1, new StubStyled().Opacity);
	}
}
