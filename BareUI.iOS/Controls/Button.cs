using System.Windows.Input;
#if IOS
using UIKit;
#endif

namespace BareUI;

/// <summary>
/// A tappable button wrapping <c>UIButton</c> built from a UIButtonConfiguration.
/// </summary>
public class Button : Control
{
	/// <summary>
	/// The button's title text.
	/// </summary>
	public string? Text { get; set; }

	/// <summary>
	/// An SF Symbol name shown alongside the text, or null for none.
	/// </summary>
	public string? Icon { get; set; }

	/// <summary>
	/// The button's visual style.
	/// </summary>
	public ButtonStyle Style { get; set; } = ButtonStyle.Plain;

	/// <summary>
	/// Command invoked on tap; its CanExecute drives the enabled state.
	/// </summary>
	public ICommand? Command { get; set; }

	/// <summary>
	/// The parameter passed to <see cref="Command"/>.
	/// </summary>
	public object? CommandParameter { get; set; }

	/// <summary>
	/// Invoked when the button is tapped.
	/// </summary>
	public Action? Clicked { get; set; }

#if IOS
	EventHandler? canExecuteHandler;

	private protected override UIView CreateNative()
	{
		UIButtonConfiguration configuration = Style switch
		{
			ButtonStyle.Gray => UIButtonConfiguration.GrayButtonConfiguration,
			ButtonStyle.Tinted => UIButtonConfiguration.TintedButtonConfiguration,
			ButtonStyle.Filled or ButtonStyle.FilledCapsule => UIButtonConfiguration.FilledButtonConfiguration,
			_ => UIButtonConfiguration.PlainButtonConfiguration
		};

		if (Style is ButtonStyle.FilledCapsule)
			configuration.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;

		configuration.Title = Text;

		if (Icon is not null)
		{
			configuration.Image = UIImage.GetSystemImage(Icon);
			if (Text is not null)
				configuration.ImagePadding = 6;
		}

		UIButton button = new()
		{
			Configuration = configuration
		};

		button.TouchUpInside += (sender, e) =>
		{
			Clicked?.Invoke();
			if (Command is { } command && command.CanExecute(CommandParameter))
				command.Execute(CommandParameter);
		};

		return button;
	}

	private protected override void OnRealized()
	{
		if (Command is null)
			return;

		canExecuteHandler = (sender, e) => UpdateEnabled();
		Command.CanExecuteChanged += canExecuteHandler;
		UpdateEnabled();
	}

	private protected override void OnUnrealized()
	{
		if (Command is not null && canExecuteHandler is not null)
			Command.CanExecuteChanged -= canExecuteHandler;

		canExecuteHandler = null;
	}

	// CanExecuteChanged can fire off-thread
	void UpdateEnabled()
	{
		bool enabled = Command?.CanExecute(CommandParameter) ?? true;

		UIApplication.SharedApplication.BeginInvokeOnMainThread(() =>
		{
			if (!IsRealized)
				return;

			((UIButton)Native).Enabled = enabled;
		});
	}
#endif
}
