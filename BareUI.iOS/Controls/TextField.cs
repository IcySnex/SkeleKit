using UIKit;

namespace BareUI;

/// <summary>
/// A single-line text input wrapping <c>UITextField</c>.
/// </summary>
public class TextField : Control
{
	/// <summary>
	/// The current text.
	/// </summary>
	public string? Text { get; set; }

	/// <summary>
	/// Placeholder text shown when empty.
	/// </summary>
	public string? Placeholder { get; set; }

	/// <summary>
	/// Which on-screen keyboard to show while editing.
	/// </summary>
	public KeyboardType Keyboard { get; set; } = KeyboardType.Default;

	/// <summary>
	/// The label shown on the keyboard's return key.
	/// </summary>
	public ReturnKeyType ReturnKey { get; set; } = ReturnKeyType.Default;

	/// <summary>
	/// Font size in points.
	/// </summary>
	public double FontSize { get; set; } = 17;

	/// <summary>
	/// Invoked with the new value whenever the text changes.
	/// </summary>
	public Action<string>? TextChanged { get; set; }

	/// <summary>
	/// Invoked when the user taps the keyboard's return key.
	/// </summary>
	public Action? OnSubmit { get; set; }

	private protected override UIView CreateNative()
	{
		UITextField field = new()
		{
			Text = Text,
			Placeholder = Placeholder,
			Font = UIFont.SystemFontOfSize((nfloat)FontSize),
			BorderStyle = UITextBorderStyle.RoundedRect,
			KeyboardType = Keyboard switch
			{
				KeyboardType.Numeric => UIKeyboardType.NumberPad,
				KeyboardType.Decimal => UIKeyboardType.DecimalPad,
				KeyboardType.Phone => UIKeyboardType.PhonePad,
				KeyboardType.Email => UIKeyboardType.EmailAddress,
				KeyboardType.Url => UIKeyboardType.Url,
				_ => UIKeyboardType.Default
			},
			ReturnKeyType = ReturnKey switch
			{
				ReturnKeyType.Go => UIReturnKeyType.Go,
				ReturnKeyType.Next => UIReturnKeyType.Next,
				ReturnKeyType.Search => UIReturnKeyType.Search,
				ReturnKeyType.Send => UIReturnKeyType.Send,
				ReturnKeyType.Done => UIReturnKeyType.Done,
				_ => UIReturnKeyType.Default
			}
		};

		field.EditingChanged += (sender, e) =>
		{
			Text = field.Text;
			TextChanged?.Invoke(Text ?? "");
		};

		field.ShouldReturn = textField =>
		{
			textField.ResignFirstResponder();
			OnSubmit?.Invoke();
			return true;
		};

		return field;
	}
}
