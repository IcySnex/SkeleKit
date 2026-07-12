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
	}

	UITextView Ui => (UITextView)Native;

	void ApplyText() =>
		Ui.Text = text;

	void ApplyFont() =>
		Ui.Font = Fonts.Scaled(fontSize, bold: false);

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
