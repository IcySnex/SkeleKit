using Microsoft.Extensions.DependencyInjection;

namespace SkeleKit;

public abstract partial class ContentView
{
	internal PageHost? Host { get; set; }
	internal Thickness PageSafeArea { get; set; } = Thickness.Zero;


#pragma warning disable CA1822
	/// <summary>
	/// The application's navigator, for navigation from page code.
	/// </summary>
	/// <remarks>
	/// ViewModels take <see cref="INavigator"/> by constructor instead.
	/// </remarks>
	protected INavigator Navigator => SkeleApplication.Current?.Services.GetRequiredService<INavigator>() ?? throw new InvalidOperationException("There is no running application.");

	/// <summary>
	/// The application's share sheet, for sharing from page code.
	/// </summary>
	/// <remarks>
	/// ViewModels take <see cref="ISharer"/> by constructor instead.
	/// </remarks>
	protected ISharer Sharer => SkeleApplication.Current?.Services.GetRequiredService<ISharer>() ?? throw new InvalidOperationException("There is no running application.");

	/// <summary>
	/// The application's photo and document pickers, for picking from page code.
	/// </summary>
	/// <remarks>
	/// ViewModels take <see cref="ISystemPicker"/> by constructor instead.
	/// </remarks>
	protected ISystemPicker SystemPicker => SkeleApplication.Current?.Services.GetRequiredService<ISystemPicker>() ?? throw new InvalidOperationException("There is no running application.");
#pragma warning restore CA1822


	public UIViewController? Controller => Host;


	partial void ApplyTitleCore() =>
		Host?.NavigationItem.Title = Title.Value;

	partial void ApplyPromptCore()
	{
		if (Host is null)
			return;

		Host.NavigationItem.Prompt = Prompt.Value;

		// a prompt change resizes the bar, which the controller only relays out on the next layout pass
		if (Host.NavigationController is UINavigationController navigation && navigation.View is not null)
		{
			navigation.View.SetNeedsLayout();
			navigation.View.LayoutIfNeeded();
		}
	}

	partial void ApplyTabBadgeCore()
	{
		if (Host is not PageHost host)
			return;

		if (host.Tab is UITab tab)
		{
			tab.BadgeValue = TabBadge.Value;
			return;
		}

		host.TabBarItem.BadgeValue = TabBadge.Value;
		host.TabBarItem.BadgeColor = TabBadgeColor?.ToUIColor();
	}

	partial void ApplyLeaveGuardCore() =>
		Host?.ApplyLeaveGuard();


	private protected override void OnRealized()
	{
		if (ScrollsUnderBars
			&& Content is { IgnoresSafeArea: SafeAreaEdges.None } content
			&& content.Scrolls)
			content.IgnoresSafeArea = SafeAreaEdges.Top | SafeAreaEdges.Bottom;

		base.OnRealized();

		NotifyLoaded();
	}

	private protected override void OnUnrealized()
	{
		NotifyUnloaded();

		base.OnUnrealized();
	}
}
