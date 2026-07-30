namespace SkeleKit;

/// <summary>
/// Secure-entry preset of <see cref="TextField"/>, masking input as it's typed.
/// </summary>
public class SecureField : TextField
{
	UIButton? revealButton;


	/// <summary>
	/// Whether a trailing eye button toggles the masking of the entered text.
	/// </summary>
	/// <remarks>
	/// Owns the trailing slot, so it wins over <see cref="TextField.TrailingIcon"/>.
	/// Turning it off restores masking.
	/// </remarks>
	public bool RevealButton
	{
		get;
		set => Set(ref field, value, ApplyTrailing);
	}


	void UpdateRevealGlyph()
	{
		if (revealButton is null)
			return;

		UIImage? glyph = UIImage.GetSystemImage(Ui.SecureTextEntry ? "eye" : "eye.slash", IconConfiguration);
		revealButton.SetImage(glyph, UIControlState.Normal);

		if (glyph is not null)
			revealButton.Frame = new(0, 0, glyph.Size.Width, glyph.Size.Height);
	}

	void ToggleReveal()
	{
		Ui.SecureTextEntry = !Ui.SecureTextEntry;

		if (Ui.IsFirstResponder && Ui.Text is string text)
		{
			Ui.Text = "";
			Ui.Text = text;
		}

		UpdateRevealGlyph();
	}


	private protected override UIView CreateNative()
	{
		UITextField field = (UITextField)base.CreateNative();
		field.SecureTextEntry = true;

		return field;
	}

	private protected override void ApplyTrailing()
	{
		if (!RevealButton)
		{
			Ui.SecureTextEntry = true;
			revealButton = null;
			base.ApplyTrailing();
			return;
		}

		if (revealButton is null)
		{
			revealButton = new();
			revealButton.TintColor = UIColor.SecondaryLabel;
			revealButton.TouchUpInside += (_, _) => ToggleReveal();
		}

		Ui.RightView = revealButton;
		Ui.RightViewMode = UITextFieldViewMode.Always;
		UpdateRevealGlyph();
	}
}
