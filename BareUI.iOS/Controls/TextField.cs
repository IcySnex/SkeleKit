using System.Windows.Input;

namespace BareUI;

/// <summary>
/// A single-line text input wrapping.
/// </summary>
public class TextField : Control
{
	(UIToolbar Bar, UIBarButtonItem[] Items)? accessoryBar;
	AccessoryHost? accessoryHost;


	UITextField Ui => (UITextField)Native;


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
	/// What the field holds, so the system can offer autofill (passwords, one-time codes, contacts).
	/// </summary>
	public ContentKind ContentKind
	{
		get => contentKind;
		set => Set(ref contentKind, value, ApplyTraits, affectsMeasure: false);
	}
	ContentKind contentKind;

	/// <summary>
	/// When typing is automatically capitalized.
	/// </summary>
	public Capitalization Capitalization
	{
		get => capitalization;
		set => Set(ref capitalization, value, ApplyTraits, affectsMeasure: false);
	}
	Capitalization capitalization = Capitalization.Sentences;

	/// <summary>
	/// Whether the keyboard autocorrects and spell-checks the input.
	/// </summary>
	public bool Autocorrection
	{
		get => autocorrection;
		set => Set(ref autocorrection, value, ApplyTraits, affectsMeasure: false);
	}
	bool autocorrection = true;

	/// <summary>
	/// When the field shows its built-in clear button.
	/// </summary>
	public ClearButton ClearButton
	{
		get => clearButton;
		set => Set(ref clearButton, value, ApplyTraits, affectsMeasure: false);
	}
	ClearButton clearButton;

	/// <summary>
	/// Whether the return key is disabled while the field is empty.
	/// </summary>
	public bool RequiresText
	{
		get => requiresText;
		set => Set(ref requiresText, value, ApplyTraits, affectsMeasure: false);
	}
	bool requiresText;

	/// <summary>
	/// The color scheme of the raised keyboard.
	/// </summary>
	public KeyboardLook KeyboardLook
	{
		get => keyboardLook;
		set => Set(ref keyboardLook, value, ApplyTraits, affectsMeasure: false);
	}
	KeyboardLook keyboardLook = KeyboardLook.Default;

	/// <summary>
	/// A bar above the raised keyboard with Done and optional previous/next arrows.
	/// </summary>
	public KeyboardToolbar KeyboardToolbar
	{
		get => keyboardToolbar;
		set => Set(ref keyboardToolbar, value, ApplyToolbar, affectsMeasure: false);
	}
	KeyboardToolbar keyboardToolbar;

	/// <summary>
	/// A custom view above the raised keyboard.
	/// </summary>
	/// <remarks>
	/// Wins over <see cref="KeyboardToolbar"/>; one view per field.
	/// </remarks>
	public View? KeyboardAccessory
	{
		get => keyboardAccessory;
		set => Set(ref keyboardAccessory, value, ApplyToolbar, affectsMeasure: false);
	}
	View? keyboardAccessory;

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
	/// The weight the text is drawn at.
	/// </summary>
	public FontWeight FontWeight
	{
		get => fontWeight;
		set => Set(ref fontWeight, value, ApplyFont, affectsMeasure: false);
	}
	FontWeight fontWeight = FontWeight.Regular;

	/// <summary>
	/// The system font design the text uses.
	/// </summary>
	public FontDesign FontDesign
	{
		get => fontDesign;
		set => Set(ref fontDesign, value, ApplyFont, affectsMeasure: false);
	}
	FontDesign fontDesign;

	/// <summary>
	/// Invoked with the new value whenever the text changes.
	/// </summary>
	public Action<string>? TextChanged { get; set; }

	/// <summary>
	/// Command invoked when the user taps the keyboard's return key.
	/// </summary>
	public ICommand? SubmitCommand { get; set; }


	void ApplyText() =>
		Ui.Text = text;

	void ApplyPlaceholder() =>
		Ui.Placeholder = placeholder;

	void ApplyFont() =>
		Ui.Font = Fonts.Scaled(fontSize, fontWeight, fontDesign);

	void ApplyToolbar() =>
		Ui.InputAccessoryView = keyboardAccessory is View custom
			? (accessoryHost ??= AccessoryHost.ForKeyboard(custom))
			: keyboardToolbar is KeyboardToolbar.None
				? null
				: (accessoryBar ??= Keyboards.Toolbar(this, keyboardToolbar)).Bar;

	void ApplyTraits()
	{
		ApplyContentType(Ui.TextContentType, contentKind, type => Ui.TextContentType = type);

		Ui.ClearButtonMode = clearButton switch
		{
			ClearButton.WhileEditing => UITextFieldViewMode.WhileEditing,
			ClearButton.UnlessEditing => UITextFieldViewMode.UnlessEditing,
			ClearButton.Always => UITextFieldViewMode.Always,
			_ => UITextFieldViewMode.Never
		};
		Ui.AutocapitalizationType = capitalization switch
		{
			Capitalization.None => UITextAutocapitalizationType.None,
			Capitalization.Words => UITextAutocapitalizationType.Words,
			Capitalization.Characters => UITextAutocapitalizationType.AllCharacters,
			_ => UITextAutocapitalizationType.Sentences
		};
		Ui.AutocorrectionType = autocorrection ? UITextAutocorrectionType.Yes : UITextAutocorrectionType.No;
		Ui.SpellCheckingType = autocorrection ? UITextSpellCheckingType.Yes : UITextSpellCheckingType.No;
		Ui.EnablesReturnKeyAutomatically = requiresText;
		Ui.KeyboardAppearance = Keyboards.Appearance(keyboardLook);
	}

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


	private protected override UIView CreateNative()
	{
		UITextField field = new()
		{
			BorderStyle = UITextBorderStyle.RoundedRect,
			AdjustsFontForContentSizeCategory = true
		};

		field.EditingChanged += (_, _) => OnEdited();
		field.EditingDidEnd += (_, _) => OnEditingEnded();

		field.ShouldReturn = textField =>
		{
			textField.ResignFirstResponder();
			if (SubmitCommand is ICommand submit && submit.CanExecute(null))
				submit.Execute(null);

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
		ApplyTraits();
		ApplyToolbar();
	}


	// empty string clears a previously set content type
	internal static void ApplyContentType(
		NSString? current,
		ContentKind kind,
		Action<NSString> assign)
	{
		NSString? type = kind switch
		{
			ContentKind.Username => UITextContentType.Username,
			ContentKind.Password => UITextContentType.Password,
			ContentKind.NewPassword => UITextContentType.NewPassword,
			ContentKind.OneTimeCode => UITextContentType.OneTimeCode,
			ContentKind.Email => UITextContentType.EmailAddress,
			ContentKind.Name => UITextContentType.Name,
			ContentKind.PhoneNumber => UITextContentType.TelephoneNumber,
			ContentKind.StreetAddress => UITextContentType.FullStreetAddress,
			ContentKind.Url => UITextContentType.Url,
			_ => null
		};

		if (type is not null)
			assign(type);
		else if (current is not null)
			assign(new(""));
	}
}
