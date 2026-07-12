namespace BareUI;

internal static partial class MainThread
{
	public static void Post(
		Action action)
	{
		bool posted = false;
		PostCore(action, ref posted);

		if (!posted)
			action();
	}

	static partial void PostCore(
		Action action,
		ref bool posted);
}
