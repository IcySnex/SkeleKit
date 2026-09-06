using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SkeleKit;

/// <summary>
/// A full screen: compose its tree into <see cref="Content"/> in the constructor.
/// </summary>
public abstract partial class ContentView : Panel
{
	/// <summary>
	/// The navigation bar title.
	/// </summary>
	public Bindable<string?> Title
	{
		get => title;
		set => titleBinding = Register(titleBinding, value, value => Set(ref title, value, ApplyTitle, affectsMeasure: false));
	}
	string? title;
	Binding<string?>? titleBinding;

	/// <summary>
	/// Which edges the page keeps clear of the safe area.
	/// </summary>
	public SafeAreaEdges SafeAreaEdges { get; set; } = SafeAreaEdges.All;

	/// <summary>
	/// Whether scrolling content passes under the navigation bar so the bar blurs over it.
	/// </summary>
	public bool ScrollsUnderBars
	{
		get;
		set => Set(ref field, value);
	} = true;

	/// <summary>
	/// Whether the title is shown large and collapses as the content scrolls.
	/// </summary>
	public TitleStyle TitleStyle { get; set; } = TitleStyle.Inline;

	/// <summary>
	/// Hides the navigation bar for this page.
	/// </summary>
	public bool HidesNavigationBar { get; set; }

	/// <summary>
	/// The page's background style.
	/// </summary>
	public PageBackground BackgroundStyle { get; set; } = PageBackground.Default;

	/// <summary>
	/// The back button title the next pushed page shows, or null for this page's title.
	/// </summary>
	public string? BackButtonTitle { get; set; }

	/// <summary>
	/// How the next pushed page's back button represents this page.
	/// </summary>
	public BackButtonStyle BackButtonStyle { get; set; }

	/// <summary>
	/// The small line of text above the navigation title, or null for none.
	/// </summary>
	public Bindable<string?> Prompt
	{
		get => prompt;
		set => promptBinding = Register(promptBinding, value, value => Set(ref prompt, value, ApplyPrompt, affectsMeasure: false));
	}
	string? prompt;
	Binding<string?>? promptBinding;

	/// <summary>
	/// The status bar look for this page.
	/// </summary>
	public StatusBarStyle StatusBar { get; set; }

	/// <summary>
	/// The tint for this page's bar buttons and back button, or null for the app tint.
	/// </summary>
	public Color? BarTint { get; set; }

	/// <summary>
	/// The navigation title's color, or null for the system default.
	/// </summary>
	public Color? TitleColor { get; set; }

	/// <summary>
	/// The expanded large title's color, or null for the system default.
	/// </summary>
	public Color? LargeTitleColor { get; set; }

	/// <summary>
	/// Asked before the page is left, so unsaved changes can veto leaving.
	/// </summary>
	/// <remarks>
	/// Fires for the back button, a sheet swipe or a popover tap-out; return <c>false</c> to stay.<br/>
	/// Leave it null while nothing needs guarding, which also disables the interactive pop-back swipe.
	/// </remarks>
	public Func<Task<bool>>? ConfirmLeave
	{
		get;
		set
		{
			field = value;
			ApplyLeaveGuardCore();
		}
	}

	/// <summary>
	/// Hides the tab bar while this page is on top of the stack.
	/// </summary>
	public bool HidesTabBar { get; set; }

	/// <summary>
	/// Command invoked when this page's tab is tapped while already selected, replacing the default pop-to-root / scroll-to-top.
	/// </summary>
	public ICommand? TabReselectedCommand { get; set; }

	/// <summary>
	/// The parameter passed to <see cref="TabReselectedCommand"/>.
	/// </summary>
	public object? TabReselectedCommandParameter { get; set; }

	/// <summary>
	/// The badge on this page's tab bar item, or null for none.
	/// </summary>
	/// <remarks>
	/// Applies before the tab is opened.
	/// </remarks>
	public Bindable<string?> TabBadge
	{
		get => tabBadge;
		set => tabBadgeBinding = Register(tabBadgeBinding, value, value =>
		{
			// not routed through Set's apply: the badge must land on a tab that was never realized
			tabBadge = value;
			ApplyTabBadge();
		});
	}
	string? tabBadge;
	Binding<string?>? tabBadgeBinding;

