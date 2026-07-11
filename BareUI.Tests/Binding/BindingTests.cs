using Xunit;

namespace BareUI.Tests.Binding;

public class BindingTests
{
	[Fact]
	public void OneWay_AppliesValueOnAttach()
	{
		MovieViewModel viewModel = new() { Title = "Interstellar" };
		StubBound view = new() { Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title) };

		view.BindingContext = viewModel;

		Assert.Equal("Interstellar", view.Current);
	}

	[Fact]
	public void OneWay_TracksSourceChanges()
	{
		MovieViewModel viewModel = new() { Title = "Interstellar" };
		StubBound view = new() { Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title) };
		view.BindingContext = viewModel;

		viewModel.Title = "Dune";

		Assert.Equal("Dune", view.Current);
	}

	[Fact]
	public void OneWay_IgnoresUnrelatedProperty()
	{
		MovieViewModel viewModel = new() { Title = "Interstellar" };
		StubBound view = new() { Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title) };
		view.BindingContext = viewModel;

		viewModel.Minutes = 169;

		Assert.Equal("Interstellar", view.Current);
	}

	[Fact]
	public void TwoWay_RoundTrips()
	{
		MovieViewModel viewModel = new() { Title = "Interstellar" };
		StubBound view = new()
		{
			Text = BindingFactory.Bind(
				(MovieViewModel vm) => vm.Title,
				(vm, value) => vm.Title = value ?? "")
		};
		view.BindingContext = viewModel;

		view.SimulateEdit("Dune");
		Assert.Equal("Dune", viewModel.Title);

		viewModel.Title = "Arrival";
		Assert.Equal("Arrival", view.Current);
	}

	[Fact]
	public void OneWay_DoesNotPushToSource()
	{
		MovieViewModel viewModel = new() { Title = "Interstellar" };
		StubBound view = new() { Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title) };
		view.BindingContext = viewModel;

		view.SimulateEdit("Dune");

		Assert.Equal("Interstellar", viewModel.Title);
	}

	[Fact]
	public void Converter_FormatsValue()
	{
		MovieViewModel viewModel = new() { Minutes = 169 };
		StubBound view = new()
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Minutes, minutes => $"{minutes} min")
		};

		view.BindingContext = viewModel;

		Assert.Equal("169 min", view.Current);
	}

	[Fact]
	public void Literal_AppliesWithoutContext()
	{
		StubBound view = new() { Text = "literal" };

		Assert.Equal("literal", view.Current);
	}

	[Fact]
	public void BindingContext_InheritsFromParent()
	{
		MovieViewModel viewModel = new() { Title = "Interstellar" };
		StubBound leaf = new() { Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title) };
		VStack root = new() { Children = { leaf } };

		root.BindingContext = viewModel;

		Assert.Equal("Interstellar", leaf.Current);
	}

	[Fact]
	public void BindingContext_ReplacedSourceRebinds()
	{
		StubBound view = new() { Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title) };
		view.BindingContext = new MovieViewModel { Title = "Interstellar" };

		MovieViewModel next = new() { Title = "Dune" };
		view.BindingContext = next;

		Assert.Equal("Dune", view.Current);

		next.Title = "Arrival";
		Assert.Equal("Arrival", view.Current);
	}

	[Fact]
	public void BindPath_ResubscribesWhenIntermediateReplaced()
	{
		MovieViewModel viewModel = new() { Movie = new() { Name = "Interstellar" } };
		StubBound view = new()
		{
			Text = BindingFactory.BindPath((MovieViewModel vm) => vm.Movie, movie => movie.Name)
		};
		view.BindingContext = viewModel;

		Assert.Equal("Interstellar", view.Current);

		viewModel.Movie!.Name = "Dune";
		Assert.Equal("Dune", view.Current);

		viewModel.Movie = new() { Name = "Arrival" };
		Assert.Equal("Arrival", view.Current);

		viewModel.Movie.Name = "Tenet";
		Assert.Equal("Tenet", view.Current);
	}

	[Fact]
	public void Path_RejectsMethodCall()
	{
		ArgumentException error = Assert.Throws<ArgumentException>(() =>
			BindingFactory.Bind((MovieViewModel vm) => vm.Title.ToUpper()));

		Assert.Contains("plain member access", error.Message);
	}
}
