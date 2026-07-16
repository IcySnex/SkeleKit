using System.Windows.Input;

namespace BareUI;

/// <summary>
/// A tappable button.
/// </summary>
public class Button : Control
{
	/// <summary>
	/// The button's title text.
	/// </summary>
	public Bindable<string?> Text
	{
		get => text;
		set => textBinding = Register(textBinding, value, value => Set(ref text, value, ApplyConfiguration));
	}
	string? text;
	Binding<string?>? textBinding;

	/// <summary>
	/// An SF Symbol name shown alongside the text, or null for none.
	/// </summary>
	public Bindable<string?> Icon
	{
		get => icon;
		set => iconBinding = Register(iconBinding, value, value => Set(ref icon, value, ApplyConfiguration));
	}
	string? icon;
	Binding<string?>? iconBinding;

	/// <summary>
	/// Smaller text shown under the title, or null for none.
	/// </summary>
	public Bindable<string?> Subtitle
	{
		get => subtitle;
		set => subtitleBinding = Register(subtitleBinding, value, value => Set(ref subtitle, value, ApplyConfiguration));
	}
	string? subtitle;
	Binding<string?>? subtitleBinding;

	/// <summary>
	/// The button's native style: plain, gray, tinted or filled.
	/// </summary>
	public ButtonStyle Kind
	{
		get => kind;
		set => Set(ref kind, value, ApplyConfiguration);
	}
	ButtonStyle kind = ButtonStyle.Plain;

	/// <summary>
	/// The built-in size class.
	/// </summary>
	public ButtonSize Size
	{
		get => size;
		set => Set(ref size, value, ApplyConfiguration);
	}
	ButtonSize size = ButtonSize.Medium;

	/// <summary>
	/// Where the icon sits relative to the text.
	/// </summary>
	public IconPlacement IconPlacement
	{
		get => iconPlacement;
		set => Set(ref iconPlacement, value, ApplyConfiguration);
	}
	IconPlacement iconPlacement = IconPlacement.Leading;

	/// <summary>
	/// The icon's point size, or NaN to match the size class.
	/// </summary>
	public double IconSize
	{
		get => iconSize;
		set => Set(ref iconSize, value, ApplyConfiguration);
	}
	double iconSize = double.NaN;

	/// <summary>
	/// Points between the icon (or spinner) and the text.
	/// </summary>
	public double IconSpacing
	{
		get => iconSpacing;
		set => Set(ref iconSpacing, value, ApplyConfiguration);
	}
	double iconSpacing = 8;

	/// <summary>
	/// Padding around the content, or null for the size class default.
	/// </summary>
	public Thickness? Padding
	{
		get => padding;
		set => Set(ref padding, value, ApplyConfiguration);
	}
	Thickness? padding;

	/// <summary>
	/// Styles the button red, for destructive actions.
	/// </summary>
	public bool IsDestructive
	{
		get => isDestructive;
		set => Set(ref isDestructive, value, ApplyConfiguration);
	}
	bool isDestructive;

	/// <summary>
	/// Shows a spinner in place of the icon while true. Bind it to a command's running state.
	/// </summary>
	public Bindable<bool> IsLoading
	{
		get => isLoading;
		set => isLoadingBinding = Register(isLoadingBinding, value, value => Set(ref isLoading, value, ApplyConfiguration, affectsMeasure: false));
	}
	bool isLoading;
	Binding<bool>? isLoadingBinding;

	/// <summary>
	/// Menu entries shown on tap instead of invoking <see cref="Command"/>. Empty for a plain button.
	/// </summary>
	public IList<MenuAction> Menu { get; } = [];

	/// <summary>
	/// When true the <see cref="Menu"/> acts as a popup picker: choosing an entry shows it as the button's title and fires its command.
	/// </summary>
	public bool SelectsFromMenu
	{
		get;
		set => Set(ref field, value, ApplyMenu, affectsMeasure: false);
	}

	/// <summary>
	/// Command invoked on tap; its CanExecute drives the enabled state.
	/// </summary>
	public ICommand? Command
	{
		get => command;
		set => SetCommand(value);
	}
	ICommand? command;

	void SetCommand(
		ICommand? value)
	{
		if (ReferenceEquals(command, value))
			return;

		if (command is not null)
			command.CanExecuteChanged -= OnCanExecuteChanged;

		command = value;

		if (command is not null)
			command.CanExecuteChanged += OnCanExecuteChanged;

		ApplyIsEnabled();
	}

	/// <summary>
	/// The parameter passed to <see cref="Command"/>.
	/// </summary>
	public object? CommandParameter
	{
		get => commandParameter;
		set => Set(ref commandParameter, value, ApplyIsEnabled, affectsMeasure: false);
	}
	object? commandParameter;

	private protected override UIView CreateNative()
	{
		UIButton button = new();
		button.TouchUpInside += (_, _) => OnClicked();

		return button;
	}

	private protected override void ApplyProperties()
	{
		ApplyConfiguration();
		ApplyMenu();
		ApplyIsEnabled();
	}

	private protected override void OnUnrealized()
	{
		if (command is not null)
			command.CanExecuteChanged -= OnCanExecuteChanged;
	}

