namespace BareUI;

/// <summary>
/// The standard colours, so app code never reaches for a hex literal.
/// </summary>
public static class Colors
{
	public static Color Transparent => new(0, 0, 0, 0);

	public static Color Black => Color.FromHex(0x000000);

	public static Color White => Color.FromHex(0xFFFFFF);

	public static Color Red => Color.FromHex(0xFF3B30);

	public static Color Orange => Color.FromHex(0xFF9500);

	public static Color Yellow => Color.FromHex(0xFFCC00);

	public static Color Green => Color.FromHex(0x34C759);

	public static Color Mint => Color.FromHex(0x00C7BE);

	public static Color Teal => Color.FromHex(0x30B0C7);

	public static Color Cyan => Color.FromHex(0x32ADE6);

	public static Color Blue => Color.FromHex(0x007AFF);

	public static Color Indigo => Color.FromHex(0x5856D6);

	public static Color Purple => Color.FromHex(0xAF52DE);

	public static Color Pink => Color.FromHex(0xFF2D55);

	public static Color Brown => Color.FromHex(0xA2845E);

	public static Color Gray => Color.FromHex(0x8E8E93);

	public static Color LightGray => Color.FromHex(0xC7C7CC);

	public static Color DarkGray => Color.FromHex(0x48484A);
}
