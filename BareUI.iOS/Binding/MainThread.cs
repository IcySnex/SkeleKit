namespace BareUI;

/// <summary>
/// Runs work on the UI thread. The neutral test shim has no UI thread, so it runs inline.
/// </summary>
static partial class MainThread
{
	public static void Post(
		Action action)
	{
		bool posted = false;
		PostCore(action, ref posted);

		if (!posted)
			action();
	}

	// iOS half dispatches when off the main thread; unimplemented in the neutral shim
	static partial void PostCore(
		Action action,
		ref bool posted);
}
