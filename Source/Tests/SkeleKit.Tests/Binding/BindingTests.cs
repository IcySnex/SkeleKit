using Xunit;

namespace SkeleKit.Tests.Binding;

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
	public void IsEnabled_TracksSourceChanges()
	{
		MovieViewModel viewModel = new() { Enabled = true };
		StubBound view = new() { IsEnabled = BindingFactory.Bind((MovieViewModel vm) => vm.Enabled) };
		view.BindingContext = viewModel;

		Assert.True(view.IsEnabled.Value);

		viewModel.Enabled = false;

		Assert.False(view.IsEnabled.Value);
	}

	[Fact]
	public void SearchState_RoundTrips()
	{
		MovieViewModel viewModel = new() { Query = "SkeleKit", SearchScope = 1 };
		StubPage page = new()
		{
			SearchText = BindingFactory.Bind((MovieViewModel vm) => vm.Query)
				.TwoWay((vm, val) => vm.Query = val ?? ""),
			SearchScopeIndex = BindingFactory.Bind((MovieViewModel vm) => vm.SearchScope)
				.TwoWay((vm, val) => vm.SearchScope = val)
		};
		page.BindingContext = viewModel;

		Assert.Equal("SkeleKit", page.SearchText.Value);
		Assert.Equal(1, page.SearchScopeIndex.Value);

		page.NotifySearch("bindings");
		page.NotifySearchScope(2);

		Assert.Equal("bindings", viewModel.Query);
		Assert.Equal(2, viewModel.SearchScope);

		viewModel.Query = "commands";
		viewModel.SearchScope = 3;

		Assert.Equal("commands", page.SearchText.Value);
		Assert.Equal(3, page.SearchScopeIndex.Value);
	}

	[Fact]
	public void SearchCommand_ReceivesCurrentText()
	{
		string? submitted = null;
		StubPage page = new()
		{
			SearchText = "SkeleKit",
			SearchCommand = Command.From<string>(value => submitted = value)
		};

		page.NotifySearchSubmitted();

		Assert.Equal("SkeleKit", submitted);
	}

	// the neutral shim applies inline; this locks the marshalled refresh path end-to-end
	[Fact]
	public async Task OneWay_TracksChangesFromBackgroundThread()
	{
		MovieViewModel viewModel = new() { Title = "Interstellar" };
		StubBound view = new() { Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title) };
		view.BindingContext = viewModel;

		await Task.Run(() => viewModel.Title = "Dune");

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
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title)
				.TwoWay((vm, val) => vm.Title = val ?? "")
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
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Minutes)
				.ConvertTo(val => $"{val} min")
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
		StackPanel root = new() { Children = { leaf } };

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
	public void Path_ResubscribesWhenIntermediateReplaced()
	{
		MovieViewModel viewModel = new() { Movie = new() { Name = "Interstellar" } };
		StubBound view = new()
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Movie)
				.Path(movie => movie?.Name)
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
	public void Path_TwoWayWritesThroughRootSource()
	{
		MovieViewModel viewModel = new() { Movie = new() { Name = "Interstellar" } };
		StubBound view = new()
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Movie)
				.Path(movie => movie?.Name)
				.TwoWay((vm, val) => vm.Movie!.Name = val ?? "")
		};
		view.BindingContext = viewModel;

		view.SimulateEdit("Dune");

		Assert.Equal("Dune", viewModel.Movie.Name);
	}

	[Fact]
	public void Path_CanObserveMultipleReplaceableObjects()
	{
		MovieViewModel viewModel = new()
		{
			Movie = new() { Director = new() { Name = "Denis Villeneuve" } }
		};
		StubBound view = new()
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Movie)
				.Path(movie => movie?.Director)
				.Path(director => director?.Name)
		};
		view.BindingContext = viewModel;

		Assert.Equal("Denis Villeneuve", view.Current);

		viewModel.Movie!.Director!.Name = "Christopher Nolan";
		Assert.Equal("Christopher Nolan", view.Current);

		viewModel.Movie.Director = new() { Name = "Greta Gerwig" };
		Assert.Equal("Greta Gerwig", view.Current);

		viewModel.Movie = new() { Director = new() { Name = "Bong Joon Ho" } };
		Assert.Equal("Bong Joon Ho", view.Current);
	}

	[Fact]
	public void Once_DoesNotTrackSourceChanges()
	{
		MovieViewModel viewModel = new() { Title = "Interstellar" };
		StubBound view = new()
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title).Once()
		};
		view.BindingContext = viewModel;

		viewModel.Title = "Dune";

		Assert.Equal("Interstellar", view.Current);
	}

	[Fact]
	public void ToSource_DoesNotReadOrSubscribe()
	{
		MovieViewModel viewModel = new() { Title = "Interstellar" };
		StubBound view = new()
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Title)
				.ToSource((vm, val) => vm.Title = val ?? "")
		};
		view.BindingContext = viewModel;

		Assert.Null(view.Current);

		viewModel.Title = "Arrival";
		Assert.Null(view.Current);

		view.SimulateEdit("Dune");
		Assert.Equal("Dune", viewModel.Title);
	}

	[Fact]
	public void ConvertedTwoWay_RoundTrips()
	{
		MovieViewModel viewModel = new() { Minutes = 169 };
		StubBound view = new()
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Minutes)
				.TwoWay((vm, val) => vm.Minutes = val)
				.ConvertTo(val => val.ToString())
				.ConvertFrom(val => int.Parse(val))
		};
		view.BindingContext = viewModel;

		Assert.Equal("169", view.Current);

		view.SimulateEdit("120");
		Assert.Equal(120, viewModel.Minutes);
	}

	[Fact]
	public void ConvertedToSource_WritesConvertedValue()
	{
		MovieViewModel viewModel = new() { Minutes = 169 };
		StubBound view = new()
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Minutes)
				.ToSource((vm, val) => vm.Minutes = val)
				.ConvertFrom((string val) => int.Parse(val))
		};
		view.BindingContext = viewModel;

		Assert.Null(view.Current);

		view.SimulateEdit("120");
		Assert.Equal(120, viewModel.Minutes);
	}

	[Fact]
	public void WritableConversion_RequiresConvertFrom()
	{
		Assert.Throws<InvalidOperationException>(() => new StubBound
		{
			Text = BindingFactory.Bind((MovieViewModel vm) => vm.Minutes)
				.TwoWay((vm, val) => vm.Minutes = val)
				.ConvertTo(val => val.ToString())
		});
	}

	[Fact]
	public void UpdateOn_StoresTrigger()
	{
		BindingExpression<MovieViewModel, string, string> binding = BindingFactory
			.Bind((MovieViewModel vm) => vm.Title)
			.TwoWay((vm, val) => vm.Title = val ?? "")
			.UpdateOn(UpdateTrigger.FocusLost);

		Assert.Equal(UpdateTrigger.FocusLost, binding.Trigger);
	}

	[Fact]
	public void Path_RejectsMethodCall()
	{
		ArgumentException error = Assert.Throws<ArgumentException>(() =>
			BindingFactory.Bind((MovieViewModel vm) => vm.Title.ToUpper()));

		Assert.Contains("plain member access", error.Message);
	}
}