	/// <summary>
	/// The badge's background color, or null for the system red.
	/// </summary>
	public Color? TabBadgeColor
	{
		get;
		set
		{
			field = value;
			ApplyTabBadge();
		}
	}

	/// <summary>
	/// Buttons in the navigation bar.
	/// </summary>
	public IList<ToolbarItem> ToolbarItems { get; } = [];

	/// <summary>
	/// Buttons along the screen's bottom edge.
	/// </summary>
	/// <remarks>
	/// The toolbar stays hidden while a tab bar occupies the bottom edge.
	/// A pushed page can set <see cref="HidesTabBar"/> to show its bottom toolbar in place of the tab bar.
	/// </remarks>
	public IList<ToolbarItem> BottomToolbarItems { get; } = [];

	/// <summary>
	/// Placeholder for the navigation bar's search field.
	/// </summary>
	/// <remarks>
	/// Setting it shows the search bar.
	/// </remarks>
	public string? SearchPlaceholder { get; set; }

	/// <summary>
	/// Whether the search bar collapses into the bar as the content scrolls.
	/// </summary>
	public bool HidesSearchBarWhenScrolling { get; set; }

	/// <summary>
	/// Whether the content dims behind an active search.
	/// </summary>
	public bool SearchObscuresBackground { get; set; }

	/// <summary>
	/// Titles of the scope buttons under an active search field. Empty for none.
	/// </summary>
	public IList<string> SearchScopes { get; } = [];

	/// <summary>
	/// Whether search scopes stay hidden until the search field contains text.
	/// </summary>
	public bool HidesSearchScopesWhenEmpty { get; set; }

	/// <summary>
	/// The search field's text.
	/// </summary>
	public Bindable<string?> SearchText
	{
		get => searchText;
		set => searchTextBinding = Register(searchTextBinding, value, value => Set(ref searchText, value, ApplySearchText, affectsMeasure: false));
	}
	string? searchText;
	Binding<string?>? searchTextBinding;

	/// <summary>
	/// The selected search scope index.
	/// </summary>
	public Bindable<int> SearchScopeIndex
	{
		get => searchScopeIndex;
		set => searchScopeIndexBinding = Register(searchScopeIndexBinding, value, value => Set(ref searchScopeIndex, value, ApplySearchScope, affectsMeasure: false));
	}
	int searchScopeIndex;
	Binding<int>? searchScopeIndexBinding;

	/// <summary>
	/// Invoked as the user types in the search field.
	/// </summary>
	public Action<string>? SearchChanged { get; set; }

	/// <summary>
	/// Invoked with the selected index when the user switches search scope.
	/// </summary>
	public Action<int>? SearchScopeChanged { get; set; }

	/// <summary>
	/// Command invoked when the user submits the current search text.
	/// </summary>
	public ICommand? SearchCommand { get; set; }

	/// <summary>
	/// Invoked when the user cancels out of the search field.
	/// </summary>
	public Action? SearchCanceled { get; set; }

	/// <summary>
	/// The page's element tree.
	/// </summary>
	public View? Content
	{
		get => Children.Count > 0 ? Children[0] : null;
		set
		{
			Children.Clear();

			if (value is not null)
				Children.Add(value);
		}
	}


	void ApplyTitle() =>
		ApplyTitleCore();

	void ApplyPrompt() =>
		ApplyPromptCore();

	void ApplySearchText() =>
		ApplySearchTextCore();

	void ApplySearchScope() =>
		ApplySearchScopeCore();

	partial void ApplyTitleCore();

	partial void ApplyPromptCore();

	partial void ApplySearchTextCore();

	partial void ApplySearchScopeCore();

	partial void ApplyTabBadgeCore();

	partial void ApplyLeaveGuardCore();


	/// <summary>
	/// Raised once, the first time the page is realized.
	/// </summary>
	protected virtual void OnLoaded()
	{ }

