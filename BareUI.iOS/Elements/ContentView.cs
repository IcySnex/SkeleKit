using System.Runtime.CompilerServices;

namespace BareUI;

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
	/// Which edges the page keeps clear of the safe area. Defaults to all of them.
	/// </summary>
	public SafeAreaEdges SafeAreaEdges { get; set; } = SafeAreaEdges.All;

	/// <summary>
	/// Whether scrolling content passes under the navigation bar, so they blur over it. On by default.
	/// </summary>
	public bool ScrollsUnderBars { get; set; } = true;

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
	public string? Prompt { get; set; }

	/// <summary>
	/// The status bar look for this page.
	/// </summary>
	public StatusBarStyle StatusBar { get; set; }

	/// <summary>
	/// The accent for this page's bar buttons and back button, or null for the app accent.
	/// </summary>
	public Color? BarAccent { get; set; }

	/// <summary>
	/// The navigation title's color, or null for the system default.
	/// </summary>
	public Color? TitleColor { get; set; }

	/// <summary>
	/// The expanded large title's color, or null for the system default.
	/// </summary>
	public Color? LargeTitleColor { get; set; }

	/// <summary>
	/// Asked before the page is left — back button or sheet swipe — so unsaved changes can veto. Return false to stay.
	/// </summary>
	public Func<Task<bool>>? ConfirmLeave { get; set; }

	/// <summary>
	/// Hides the tab bar while this page is on top of the stack.
	/// </summary>
	public bool HidesTabBar { get; set; }

	/// <summary>
	/// The badge on this page's tab bar item, or null for none. Applies even while the tab was never opened.
	/// </summary>
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
	/// Buttons in a persistent bar along the screen's bottom edge. Above a visible tab bar they float as its accessory; everywhere else they form the classic bottom toolbar.
	/// </summary>
	public IList<ToolbarItem> BottomToolbarItems { get; } = [];

	/// <summary>
	/// Placeholder for the navigation bar's search field. Setting it shows the search bar.
	/// </summary>
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
	/// Invoked as the user types in the search field.
	/// </summary>
	public Action<string>? SearchChanged { get; set; }

	/// <summary>
	/// Invoked with the selected index when the user switches search scope.
	/// </summary>
	public Action<int>? SearchScopeChanged { get; set; }

	/// <summary>
	/// Invoked when the user cancels out of the search field.
	/// </summary>
	public Action? SearchCancelled { get; set; }

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
	/// Raised after the page appears on screen.
	/// </summary>
	protected virtual void OnAppearing()
	{ }

	/// <summary>
	/// Raised as the page leaves the screen.
	/// </summary>
	protected virtual void OnDisappearing()
	{ }


	internal void NotifySearch(
		string text) =>
		SearchChanged?.Invoke(text);

	internal void NotifySearchScope(
		int index) =>
		SearchScopeChanged?.Invoke(index);

	internal void NotifySearchCancelled() =>
		SearchCancelled?.Invoke();

	internal void NotifyLoaded() =>
		OnLoaded();

	internal void NotifyUnloaded() =>
		OnUnloaded();

	internal void NotifyAppearing() =>
		OnAppearing();

	internal void NotifyDisappearing() =>
		OnDisappearing();


	void ApplyTitle() =>
		ApplyTitleCore();

	partial void ApplyTitleCore();

	internal void ApplyTabBadge() =>
		ApplyTabBadgeCore();

	partial void ApplyTabBadgeCore();


	protected override Size MeasureOverride(
		Size availableSize)
	{
		if (Content is not { } content)
			return Size.Zero;

		content.Measure(availableSize);

		return content.DesiredSize;
	}

	protected override Size ArrangeOverride(
		Size finalSize)
	{
		Content?.Arrange(new(Point.Zero, finalSize));

		return finalSize;
	}
}

