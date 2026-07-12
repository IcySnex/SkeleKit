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
	/// The button's native style: plain, gray, tinted or filled.
	/// </summary>
	public ButtonStyle Kind
	{
		get => kind;
		set => Set(ref kind, value, ApplyConfiguration);
	}
	ButtonStyle kind = ButtonStyle.Plain;

	/// <summary>
	/// Command invoked on tap; its CanExecute drives the enabled state.
	/// </summary>
	public Bindable<ICommand?> Command
	{
		get => Bindable.From<ICommand?>(command);
		set => commandBinding = Register(commandBinding, value, SetCommand);
	}
	ICommand? command;
	Binding<ICommand?>? commandBinding;

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

	/// <summary>
	/// Invoked when the button is tapped.
	/// </summary>
	public Action? Clicked { get; set; }


	private protected override UIView CreateNative()
	{
		UIButton button = new();
		button.TouchUpInside += (_, _) => OnClicked();

		return button;
	}

	private protected override void ApplyProperties()
	{
		ApplyConfiguration();
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
		UIButtonConfiguration configuration = kind switch
		{
			ButtonStyle.Gray => UIButtonConfiguration.GrayButtonConfiguration,
			ButtonStyle.Tinted => UIButtonConfiguration.TintedButtonConfiguration,
			ButtonStyle.Filled or ButtonStyle.FilledCapsule => UIButtonConfiguration.FilledButtonConfiguration,
			_ => UIButtonConfiguration.PlainButtonConfiguration
		};

		if (kind is ButtonStyle.FilledCapsule)
			configuration.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;

		configuration.Title = text;

		if (icon is not null)
		{
			configuration.Image = UIImage.GetSystemImage(icon);
			if (text is not null)
				configuration.ImagePadding = 6;
		}

		Ui.Configuration = configuration;
	}

	void ApplyIsEnabled()
	{
		if (IsRealized)
			Ui.Enabled = command?.CanExecute(commandParameter) ?? true;
	}

	void OnClicked()
	{
		Clicked?.Invoke();

		if (command is { } current && current.CanExecute(commandParameter))
			current.Execute(commandParameter);
	}

	// CanExecuteChanged can fire off-thread
	void OnCanExecuteChanged(
		object? sender,
		EventArgs e) =>
		MainThread.Post(ApplyIsEnabled);
}
