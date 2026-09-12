using System.Windows.Input;

namespace SkeleKit;

/// <summary>
/// A tappable button.
/// </summary>
public class Button : Control
{
	UIButton Ui => (UIButton)Native;


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
	/// The local icon shown alongside the text, or null for none.
	/// </summary>
	public Bindable<ImageSource?> Icon
	{
		get => icon;
		set => iconBinding = Register(iconBinding, value, value => Set(ref icon, value, ApplyConfiguration));
	}
	ImageSource? icon;
	Binding<ImageSource?>? iconBinding;

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
		set => isLoadingBinding = Register(isLoadingBinding, value, value => Set(ref isLoading, value, ApplyConfiguration));
	}
	bool isLoading;
	Binding<bool>? isLoadingBinding;

	/// <summary>
	/// Menu entries shown on tap instead of invoking <see cref="Command"/>. Empty for a plain button.
	/// </summary>
	public IList<MenuAction> Menu { get; } = [];

	/// <summary>
	/// Command invoked on tap; its CanExecute drives the enabled state.
	/// </summary>
	public ICommand? Command
	{
		get => command;
		set => SetCommand(value);
	}
	ICommand? command;

	/// <summary>
	/// The parameter passed to <see cref="Command"/>.
	/// </summary>
	public object? CommandParameter
	{
		get => commandParameter;
		set => Set(ref commandParameter, value, ApplyIsEnabled, affectsMeasure: false);
	}
	object? commandParameter;


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

	void ApplyConfiguration()
	{
		UIButtonConfiguration configuration = NativeButtonConfiguration.Create(kind, Tint, isDestructive);

		configuration.Title = text;
		configuration.Subtitle = subtitle;
		configuration.TitleLineBreakMode = UILineBreakMode.TailTruncation;
		configuration.SubtitleLineBreakMode = UILineBreakMode.TailTruncation;
		NativeButtonConfiguration.ApplyLayout(configuration, size, padding);

		if (icon is ImageSource iconSource)
		{
			configuration.Image = iconSource.ResolveLocal();

			double points = size switch
			{
				ButtonSize.Mini => 10,
				ButtonSize.Small => 11,
				ButtonSize.Large => 15,
				_ => 13
			};

			configuration.PreferredSymbolConfigurationForImage = iconSource.CreateSymbolConfiguration(
				points,
				FontWeight.Medium);

			configuration.ImagePlacement = iconPlacement switch
			{
				IconPlacement.Trailing => NSDirectionalRectEdge.Trailing,
				IconPlacement.Top => NSDirectionalRectEdge.Top,
				IconPlacement.Bottom => NSDirectionalRectEdge.Bottom,
				_ => NSDirectionalRectEdge.Leading
			};
		}

		configuration.ShowsActivityIndicator = isLoading;

		if ((icon is not null || isLoading) && text is not null)
			configuration.ImagePadding = (nfloat)iconSpacing;

		Ui.Configuration = configuration;
		RefreshConfigurationLayout();
	}

	void ApplyMenu()
	{
		if (Menu.Count == 0)
			return;

		UIAction[] actions = new UIAction[Menu.Count];

		for (int index = 0; index < Menu.Count; index++)
		{
			MenuAction entry = Menu[index];

			actions[index] = UIAction.Create(
				entry.Text,
				entry.Icon?.ResolveLocal(),
				null,
				_ =>
				{
					if (entry.Command is ICommand entryCommand && entryCommand.CanExecute(entry.CommandParameter))
						entryCommand.Execute(entry.CommandParameter);
				});

			if (entry.IsDestructive)
				actions[index].Attributes = UIMenuElementAttributes.Destructive;
		}

		Ui.Menu = UIMenu.Create(actions);
		Ui.ShowsMenuAsPrimaryAction = true;
	}

	void RefreshConfigurationLayout()
	{
		Ui.InvalidateIntrinsicContentSize();
		Ui.SetNeedsLayout();
		Ui.LayoutIfNeeded();
	}

	void ApplyIsEnabled()
	{
		if (IsRealized)
			Ui.Enabled = command?.CanExecute(commandParameter) ?? true;
	}

	void OnClicked()
	{
		if (command is ICommand current && current.CanExecute(commandParameter))
			current.Execute(commandParameter);
	}

	// CanExecuteChanged can fire off-thread
	void OnCanExecuteChanged(
		object? sender,
		EventArgs e) =>
		MainThread.Post(ApplyIsEnabled);


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


	internal override void TintChanged()
	{
		if (IsRealized)
			ApplyConfiguration();
	}
}
