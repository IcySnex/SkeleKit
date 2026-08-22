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
		{
			revealButton.Frame = new(
				revealButton.Frame.X,
				revealButton.Frame.Y,
				glyph.Size.Width,
				glyph.Size.Height);
		}
	}

	void ToggleReveal()
	{
		SetSecure(!Ui.SecureTextEntry);
		UpdateRevealGlyph();
	}

	void SetSecure(
		bool secure)
	{
		if (Ui.SecureTextEntry == secure)
			return;

		string? text = Ui.Text;
		(nint Start, nint End)? selection = SelectionOffsets();
		bool focused = Ui.IsFirstResponder;

		if (secure && focused)
		{
			RunNativeTextUpdate(() =>
			{
				bool enabled = Ui.Enabled;
				Ui.Enabled = false;
				Ui.SecureTextEntry = true;
				Ui.Enabled = enabled;

				if (enabled)
					Ui.BecomeFirstResponder();

				if (text is not null)
				{
					Ui.Text = "";
					Ui.InsertText(text);
				}

				if (selection.HasValue)
					RestoreSelection(selection.Value.Start, selection.Value.End);
			});
		}
		else
			Ui.SecureTextEntry = secure;
	}

	(nint Start, nint End)? SelectionOffsets()
	{
		if (!Ui.IsFirstResponder || Ui.SelectedTextRange is not UITextRange selection)
			return null;

		return (
			Ui.GetOffsetFromPosition(Ui.BeginningOfDocument, selection.Start),
			Ui.GetOffsetFromPosition(Ui.BeginningOfDocument, selection.End));
	}

	void RestoreSelection(
		nint startOffset,
		nint endOffset)
	{
		UITextPosition start = Ui.GetPosition(Ui.BeginningOfDocument, startOffset);
		UITextPosition end = Ui.GetPosition(Ui.BeginningOfDocument, endOffset);

		Ui.SelectedTextRange = Ui.GetTextRange(start, end);
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
			SetSecure(true);
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

		if (!ReferenceEquals(Ui.RightView, revealButton))
			Ui.RightView = revealButton;

		Ui.RightViewMode = UITextFieldViewMode.Always;
		UpdateRevealGlyph();
	}
}
