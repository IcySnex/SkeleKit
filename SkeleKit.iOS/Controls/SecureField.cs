namespace SkeleKit;

/// <summary>
/// Secure-entry preset of <see cref="TextField"/>, masking input as it's typed.
/// </summary>
public class SecureField : TextField
{
	private protected override UIView CreateNative()
	{
		UITextField field = (UITextField)base.CreateNative();
		field.SecureTextEntry = true;

		return field;
	}
}
