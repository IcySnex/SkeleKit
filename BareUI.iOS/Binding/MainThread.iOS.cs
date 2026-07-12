using CoreFoundation;
using Foundation;

namespace BareUI;

internal static partial class MainThread
{
	static partial void PostCore(
		Action action,
		ref bool posted)
	{
		if (NSThread.IsMain)
			return;

		DispatchQueue.MainQueue.DispatchAsync(action);
		posted = true;
	}
}
