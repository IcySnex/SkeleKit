namespace SkeleKit;

internal static partial class MainThread
{
	static partial void PostCore(
		Action action,
		ref bool posted);

	
	public static void Post(
		Action action)
	{
		bool posted = false;
		PostCore(action, ref posted);

		if (!posted)
			action();
	}
}
