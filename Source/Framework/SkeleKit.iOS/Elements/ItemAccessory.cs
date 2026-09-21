using System.Windows.Input;

namespace SkeleKit;

/// <summary>
/// The collection modes that show an <see cref="ItemAccessory"/>.
/// </summary>
public enum AccessoryDisplay
{
	/// <summary>Shown in every mode.</summary>
	Always,

	/// <summary>Shown only while the collection is editing.</summary>
	WhenEditing,

	/// <summary>Shown only while the collection is not editing.</summary>
	WhenNotEditing
}

/// <summary>
/// A native affordance pinned to an item's edge, outside the item content.
/// </summary>
/// <remarks>
/// Add accessories to <see cref="ItemView{TItem}.Accessories"/> in the item template constructor.
/// Each kind maps to a system <c>UICellAccessory</c>, so UIKit owns its glyph, spacing, tint,
/// right-to-left behavior and accessibility. List layouts only.
/// </remarks>
public abstract class ItemAccessory
{
	readonly List<BindingBase> bindings = [];

	private protected ItemAccessory()
	{ }


	internal IItemAccessoryHost? Host { get; private set; }


	/// <summary>
	/// Which collection modes show the accessory. Defaults to <see cref="AccessoryDisplay.Always"/>.
	/// </summary>
	public AccessoryDisplay Display { get; init; } = AccessoryDisplay.Always;

	/// <summary>
	/// Whether the accessory is visible, bindable like any control property. Defaults to <c>true</c>.
	/// </summary>
	/// <remarks>
	/// A hidden accessory takes no layout space, and the row reflows like a native list row.
	/// </remarks>
	public Bindable<bool> IsVisible
	{
		get => isVisible;
		set
		{
			isVisible = value;
			isVisibleBinding = Register(isVisibleBinding, value, resolved => ResolvedIsVisible = resolved);
		}
	}
	Bindable<bool> isVisible = true;
	Binding<bool>? isVisibleBinding;

	/// <summary>
	/// The color applied to the accessory, or null for the system default.
	/// </summary>
	public Bindable<Color?> Tint
	{
		get => tint;
		set
		{
			tint = value;
			tintBinding = Register(tintBinding, value, resolved => ResolvedTint = resolved);
		}
	}
	Bindable<Color?> tint;
	Binding<Color?>? tintBinding;


	internal bool ResolvedIsVisible { get; set; } = true;

	internal Color? ResolvedTint { get; set; }


	internal void Attach(
		IItemAccessoryHost host)
	{
		if (Host is not null)
			throw new InvalidOperationException("This accessory already belongs to an ItemView.");

		Host = host;
		AttachBindings();
	}

	internal void Detach()
	{
		foreach (BindingBase binding in bindings)
			Host?.Untrack(binding);

		bindings.Clear();
		Host = null;
	}


	private protected Binding<T>? Register<T>(
		Binding<T>? existing,
		Bindable<T> value,
		Action<T?> apply)
	{
		if (existing is not null)
		{
			Host?.Untrack(existing);
			bindings.Remove(existing);
		}

		if (Host is not IItemAccessoryHost host)
			return null;

		if (value.Expression is BindingExpression<T> expression)
		{
			Binding<T> binding = new(expression, resolved =>
			{
				apply(resolved);
				host.NotifyChanged();
			});

			bindings.Add(binding);
			host.Track(binding);
			return binding;
		}

		apply(value.Value);
		host.NotifyChanged();
		return null;
	}


	private protected virtual void AttachBindings()
	{
		isVisibleBinding = Register(isVisibleBinding, isVisible, value => ResolvedIsVisible = value);
		tintBinding = Register(tintBinding, tint, value => ResolvedTint = value);
	}
}

/// <summary>
/// A trailing checkmark, tinted by the system green unless <see cref="ItemAccessory.Tint"/> overrides it.
/// </summary>
public sealed class CheckmarkAccessory : ItemAccessory;

/// <summary>
/// A trailing disclosure chevron.
/// </summary>
public sealed class DisclosureAccessory : ItemAccessory;

/// <summary>
/// A trailing info button.
/// </summary>
public sealed class DetailAccessory : ItemAccessory
{
	/// <summary>
	/// The command run with the item when the button is tapped, or null for an inert button.
	/// </summary>
	public Bindable<ICommand?> Command
	{
		get => command;
		set
		{
			command = value;
			commandBinding = Register(commandBinding, value, resolved => ResolvedCommand = resolved);
		}
	}
	Bindable<ICommand?> command;
	Binding<ICommand?>? commandBinding;

	internal ICommand? ResolvedCommand { get; set; }


	private protected override void AttachBindings()
	{
		base.AttachBindings();
		commandBinding = Register(commandBinding, command, value => ResolvedCommand = value);
	}
}

/// <summary>
/// A trailing label, typically a short count or status string.
/// </summary>
public sealed class LabelAccessory : ItemAccessory
{
	/// <summary>
	/// The label text.
	/// </summary>
	public Bindable<string?> Text
	{
		get => text;
		set
		{
			text = value;
			textBinding = Register(textBinding, value, resolved => ResolvedText = resolved);
		}
	}
	Bindable<string?> text;
	Binding<string?>? textBinding;

	/// <summary>
	/// The text style the label scales with. Defaults to <see cref="SkeleKit.TextStyle.Body"/>.
	/// </summary>
	public TextStyle TextStyle { get; init; } = SkeleKit.TextStyle.Body;

	/// <summary>
	/// The label's font weight. Defaults to <see cref="SkeleKit.FontWeight.Regular"/>.
	/// </summary>
	public FontWeight FontWeight { get; init; } = SkeleKit.FontWeight.Regular;

	internal string? ResolvedText { get; set; }


	private protected override void AttachBindings()
	{
		base.AttachBindings();
		textBinding = Register(textBinding, text, value => ResolvedText = value);
	}
}

internal interface IItemAccessoryHost
{
	void Track(
		BindingBase binding);

	void Untrack(
		BindingBase binding);

	void NotifyChanged();
}
