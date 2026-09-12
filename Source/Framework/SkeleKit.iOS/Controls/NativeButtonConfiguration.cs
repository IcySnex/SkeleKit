namespace SkeleKit;

internal static class NativeButtonConfiguration
{
	public static UIButtonConfiguration Create(
		ButtonStyle kind,
		Color? tint = null,
		bool destructive = false)
	{
		bool glassy = OperatingSystem.IsIOSVersionAtLeast(26);

		UIButtonConfiguration configuration = kind switch
		{
			ButtonStyle.Gray => UIButtonConfiguration.GrayButtonConfiguration,
			ButtonStyle.Tinted => UIButtonConfiguration.TintedButtonConfiguration,
			ButtonStyle.Filled or ButtonStyle.FilledCapsule => UIButtonConfiguration.FilledButtonConfiguration,
			ButtonStyle.Glass when glassy => UIButtonConfiguration.GlassButtonConfiguration,
			ButtonStyle.ProminentGlass => glassy
				? UIButtonConfiguration.ProminentGlassButtonConfiguration
				: UIButtonConfiguration.FilledButtonConfiguration,
			ButtonStyle.ClearGlass when glassy => UIButtonConfiguration.ClearGlassButtonConfiguration,
			_ => UIButtonConfiguration.PlainButtonConfiguration
		};

		if (kind is ButtonStyle.FilledCapsule)
			configuration.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;

		bool filled = kind is ButtonStyle.Filled or ButtonStyle.FilledCapsule or ButtonStyle.ProminentGlass;

		if (destructive)
		{
			configuration.BaseForegroundColor = UIColor.SystemRed;

			if (filled)
			{
				configuration.BaseBackgroundColor = UIColor.SystemRed;
				configuration.BaseForegroundColor = UIColor.White;
			}
		}
		else if (tint is Color color)
		{
			UIColor native = color.ToUIColor();

			if (filled || kind is ButtonStyle.Tinted)
				configuration.BaseBackgroundColor = native;

			if (!filled)
				configuration.BaseForegroundColor = native;
		}

		return configuration;
	}
}
