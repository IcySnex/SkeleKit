using SkeleKit.Tests.Binding;
using Xunit;

namespace SkeleKit.Tests.Primitives;

public class ToolbarBindingTests
{
	[Fact]
	public void Text_Binding_AttachesToContext()
	{
		MovieViewModel source = new() { Title = "Interstellar" };
		ToolbarButton item = new()
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title)
		};

		item.AttachBindings(source);

		Assert.Equal("Interstellar", item.Text.Value);

		source.Title = "Dune";

		Assert.Equal("Dune", item.Text.Value);
	}

	[Fact]
	public void IsVisible_Binding_TracksSourceChanges()
	{
		MovieViewModel source = new() { Enabled = true };
		ToolbarButton item = new()
		{
			IsVisible = BindingFactory.Bind((MovieViewModel vm) => vm.Enabled)
		};
		item.AttachBindings(source);

		Assert.True(item.IsVisible.Value);

		source.Enabled = false;

		Assert.False(item.IsVisible.Value);
	}

	[Fact]
	public void IsEnabled_Binding_TracksSourceChanges()
	{
		MovieViewModel source = new() { Enabled = true };
		ToolbarButton item = new()
		{
			IsEnabled = BindingFactory.Bind((MovieViewModel vm) => vm.Enabled)
		};
		item.AttachBindings(source);

		Assert.True(item.IsEnabled.Value);

		source.Enabled = false;

		Assert.False(item.IsEnabled.Value);
	}

	[Fact]
	public void Tint_Binding_TracksThemeChanges()
	{
		Theme theme = new() { Tint = Colors.Indigo };
		ToolbarButton item = new()
		{
			Tint = BindingFactory.Bind(theme, value => value.Tint)
		};

		Assert.Equal(Colors.Indigo, item.Tint.Value);

		theme.Tint = Colors.Pink;

		Assert.Equal(Colors.Pink, item.Tint.Value);
	}
}
