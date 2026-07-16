using CoreGraphics;
using UIKit;

namespace BareUI;

/// <summary>
/// A multi-line text input.
/// </summary>
public class TextEditor : Control
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
	/// What the editor holds, so the system can offer autofill.
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

	InputAccessory? accessory;

	void ApplyToolbar()
	{
		accessory = keyboardToolbar is KeyboardToolbar.None ? null : new(this, keyboardToolbar);
		Ui.InputAccessoryView = accessory?.Bar;
	}

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


	private protected override UIView CreateNative()
	{
		UITextView view = new()
		{
			Editable = true,
			AdjustsFontForContentSizeCategory = true
		};

		view.Changed += (_, _) => OnChanged();
		view.Ended += (_, _) => OnEditingEnded();

		return view;
	}

	private protected override void ApplyProperties()
	{
		ApplyText();
		ApplyFont();
		ApplyTraits();
		ApplyToolbar();
	}

	UITextView Ui => (UITextView)Native;

	void ApplyText() =>
		Ui.Text = text;

	void ApplyFont() =>
		Ui.Font = Fonts.Scaled(fontSize, fontWeight, fontDesign);

	void ApplyTraits()
	{
		TextField.ApplyContentType(Ui.TextContentType, contentKind, type => Ui.TextContentType = type);

		Ui.AutocapitalizationType = capitalization switch
		{
			Capitalization.None => UITextAutocapitalizationType.None,
			Capitalization.Words => UITextAutocapitalizationType.Words,
			Capitalization.Characters => UITextAutocapitalizationType.AllCharacters,
			_ => UITextAutocapitalizationType.Sentences
		};
		Ui.AutocorrectionType = autocorrection ? UITextAutocorrectionType.Yes : UITextAutocorrectionType.No;
		Ui.SpellCheckingType = autocorrection ? UITextSpellCheckingType.Yes : UITextSpellCheckingType.No;
		Ui.KeyboardAppearance = Keyboards.Appearance(keyboardLook);
	}

	void OnChanged()
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

	// UITextView over-reports empty height; size from content, floor at one line
	protected override Size MeasureOverride(
		Size availableSize)
	{
		UITextView view = Ui;

		CGSize fit = view.SizeThatFits(ClampToFinite(availableSize));

		UIFont font = view.Font ?? Fonts.Scaled(fontSize, bold: false);
		UIEdgeInsets inset = view.TextContainerInset;
		nfloat lineFloor = (nfloat)Math.Ceiling(font.LineHeight) + inset.Top + inset.Bottom;

		nfloat resultHeight = string.IsNullOrEmpty(view.Text)
			? lineFloor
			: (nfloat)Math.Max(fit.Height, lineFloor);

		return new(fit.Width, resultHeight);
	}
}
