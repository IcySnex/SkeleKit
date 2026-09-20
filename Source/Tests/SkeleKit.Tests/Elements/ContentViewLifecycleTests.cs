using Xunit;

namespace SkeleKit.Tests.Elements;

public class ContentViewLifecycleTests
{
	sealed class EmptyView : ContentView
	{ }

	sealed class LifecycleView : ContentView
	{
		public List<string> Events { get; } = [];


		protected override void OnAppearing() =>
			Events.Add("Appearing");

		protected override void OnAppeared() =>
			Events.Add("Appeared");

		protected override void OnDisappearing() =>
			Events.Add("Disappearing");

		protected override void OnDisappeared() =>
			Events.Add("Disappeared");
	}

	[Fact]
	public void NavigationTitleStyle_DefaultsToInherited()
	{
		EmptyView view = new();

		Assert.Null(view.NavigationTitleStyle);
	}

	[Fact]
	public void NavigationBarMinimization_HasStableDefaults()
	{
		EmptyView view = new();

		Assert.Equal(NavigationBarMinimize.Never, view.NavigationBarMinimizeBehavior);
		Assert.Equal(NavigationBarMinimizeSafeArea.Automatic, view.NavigationBarMinimizeSafeAreaAdjustment);
		Assert.Equal(NavigationBarMinimizeRestore.Automatic, view.NavigationBarMinimizeRestorationBehavior);
	}

	[Fact]
	public void TopScrollEdgeStyle_DefaultsToInherited()
	{
		EmptyView view = new();

		Assert.Null(view.TopScrollEdgeStyle);

		view.NavigationAccessory = new Border();

		Assert.Null(view.TopScrollEdgeStyle);

		view.TopScrollEdgeStyle = ScrollEdgeStyle.Hard;

		Assert.Equal(ScrollEdgeStyle.Hard, view.TopScrollEdgeStyle);
	}

	[Fact]
	public void NavigationAccessory_DisablesMinimizationUntilRemoved()
	{
		EmptyView view = new()
		{
			NavigationBarMinimizeBehavior = NavigationBarMinimize.OnScrollDown
		};

		Assert.Equal(NavigationBarMinimize.OnScrollDown, view.EffectiveNavigationBarMinimizeBehavior);

		view.NavigationAccessory = new Border();

		Assert.Equal(NavigationBarMinimize.Never, view.EffectiveNavigationBarMinimizeBehavior);

		view.NavigationAccessory = null;

		Assert.Equal(NavigationBarMinimize.OnScrollDown, view.EffectiveNavigationBarMinimizeBehavior);
	}

	[Fact]
	public void Background_DefaultsToSystemBackground()
	{
		EmptyView view = new();

		SolidBrush background = Assert.IsType<SolidBrush>(view.Background?.Value);
		Assert.Equal(Colors.Background, background.Color);
	}

	[Fact]
	public void NavigationAccessory_BelongsToPageAndDetachesWhenReplaced()
	{
		EmptyView page = new();
		Border first = new();
		Border second = new();

		page.NavigationAccessory = first;

		Assert.Same(page, first.Parent);

		page.NavigationAccessory = second;

		Assert.Null(first.Parent);
		Assert.Same(page, second.Parent);
	}


	[Fact]
	public void Notifications_InvokeMatchingLifecycleHooks()
	{
		LifecycleView view = new();

		view.NotifyAppearing();
		view.NotifyAppeared();
		view.NotifyDisappearing();
		view.NotifyDisappeared();

		Assert.Equal(
			["Appearing", "Appeared", "Disappearing", "Disappeared"],
			view.Events);
	}
}