/// <summary>
/// A page bound to a typed ViewModel: bind with <c>Bind(...)</c>.
/// </summary>
/// <typeparam name="TViewModel">The structural data type backing the container context.</typeparam>
public abstract class ContentView<TViewModel> : ContentView
	where TViewModel : class
{
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


	/// <summary>
	/// Binds one way to a ViewModel property.
	/// </summary>
	/// <typeparam name="T">The underlying type of the bound target element.</typeparam>
	/// <param name="getter">The property selection expression.</param>
	/// <param name="path">The automatically captured string representation of the expression property.</param>
	/// <returns>A one-way tracking data stream context configuration.</returns>
	protected static BindingExpression<T?> Bind<T>(
		Func<TViewModel, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, path);

	/// <summary>
	/// Binds two ways; <paramref name="setter"/> writes the control's value back.
	/// </summary>
	/// <typeparam name="T">The underlying type of the bound target element.</typeparam>
	/// <param name="getter">The property selection expression.</param>
	/// <param name="setter">The logic layer handler mutation expression.</param>
	/// <param name="path">The automatically captured string representation of the expression property.</param>
	/// <returns>A bi-directional tracking data stream context configuration.</returns>
	protected static BindingExpression<T?> Bind<T>(
		Func<TViewModel, T> getter,
		Action<TViewModel, T?> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, setter, path);

	/// <summary>
	/// Binds one way through a converter.
	/// </summary>
	/// <typeparam name="TValue">The intermediate value node processed from the state tier.</typeparam>
	/// <typeparam name="T">The targeted structural output presentation type.</typeparam>
	/// <param name="getter">The property selection expression.</param>
	/// <param name="format">The mapper rule converting source parameters to targets.</param>
	/// <param name="path">The automatically captured string representation of the expression property.</param>
	/// <returns>A converted one-way tracking data stream context configuration.</returns>
	protected static BindingExpression<T?> Bind<TValue, T>(
		Func<TViewModel, TValue> getter,
		Func<TValue, T> format,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, format, path);

	/// <summary>
	/// Binds two ways through converters: <paramref name="format"/> out, <paramref name="parse"/> back in. A numeric text field, for instance.
	/// </summary>
	/// <typeparam name="TValue">The intermediate value node processed from the state tier.</typeparam>
	/// <typeparam name="T">The targeted structural output presentation type.</typeparam>
	/// <param name="getter">The property selection expression.</param>
	/// <param name="setter">The logic layer handler mutation expression.</param>
	/// <param name="format">The mapping parser converting state metrics into screen views.</param>
	/// <param name="parse">The reverse mapping translation parsing screen string types into model fields.</param>
	/// <param name="path">The automatically captured string representation of the expression property.</param>
	/// <returns>A converted bi-directional tracking data stream context configuration.</returns>
	protected static BindingExpression<T?> Bind<TValue, T>(
		Func<TViewModel, TValue> getter,
		Action<TViewModel, TValue> setter,
		Func<TValue, T> format,
		Func<T, TValue> parse,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, setter, format, parse, path);

	/// <summary>
	/// Binds control to source only: the control writes, and never reads back.
	/// </summary>
	/// <typeparam name="T">The underlying type of the bound target element.</typeparam>
	/// <param name="getter">The basic initialization fallback extractor rule.</param>
	/// <param name="setter">The logic layer handler mutation expression.</param>
	/// <param name="path">The automatically captured string representation of the expression property.</param>
	/// <returns>An upward-streaming destination-only data configuration.</returns>
	protected static BindingExpression<T?> BindToSource<T>(
		Func<TViewModel, T> getter,
		Action<TViewModel, T?> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.BindToSource(getter, setter, path);

	/// <summary>
	/// Reads the value once when the ViewModel attaches, then never again.
	/// </summary>
	/// <typeparam name="T">The underlying type of the bound target element.</typeparam>
	/// <param name="getter">The instant-evaluation property state extraction expression.</param>
	/// <param name="path">The automatically captured string representation of the expression property.</param>
	/// <returns>A static-snapshot evaluation configuration.</returns>
	protected static BindingExpression<T?> BindOnce<T>(
		Func<TViewModel, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.BindOnce(getter, path);
}
