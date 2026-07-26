using System.Runtime.CompilerServices;

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
	/// Invoked when this page's tab is tapped while already selected, replacing the default pop-to-root / scroll-to-top.
	/// </summary>
	public Action? TabReselected { get; set; }

	/// <summary>
	/// The badge on this page's tab bar item, or null for none.
	/// </summary>
	/// <remarks>
	/// Applies even while the tab was never opened.
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
	/// Buttons in a persistent bar along the screen's bottom edge.
	/// </summary>
	/// <remarks>
	/// Above a visible tab bar they float as its accessory; everywhere else they form the classic bottom toolbar.
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

	partial void ApplyTitleCore();

	partial void ApplyPromptCore();

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

	internal void NotifySearchCanceled() =>
		SearchCanceled?.Invoke();

	internal void NotifyLoaded() =>
		OnLoaded();

	internal void NotifyUnloaded() =>
		OnUnloaded();

	internal void NotifyWillAppear() =>
		PageAppeared();

	internal void NotifyAppearing() =>
		OnAppearing();

	internal void NotifyDisappearing() =>
		OnDisappearing();

	internal void ApplyTabBadge() =>
		ApplyTabBadgeCore();


	protected override Size MeasureOverride(
		Size availableSize)
	{
		Size inner = availableSize.Deflate(Padding);

		if (Content is not View content)
			return new(Padding.Horizontal, Padding.Vertical);

		content.Measure(inner);

		return content.DesiredSize.Inflate(Padding);
	}

	protected override Size ArrangeOverride(
		Size finalSize)
	{
		if (Content is View content)
		{
			PrepareContentLayoutCore(content);
			content.Arrange(
				new Rect(
					new Point(Padding.Left, Padding.Top),
					finalSize.Deflate(Padding)));
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
	/// Binds one way to a ViewModel property.
	/// </summary>
	/// <typeparam name="T">The bound value type.</typeparam>
	/// <param name="getter">The ViewModel property to read.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<T?> Bind<T>(
		Func<TViewModel, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, path);

	/// <summary>
	/// Binds two ways; <paramref name="setter"/> writes the control's value back.
	/// </summary>
	/// <typeparam name="T">The bound value type.</typeparam>
	/// <param name="getter">The ViewModel property to read.</param>
	/// <param name="setter">The action that writes the value back to the ViewModel.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<T?> Bind<T>(
		Func<TViewModel, T> getter,
		Action<TViewModel, T?> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, setter, path);

	/// <summary>
	/// Binds one way through a converter.
	/// </summary>
	/// <typeparam name="TValue">The value type read from the ViewModel.</typeparam>
	/// <typeparam name="T">The converted value type.</typeparam>
	/// <param name="getter">The ViewModel property to read.</param>
	/// <param name="format">Converts the ViewModel value for display.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<T?> Bind<TValue, T>(
		Func<TViewModel, TValue> getter,
		Func<TValue, T> format,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.Bind(getter, format, path);

	/// <summary>
	/// Binds two ways through converters: <paramref name="format"/> out, <paramref name="parse"/> back in, as for a numeric text field.
	/// </summary>
	/// <typeparam name="TValue">The value type read from the ViewModel.</typeparam>
	/// <typeparam name="T">The converted value type.</typeparam>
	/// <param name="getter">The ViewModel property to read.</param>
	/// <param name="setter">The action that writes the value back to the ViewModel.</param>
	/// <param name="format">Converts the ViewModel value for display.</param>
	/// <param name="parse">Converts the displayed value back to the ViewModel type.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
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
	/// <typeparam name="T">The bound value type.</typeparam>
	/// <param name="getter">The ViewModel property to read the initial value from.</param>
	/// <param name="setter">The action that writes the value back to the ViewModel.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<T?> BindToSource<T>(
		Func<TViewModel, T> getter,
		Action<TViewModel, T?> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.BindToSource(getter, setter, path);

	/// <summary>
	/// Reads the value once when the ViewModel attaches, then never again.
	/// </summary>
	/// <typeparam name="T">The bound value type.</typeparam>
	/// <param name="getter">The ViewModel property to read.</param>
	/// <param name="path">The source lambda, captured automatically to derive the property path.</param>
	/// <returns>The binding expression.</returns>
	protected static BindingExpression<T?> BindOnce<T>(
		Func<TViewModel, T> getter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		BindingFactory.BindOnce(getter, path);


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