	UIButton Ui =>
		(UIButton)Native;

	void ApplyConfiguration()
	{
		bool glassy = OperatingSystem.IsIOSVersionAtLeast(26);

		UIButtonConfiguration configuration = kind switch
		{
			ButtonStyle.Gray => UIButtonConfiguration.GrayButtonConfiguration,
			ButtonStyle.Tinted => UIButtonConfiguration.TintedButtonConfiguration,
			ButtonStyle.Filled or ButtonStyle.FilledCapsule => UIButtonConfiguration.FilledButtonConfiguration,
			ButtonStyle.Glass when glassy => UIButtonConfiguration.GlassButtonConfiguration,
			ButtonStyle.ProminentGlass when glassy => UIButtonConfiguration.ProminentGlassButtonConfiguration,
			_ => UIButtonConfiguration.PlainButtonConfiguration
		};

		if (kind is ButtonStyle.FilledCapsule)
			configuration.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;

		configuration.Title = text;
		configuration.Subtitle = subtitle;
		configuration.ButtonSize = size switch
		{
			ButtonSize.Mini => UIButtonConfigurationSize.Mini,
			ButtonSize.Small => UIButtonConfigurationSize.Small,
			ButtonSize.Large => UIButtonConfigurationSize.Large,
			_ => UIButtonConfigurationSize.Medium
		};

		if (icon is not null)
		{
			configuration.Image = UIImage.GetSystemImage(icon);

			// sized to sit beside the title, not at the symbol's free-standing size
			double points = double.IsNaN(iconSize)
				? size switch
				{
					ButtonSize.Mini => 10,
					ButtonSize.Small => 11,
					ButtonSize.Large => 15,
					_ => 13
				}
				: iconSize;

			configuration.PreferredSymbolConfigurationForImage = UIImageSymbolConfiguration.Create(
				(nfloat)points,
				UIImageSymbolWeight.Medium);

			configuration.ImagePlacement = iconPlacement switch
			{
				IconPlacement.Trailing => NSDirectionalRectEdge.Trailing,
				IconPlacement.Top => NSDirectionalRectEdge.Top,
				IconPlacement.Bottom => NSDirectionalRectEdge.Bottom,
				_ => NSDirectionalRectEdge.Leading
			};
		}

		// the spinner takes the image slot, so it needs the same breathing room
		configuration.ShowsActivityIndicator = isLoading;

		if ((icon is not null || isLoading) && text is not null)
			configuration.ImagePadding = (nfloat)iconSpacing;

		if (padding is { } insets)
			configuration.ContentInsets = new NSDirectionalEdgeInsets(
				(nfloat)insets.Top,
				(nfloat)insets.Left,
				(nfloat)insets.Bottom,
				(nfloat)insets.Right);

		bool filled = kind is ButtonStyle.Filled or ButtonStyle.FilledCapsule;

		if (isDestructive)
		{
			configuration.BaseForegroundColor = UIColor.SystemRed;

			if (filled)
			{
				configuration.BaseBackgroundColor = UIColor.SystemRed;
				configuration.BaseForegroundColor = UIColor.White;
			}
		}
		// a configuration paints from its own colors, so an inherited tint has to be written into it
		else if (Tint is { } accent)
		{
			UIColor color = accent.ToUIColor();

			if (filled || kind is ButtonStyle.Tinted)
				configuration.BaseBackgroundColor = color;

			if (!filled)
				configuration.BaseForegroundColor = color;
		}

		Ui.Configuration = configuration;
	}

	internal override void TintChanged()
	{
		if (IsRealized)
			ApplyConfiguration();
	}

	// the actions stay rooted here: UIKit's retain alone would let their managed peers die
	UIAction[]? menuActions;

	void ApplyMenu()
	{
		if (Menu.Count == 0)
			return;

		menuActions = new UIAction[Menu.Count];

		for (int index = 0; index < Menu.Count; index++)
		{
			MenuAction entry = Menu[index];

			menuActions[index] = UIAction.Create(
				entry.Text,
				entry.Icon is { } entryIcon ? UIImage.GetSystemImage(entryIcon) : null,
				null,
				_ =>
				{
					if (entry.Command is { } entryCommand && entryCommand.CanExecute(null))
						entryCommand.Execute(null);
				});

			if (entry.IsDestructive)
				menuActions[index].Attributes = UIMenuElementAttributes.Destructive;
		}

		Ui.Menu = UIMenu.Create(menuActions);
		Ui.ShowsMenuAsPrimaryAction = true;
		Ui.ChangesSelectionAsPrimaryAction = SelectsFromMenu;
	}

	void ApplyIsEnabled()
	{
		if (IsRealized)
			Ui.Enabled = command?.CanExecute(commandParameter) ?? true;
	}

	void OnClicked()
	{
		if (command is { } current && current.CanExecute(commandParameter))
			current.Execute(commandParameter);
	}

	// CanExecuteChanged can fire off-thread
	void OnCanExecuteChanged(
		object? sender,
		EventArgs e) =>
		MainThread.Post(ApplyIsEnabled);
}
