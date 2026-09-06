namespace SkeleKit;

public abstract partial class ContentView
{
	internal PageHost? Host { get; set; }
	internal Thickness PageSafeArea { get; set; } = Thickness.Zero;

	View? automaticScrollBleed;

	internal View? AutomaticScrollBleed => automaticScrollBleed;

	/// <summary>
	/// The native controller hosting this page, or null before it is presented.
	/// </summary>
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

	partial void ApplySearchTextCore() =>
		Host?.ApplySearchText(SearchText.Value ?? string.Empty);

	partial void ApplySearchScopeCore() =>
		Host?.ApplySearchScope(SearchScopeIndex.Value);

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
		if (Content is View content)
			UpdateAutomaticScrollBleed(content);

		base.OnRealized();

		NotifyLoaded();
	}

	private protected override void OnUnrealized()
	{
		NotifyUnloaded();

		base.OnUnrealized();
	}

	partial void PrepareContentLayoutCore(
		View content) =>
		UpdateAutomaticScrollBleed(content);

	void UpdateAutomaticScrollBleed(
		View content)
	{
		bool shouldBleed = ScrollsUnderBars
			&& content.Scrolls
			&& content.VerticalAlignment == VerticalAlignment.Stretch
			&& double.IsNaN(content.Height)
			&& double.IsPositiveInfinity(content.MaxHeight);

		if (automaticScrollBleed is View previous
			&& (!ReferenceEquals(previous, content) || !shouldBleed))
		{
			if (previous.IgnoresSafeArea == (SafeAreaEdges.Top | SafeAreaEdges.Bottom))
				previous.IgnoresSafeArea = SafeAreaEdges.None;

			automaticScrollBleed = null;
		}

		if (!shouldBleed || content.IgnoresSafeArea != SafeAreaEdges.None)
			return;

		content.IgnoresSafeArea = SafeAreaEdges.Top | SafeAreaEdges.Bottom;
		automaticScrollBleed = content;
	}
}
