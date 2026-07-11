using Xunit;

namespace BareUI.Tests.Primitives;

public class ImageSourceTests
{
	[Fact]
	public void Symbol_SetsKindAndValue()
	{
		ImageSource source = ImageSource.Symbol("star.fill");

		Assert.Equal(ImageSourceKind.Symbol, source.Kind);
		Assert.Equal("star.fill", source.Value);
	}

	[Fact]
	public void Bundle_SetsKindAndValue()
	{
		ImageSource source = ImageSource.Bundle("poster");

		Assert.Equal(ImageSourceKind.Bundle, source.Kind);
		Assert.Equal("poster", source.Value);
	}

	[Fact]
	public void Url_SetsKindAndValue()
	{
		ImageSource source = ImageSource.Url("https://example.com/a.png");

		Assert.Equal(ImageSourceKind.Url, source.Kind);
		Assert.Equal("https://example.com/a.png", source.Value);
	}

	[Theory]
	[InlineData("https://example.com/a.png")]
	[InlineData("http://example.com/a.png")]
	[InlineData("file:///tmp/a.png")]
	public void ImplicitConversion_WithScheme_IsUrl(
		string value)
	{
		ImageSource source = value;

		Assert.Equal(ImageSourceKind.Url, source.Kind);
		Assert.Equal(value, source.Value);
	}

	[Theory]
	[InlineData("star.fill")]
	[InlineData("poster")]
	[InlineData("chevron.left.forwardslash.chevron.right")]
	public void ImplicitConversion_WithoutScheme_IsAuto(
		string value)
	{
		ImageSource source = value;

		Assert.Equal(ImageSourceKind.Auto, source.Kind);
		Assert.Equal(value, source.Value);
	}

	[Fact]
	public void ImplicitConversion_UnknownScheme_IsStillUrl()
	{
		// The heuristic only looks for "://", so any scheme counts — not just http(s).
		ImageSource source = "weird://value";

		Assert.Equal(ImageSourceKind.Url, source.Kind);
	}
}
