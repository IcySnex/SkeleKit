using UIKit;

namespace BareUI;

internal static class Keyboards
{
	public static UIKeyboardAppearance Appearance(
		KeyboardLook look) =>
		look switch
		{
			KeyboardLook.Light => UIKeyboardAppearance.Light,
			KeyboardLook.Dark => UIKeyboardAppearance.Dark,
			_ => UIKeyboardAppearance.Default
		};
}
