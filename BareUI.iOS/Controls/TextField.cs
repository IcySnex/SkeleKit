using UIKit;

namespace BareUI;

/// <summary>
/// A single-line text input wrapping <c>UITextField</c>.
/// </summary>
public class TextField : Control
{
	/// <summary>
	/// The current text. Two-way by default.
	/// </summary>
	public Bindable<string?> Text
	{
		get => text;
		set => textBinding = Register(textBinding, value, value => Set(ref text, value, ApplyText));
	}
	string? text;
	Binding<string?>? textBinding;

	/// <summary>
	/// Placeholder text shown when empty.
	/// </summary>
	public Bindable<string?> Placeholder
	{
		get => placeholder;
		set => placeholderBinding = Register(placeholderBinding, value, value => Set(ref placeholder, value, ApplyPlaceholder));
	}
	string? placeholder;
	Binding<string?>? placeholderBinding;

	/// <summary>
	/// Which on-screen keyboard to show while editing.
	/// </summary>
	public KeyboardType Keyboard
	{
		get => keyboard;
		set => Set(ref keyboard, value, ApplyKeyboard, affectsMeasure: false);
	}
	KeyboardType keyboard = KeyboardType.Default;

	/// <summary>
	/// The label shown on the keyboard's return key.
	/// </summary>
	public ReturnKeyType ReturnKey
	{
		get => returnKey;
		set => Set(ref returnKey, value, ApplyReturnKey, affectsMeasure: false);
	}
	ReturnKeyType returnKey = ReturnKeyType.Default;

	/// <summary>
	/// Font size in points.
	/// </summary>
	public Bindable<double> FontSize
	{
		get => fontSize;
		set => fontSizeBinding = Register(fontSizeBinding, value, value => Set(ref fontSize, value, ApplyFont));
	}
	double fontSize = 17;
	Binding<double>? fontSizeBinding;

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
			BorderStyle = UITextBorderStyle.RoundedRect,
			AdjustsFontForContentSizeCategory = true
		};

		field.EditingChanged += (sender, e) => OnEdited();
		field.EditingDidEnd += (sender, e) => OnEditingEnded();

		field.ShouldReturn = textField =>
		{
			textField.ResignFirstResponder();
			OnSubmit?.Invoke();

			return true;
		};

		return field;
	}

	private protected override void ApplyProperties()
	{
		ApplyText();
		ApplyPlaceholder();
		ApplyFont();
		ApplyKeyboard();
		ApplyReturnKey();
	}

	UITextField Ui =>
		(UITextField)Native;

	void ApplyText() =>
		Ui.Text = text;

	void ApplyPlaceholder() =>
		Ui.Placeholder = placeholder;

	void ApplyFont() =>
		Ui.Font = Fonts.Scaled(fontSize, bold: false);

	void ApplyKeyboard() =>
		Ui.KeyboardType = keyboard switch
		{
			KeyboardType.Numeric => UIKeyboardType.NumberPad,
			KeyboardType.Decimal => UIKeyboardType.DecimalPad,
			KeyboardType.Phone => UIKeyboardType.PhonePad,
			KeyboardType.Email => UIKeyboardType.EmailAddress,
			KeyboardType.Url => UIKeyboardType.Url,
			_ => UIKeyboardType.Default
		};

	void ApplyReturnKey() =>
		Ui.ReturnKeyType = returnKey switch
		{
			ReturnKeyType.Go => UIReturnKeyType.Go,
			ReturnKeyType.Next => UIReturnKeyType.Next,
			ReturnKeyType.Search => UIReturnKeyType.Search,
			ReturnKeyType.Send => UIReturnKeyType.Send,
			ReturnKeyType.Done => UIReturnKeyType.Done,
			_ => UIReturnKeyType.Default
		};

	void OnEdited()
	{
		string? value = Ui.Text;

		Set(ref text, value);
		TextChanged?.Invoke(value ?? "");

		if (textBinding?.Trigger is UpdateTrigger.PropertyChanged)
			textBinding.PushToSource(value);
	}

	void OnEditingEnded()
	{
		if (textBinding?.Trigger is UpdateTrigger.FocusLost)
			textBinding.PushToSource(Ui.Text);
	}
}
