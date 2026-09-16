using Xunit;

namespace SkeleKit.Tests.Styling;

public class StylesTests : IDisposable
{
	public StylesTests() =>
		Styles.Reset();

	public void Dispose() =>
		Styles.Reset();


	[Fact]
	public void Add_AppliesToEveryView()
	{
		Styles.Use(styles => styles.Add(new Style<StubStyled>(view => view.Opacity = 0.5)));

		Assert.Equal(0.5, new StubStyled().Opacity);
		Assert.Equal(0.5, new StubStyled().Opacity);
	}

	[Fact]
	public void Add_AppliesToSubtypes()
	{
		Styles.Use(styles => styles.Add(new Style<StubStyled>(view => view.CornerRadius = 12)));

		Assert.Equal(12, new StubStyledLeaf().CornerRadius);
	}

	[Fact]
	public void Add_LeavesOtherTypesAlone()
	{
		Styles.Use(styles => styles.Add(new Style<StubStyled>(view => view.CornerRadius = 12)));

		Assert.Equal(0, new StubOther().CornerRadius);
	}

	// base-most first: the most derived style is the last word
	[Fact]
	public void Chain_AppliesBaseTypeStylesFirst()
	{
		Styles.Use(styles => styles
			.Add(new Style<StubStyledLeaf>(view => view.Tag = "leaf"))
			.Add(new Style<StubStyled>(view => view.Tag = "base")));

		Assert.Equal("leaf", new StubStyledLeaf().Tag);
	}

	[Fact]
	public void Chain_AppliesStylesOfOneTypeInRegistrationOrder()
	{
		Styles.Use(styles => styles
			.Add(new Style<StubStyled>(view => view.Tag = "first"))
			.Add(new Style<StubStyled>(view => view.Tag = "second")));

		Assert.Equal("second", new StubStyled().Tag);
	}

	// the whole point of applying in the base ctor: anything the initializer writes still wins
	[Fact]
	public void LocalValue_BeatsStyles()
	{
		Styles.Use(styles => styles.Add(new Style<StubStyled>(view => view.Opacity = 0.5)));

		Assert.Equal(0.25, new StubStyled { Opacity = 0.25 }.Opacity);
	}

	[Fact]
	public void ExplicitStyle_BeatsStyles()
	{
		Styles.Use(styles => styles.Add(new Style<StubStyled>(view => view.Opacity = 0.5)));

		StubStyled view = new() { Style = new Style<StubStyled>(view => view.Opacity = 1) };

		Assert.Equal(1, view.Opacity);
	}

	[Fact]
	public void Use_Twice_Throws()
	{
		Styles.Use(_ => { });

		Assert.Throws<InvalidOperationException>(() => Styles.Use(_ => { }));
	}

	[Fact]
	public void Add_AfterUse_Throws()
	{
		Styles? registered = null;
		Styles.Use(styles => registered = styles);

		Assert.Throws<InvalidOperationException>(() => registered!.Add(new Style<StubStyled>(view => view.Opacity = 0.5)));
	}

	[Fact]
	public void NoStyles_LeavesDefaults()
	{
		Assert.Equal(1, new StubStyled().Opacity);
	}
}