	/// <summary>
	/// Raised when the page's tree is torn down.
	/// </summary>
	protected virtual void OnUnloaded()
	{ }

	/// <summary>
	/// Raised before the page appears on screen.
	/// </summary>
	protected virtual void OnAppearing()
	{ }

	/// <summary>
	/// Raised after the page appears on screen.
	/// </summary>
	protected virtual void OnAppeared()
	{ }

	/// <summary>
	/// Raised before the page leaves the screen.
	/// </summary>
	protected virtual void OnDisappearing()
	{ }

	/// <summary>
	/// Raised after the page leaves the screen.
	/// </summary>
	protected virtual void OnDisappeared()
	{ }


	internal void NotifySearch(
		string text)
	{
		Set(ref searchText, text, affectsMeasure: false);
		searchTextBinding?.PushToSource(text);
		SearchChanged?.Invoke(text);
	}

	internal void NotifySearchScope(
		int index)
	{
		Set(ref searchScopeIndex, index, affectsMeasure: false);
		searchScopeIndexBinding?.PushToSource(index);
		SearchScopeChanged?.Invoke(index);
	}

	internal void NotifySearchSubmitted()
	{
		if (SearchCommand is ICommand command && command.CanExecute(searchText))
			command.Execute(searchText);
	}

	internal void NotifySearchCanceled() =>
		SearchCanceled?.Invoke();

	internal void NotifyLoaded() =>
		OnLoaded();

	internal void NotifyUnloaded() =>
		OnUnloaded();

	internal void NotifyAppearing()
	{
		PageWillAppear();
		OnAppearing();
	}

	internal void NotifyAppeared() =>
		OnAppeared();

	internal void NotifyDisappearing() =>
		OnDisappearing();

	internal void NotifyDisappeared() =>
		OnDisappeared();

	internal void ApplyTabBadge() =>
		ApplyTabBadgeCore();


	/// <inheritdoc/>
	protected override Size MeasureOverride(
		Size availableSize)
	{
		Size inner = availableSize.Deflate(Padding);

		if (Content is not View content)
			return new(Padding.Horizontal, Padding.Vertical);

		content.Measure(inner);

		return content.DesiredSize.Inflate(Padding);
	}

	/// <inheritdoc/>
	protected override Size ArrangeOverride(
		Size finalSize)
	{
		if (Content is View content)
		{
			PrepareContentLayoutCore(content);
			content.Arrange(new(new(Padding.Left, Padding.Top), finalSize.Deflate(Padding)));
		}

		return finalSize;
	}

	partial void PrepareContentLayoutCore(
		View content);
}

/// <summary>
/// A page bound to a typed ViewModel: bind with <c>Bind(...)</c>.
/// </summary>
/// <typeparam name="TViewModel">The ViewModel type driving the page.</typeparam>
public abstract class ContentView<TViewModel> : ContentView
	where TViewModel : class
{
	/// <summary>
	/// Starts a binding that can only write control values to the ViewModel.
	/// </summary>
	/// <returns>A source-only binding builder.</returns>
	protected static ToSourceBindingBuilder<TViewModel> Bind() =>
		BindingFactory.Bind<TViewModel>();


	/// <summary>
	/// Binds one way to a ViewModel property.
	/// </summary>
	/// <typeparam name="T">The bound value type.</typeparam>
	/// <param name="read">The ViewModel property to read.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<TViewModel, TViewModel, T> Bind<T>(
		Func<TViewModel, T> read,
		[CallerArgumentExpression(nameof(read))] string? path = null) =>
		BindingFactory.Bind(read, path);


	/// <summary>
	/// Stores the ViewModel, so the derived constructor composes its tree against it directly.
	/// </summary>
	/// <param name="viewModel">The ViewModel driving this page.</param>
	protected ContentView(
		TViewModel viewModel)
	{
		ViewModel = viewModel;
		BindingContext = viewModel;
	}


	/// <summary>
	/// The ViewModel this page was built around. Bindings resolve against it.
	/// </summary>
	public TViewModel ViewModel { get; }
}
