#if IOS
using UIKit;
#endif

namespace BareUI;

/// <summary>
/// Secure-entry preset of <see cref="TextField"/>, masking input as it's typed.
/// </summary>
public class SecureField : TextField
{
#if IOS
	private protected override UIView CreateNative()
	{
		UITextField field = (UITextField)base.CreateNative();
		field.SecureTextEntry = true;

		return field;
	}
#endif
}
