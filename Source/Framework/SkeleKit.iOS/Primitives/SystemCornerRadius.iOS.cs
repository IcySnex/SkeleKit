namespace SkeleKit;

public static partial class SystemCornerRadius
{
	static partial void ResolveGroupedList(
		ref double radius)
	{
		using UITraitCollection traits = UITraitCollection.GetTraitCollection(UIListEnvironment.InsetGrouped);
		using UIViewConfigurationState state = new(traits);
		using UIBackgroundConfiguration resolved = UIBackgroundConfiguration.ListCellConfiguration.GetUpdatedConfiguration(state);

		radius = resolved.CornerRadius;
	}
}
