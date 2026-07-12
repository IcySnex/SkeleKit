namespace BareUI;

/// <summary>
/// The standard colours, so app code never reaches for a hex literal. All adapt to dark mode; the hex values are the light-mode fallback.
/// </summary>
public static class Colors
{
	public static Color Transparent => new(0, 0, 0, 0);

	public static Color Black => Color.FromHex(0x000000);

	public static Color White => Color.FromHex(0xFFFFFF);

	public static Color Red => System(SystemColor.Red, 0xFF3B30);

	public static Color Orange => System(SystemColor.Orange, 0xFF9500);

	public static Color Yellow => System(SystemColor.Yellow, 0xFFCC00);

	public static Color Green => System(SystemColor.Green, 0x34C759);

	public static Color Mint => System(SystemColor.Mint, 0x00C7BE);

	public static Color Teal => System(SystemColor.Teal, 0x30B0C7);

	public static Color Cyan => System(SystemColor.Cyan, 0x32ADE6);

	public static Color Blue => System(SystemColor.Blue, 0x007AFF);

	public static Color Indigo => System(SystemColor.Indigo, 0x5856D6);

	public static Color Purple => System(SystemColor.Purple, 0xAF52DE);

	public static Color Pink => System(SystemColor.Pink, 0xFF2D55);

	public static Color Brown => System(SystemColor.Brown, 0xA2845E);

	public static Color Gray => System(SystemColor.Gray, 0x8E8E93);

	public static Color Gray2 => System(SystemColor.Gray2, 0xAEAEB2);

	public static Color Gray3 => System(SystemColor.Gray3, 0xC7C7CC);

	public static Color Gray4 => System(SystemColor.Gray4, 0xD1D1D6);

	public static Color Gray5 => System(SystemColor.Gray5, 0xE5E5EA);

	public static Color Gray6 => System(SystemColor.Gray6, 0xF2F2F7);

	public static Color LightGray => Gray3;

	public static Color DarkGray => Color.FromHex(0x48484A);


	// Semantic text

	/// <summary>Primary text.</summary>
	public static Color Label => System(SystemColor.Label, 0x000000);

	/// <summary>Secondary text: subtitles, footnotes.</summary>
	public static Color SecondaryLabel => System(SystemColor.SecondaryLabel, 0x3C3C43);

	/// <summary>Tertiary text: disabled or placeholder-adjacent.</summary>
	public static Color TertiaryLabel => System(SystemColor.TertiaryLabel, 0x3C3C43);

	/// <summary>Placeholder text in empty fields.</summary>
	public static Color PlaceholderText => System(SystemColor.PlaceholderText, 0x3C3C43);

	/// <summary>Thin dividing lines.</summary>
	public static Color Separator => System(SystemColor.Separator, 0xC6C6C8);

	/// <summary>Tappable link text.</summary>
	public static Color Link => System(SystemColor.Link, 0x007AFF);


	// Semantic backgrounds

	/// <summary>The main page background.</summary>
	public static Color Background => System(SystemColor.Background, 0xFFFFFF);

	/// <summary>Content layered on the main background (a card).</summary>
	public static Color SecondaryBackground => System(SystemColor.SecondaryBackground, 0xF2F2F7);

	/// <summary>Content layered on a secondary background.</summary>
	public static Color TertiaryBackground => System(SystemColor.TertiaryBackground, 0xFFFFFF);

	/// <summary>The page background behind grouped lists (Settings).</summary>
	public static Color GroupedBackground => System(SystemColor.GroupedBackground, 0xF2F2F7);

	/// <summary>A cell on a grouped background.</summary>
	public static Color SecondaryGroupedBackground => System(SystemColor.SecondaryGroupedBackground, 0xFFFFFF);

	/// <summary>Content layered on a grouped cell.</summary>
	public static Color TertiaryGroupedBackground => System(SystemColor.TertiaryGroupedBackground, 0xF2F2F7);


	static Color System(
		SystemColor system,
		uint lightHex) =>
		Color.FromHex(lightHex) with { System = system };
}
