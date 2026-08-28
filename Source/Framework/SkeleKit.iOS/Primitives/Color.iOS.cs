
namespace SkeleKit;

internal static class ColorInterop
{
	static UIColor Rgba(
		double red,
		double green,
		double blue,
		double alpha) =>
		UIColor.FromRGBA((nfloat)red, (nfloat)green, (nfloat)blue, (nfloat)alpha);

	static UIColor Resolve(
		SystemColor system) =>
		system switch
		{
			SystemColor.Red => UIColor.SystemRed,
			SystemColor.Orange => UIColor.SystemOrange,
			SystemColor.Yellow => UIColor.SystemYellow,
			SystemColor.Green => UIColor.SystemGreen,
			SystemColor.Mint => UIColor.SystemMint,
			SystemColor.Teal => UIColor.SystemTeal,
			SystemColor.Cyan => UIColor.SystemCyan,
			SystemColor.Blue => UIColor.SystemBlue,
			SystemColor.Indigo => UIColor.SystemIndigo,
			SystemColor.Purple => UIColor.SystemPurple,
			SystemColor.Pink => UIColor.SystemPink,
			SystemColor.Brown => UIColor.SystemBrown,
			SystemColor.Gray => UIColor.SystemGray,
			SystemColor.Gray2 => UIColor.SystemGray2,
			SystemColor.Gray3 => UIColor.SystemGray3,
			SystemColor.Gray4 => UIColor.SystemGray4,
			SystemColor.Gray5 => UIColor.SystemGray5,
			SystemColor.Gray6 => UIColor.SystemGray6,
			SystemColor.Label => UIColor.Label,
			SystemColor.SecondaryLabel => UIColor.SecondaryLabel,
			SystemColor.TertiaryLabel => UIColor.TertiaryLabel,
			SystemColor.PlaceholderText => UIColor.PlaceholderText,
			SystemColor.Separator => UIColor.Separator,
			SystemColor.Link => UIColor.Link,
			SystemColor.Background => UIColor.SystemBackground,
			SystemColor.SecondaryBackground => UIColor.SecondarySystemBackground,
			SystemColor.TertiaryBackground => UIColor.TertiarySystemBackground,
			SystemColor.GroupedBackground => UIColor.SystemGroupedBackground,
			SystemColor.SecondaryGroupedBackground => UIColor.SecondarySystemGroupedBackground,
			_ => UIColor.TertiarySystemGroupedBackground
		};

	
	// ReSharper disable once InconsistentNaming
	public static UIColor ToUIColor(
		this Color color)
	{
		if (color.System is SystemColor system)
			return Resolve(system).ColorWithAlpha((nfloat)color.Alpha);

		if (color.Dark.HasValue)
		{
			(double Red, double Green, double Blue, double Alpha) dark = color.Dark.Value;

			UIColor light = Rgba(color.Red, color.Green, color.Blue, color.Alpha);
			UIColor darker = Rgba(dark.Red, dark.Green, dark.Blue, dark.Alpha);

			return UIColor.FromDynamicProvider(traits =>
				traits.UserInterfaceStyle is UIUserInterfaceStyle.Dark ? darker : light);
		}

		return Rgba(color.Red, color.Green, color.Blue, color.Alpha);
	}

	public static Color ToColor(
		this UIColor color)
	{
		color.GetRGBA(out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha);

		return new(red, green, blue, alpha);
	}
}
