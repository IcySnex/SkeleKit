using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace SkeleKit;

/// <summary>
/// Which side of the navigation bar a toolbar item sits on.
/// </summary>
public enum ToolbarSide
{
	/// <summary>
	/// Trailing edge (the right, in a left-to-right layout).
	/// </summary>
	Trailing,

	/// <summary>
	/// Leading edge.
	/// </summary>
	Leading
}

/// <summary>
/// An entry in a page's navigation bar or bottom toolbar.
/// </summary>
/// <remarks>
/// Use <see cref="ToolbarButton"/>, <see cref="ToolbarMenu"/>, or <see cref="ToolbarGroup"/>.
/// Bindable presentation properties accept literal values or <c>Bind(...)</c> expressions.
/// </remarks>
public abstract class ToolbarItem
{
	readonly List<BindingBase> bindings = [];
	object? bindingContext;
	bool bindingsAttached;

	internal ToolbarItem()
	{
	}

	/// <summary>
	/// Whether the item is in the bar at all.
	/// </summary>
	/// <remarks>
	/// Contextual actions toggle it live, like a Delete that only exists in edit mode.
	/// </remarks>
	public Bindable<bool> IsVisible
	{
		get => isVisible;
		set => isVisibleBinding = Register(isVisibleBinding, value, value => Set(ref isVisible, value));
	}
	bool isVisible = true;
	Binding<bool>? isVisibleBinding;

	internal bool IsVisibleValue => isVisible;

	internal event Action<ToolbarItem>? Changed;

	private protected void OnChanged() =>
		Changed?.Invoke(this);

	/// <summary>
	/// Which side of the bar the item sits on.
	/// </summary>
	/// <remarks>
	/// Only used for top-level entries. The value is ignored for actions inside a
	/// <see cref="ToolbarGroup"/>.
	/// </remarks>
	public ToolbarSide Side { get; set; } = ToolbarSide.Trailing;


	internal void AttachBindings(
		object? context)
	{
		if (bindingsAttached && ReferenceEquals(bindingContext, context))
			return;

		bindingContext = context;

		foreach (BindingBase binding in bindings)
			binding.Attach(context);

		bindingsAttached = true;
		OnBindingsAttached();
	}

	internal void DetachBindings()
	{
		OnBindingsDetached();
		bindingsAttached = false;
		bindingContext = null;

		foreach (BindingBase binding in bindings)
			binding.Detach();
	}

	private protected virtual void OnBindingsAttached()
	{
	}

	private protected virtual void OnBindingsDetached()
	{
	}

	private protected Binding<T>? Register<T>(
		Binding<T>? existing,
		Bindable<T> value,
		Action<T?> apply)
	{
		if (existing is not null)
		{
			existing.Detach();
			bindings.Remove(existing);
		}

		if (value.Expression is null)
		{
			apply(value.Value);
			return null;
		}

		Binding<T> binding = new(value.Expression, apply);
		bindings.Add(binding);
		binding.Attach(bindingContext);

		return binding;
	}

	private protected Binding<IReadOnlyList<T>?>? Register<T>(
		Binding<IReadOnlyList<T>?>? existing,
		BindableList<T> value,
		Action<IReadOnlyList<T>?> apply)
	{
		if (existing is not null)
		{
			existing.Detach();
			bindings.Remove(existing);
		}

		if (value.Expression is null)
		{
			apply(value.Value);
			return null;
		}

		Binding<IReadOnlyList<T>?> binding = new(value.Expression, apply);
		bindings.Add(binding);
		binding.Attach(bindingContext);

		return binding;
	}

	private protected void Set<T>(
		ref T field,
		T value)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return;

		field = value;
		OnChanged();
	}
}

/// <summary>
/// A toolbar item that performs an action or opens a menu.
/// </summary>
public abstract class ToolbarAction : ToolbarItem
{
	internal ToolbarAction()
	{
	}

	/// <summary>
	/// The item's text, or null when it shows only an icon.
	/// </summary>
	public Bindable<string?> Text
	{
		get => text;
		set => textBinding = Register(textBinding, value, value => Set(ref text, value));
	}
	string? text;
	Binding<string?>? textBinding;

	internal string? TextValue => text;

	/// <summary>
	/// The local icon, or null for a text-only item.
	/// </summary>
	public Bindable<ImageSource?> Icon
	{
		get => icon;
		set => iconBinding = Register(iconBinding, value, value => Set(ref icon, value));
	}
	ImageSource? icon;
	Binding<ImageSource?>? iconBinding;

	internal ImageSource? IconValue => icon;

	/// <summary>
	/// Whether the item is rendered as the prominent action.
	/// </summary>
	/// <remarks>
	/// A prominent item does not share its Liquid Glass background with other items.
	/// </remarks>
	public Bindable<bool> IsPrimary
	{
		get => isPrimary;
		set => isPrimaryBinding = Register(isPrimaryBinding, value, value => Set(ref isPrimary, value));
	}
	bool isPrimary;
	Binding<bool>? isPrimaryBinding;

	internal bool IsPrimaryValue => isPrimary;

	/// <summary>
	/// How long the item remains visible when the bar has limited space.
	/// </summary>
	/// <remarks>
	/// iOS 27 and later. Earlier versions use the system's normal ordering.
	/// </remarks>
	public ToolbarVisibilityPriority VisibilityPriority
	{
		get;
		set
		{
			field = value;
			OnChanged();
		}
	} = ToolbarVisibilityPriority.Standard;

	/// <summary>
	/// The item's tint, or null for the automatic bar tint.
	/// </summary>
	/// <remarks>
	/// The automatic tint uses the page's <see cref="ContentView.BarTint"/> when set.
	/// Otherwise it uses the app tint on earlier iOS versions, or the system label color
	/// for non-prominent items on iOS 26 and later.
	/// </remarks>
	public Bindable<Color?> Tint
	{
		get => tint;
		set => tintBinding = Register(tintBinding, value, value => Set(ref tint, value));
	}
	Color? tint;
	Binding<Color?>? tintBinding;

	internal Color? TintValue => tint;
}

/// <summary>
/// A button in the page's navigation bar or bottom toolbar.
/// </summary>
public sealed class ToolbarButton : ToolbarAction
{
	/// <summary>
	/// Invoked when the item is tapped; its CanExecute drives the enabled state.
	/// </summary>
	public ICommand? Command { get; set; }

	/// <summary>
	/// The parameter passed to <see cref="Command"/>.
	/// </summary>
	public object? CommandParameter { get; set; }
}

/// <summary>
/// A button that opens a menu in the page's navigation bar or bottom toolbar.
/// </summary>
public sealed class ToolbarMenu : ToolbarAction
{
	IReadOnlyList<MenuAction> actions = [];
	Binding<IReadOnlyList<MenuAction>?>? actionsBinding;
	bool actionsHooked;

	public ToolbarMenu()
	{
	}

	public ToolbarMenu(
		params MenuAction[] actions)
	{
		Actions = actions;
	}

	/// <summary>
	/// Menu entries shown on tap.
	/// </summary>
	/// <remarks>
	/// Accepts a literal list, an observable collection, or a <c>Bind(...)</c> list source.
	/// </remarks>
	public BindableList<MenuAction> Actions
	{
		get => new(actions);
		set => actionsBinding = Register(actionsBinding, value, SetActions);
	}

	internal IReadOnlyList<MenuAction> ActionsValue => actions;


	void SetActions(
		IReadOnlyList<MenuAction>? value)
	{
		IReadOnlyList<MenuAction> next = value ?? [];
		if (ReferenceEquals(actions, next))
			return;

		if (actionsHooked && actions is INotifyCollectionChanged old)
			old.CollectionChanged -= OnActionsChanged;

		actions = next;

		if (actionsHooked && actions is INotifyCollectionChanged live)
			live.CollectionChanged += OnActionsChanged;

		OnChanged();
	}

	void OnActionsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs args) =>
		OnChanged();

	private protected override void OnBindingsAttached()
	{
		if (actionsHooked)
			return;

		actionsHooked = true;

		if (actions is INotifyCollectionChanged live)
			live.CollectionChanged += OnActionsChanged;
	}

	private protected override void OnBindingsDetached()
	{
		if (!actionsHooked)
			return;

		actionsHooked = false;

		if (actions is INotifyCollectionChanged live)
			live.CollectionChanged -= OnActionsChanged;
	}
}

/// <summary>
/// A set of toolbar actions rendered as one visual group.
/// </summary>
/// <remarks>
/// Items are laid out contiguously. On iOS 26 and later, adjacent top-level entries are
/// separated so each group keeps its own Liquid Glass background.
/// </remarks>
public sealed class ToolbarGroup : ToolbarItem
{
	readonly ObservableCollection<ToolbarAction> items = [];

	public ToolbarGroup()
	{
		items.CollectionChanged += (_, _) => OnChanged();
	}

	/// <summary>
	/// The actions in the group. Groups cannot contain other groups.
	/// </summary>
	public IList<ToolbarAction> Items => items;
}
