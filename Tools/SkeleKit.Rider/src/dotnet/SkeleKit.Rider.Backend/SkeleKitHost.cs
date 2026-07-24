using JetBrains.Application.Parts;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using SkeleKit.Rider.Backend.HotReload;

namespace SkeleKit.Rider.Backend;

// Starts the native-session hot-reload bridge when a solution loads. The frontend advice reroutes the
// iOS debug ports to the bridge, which relays the session to Rider (breakpoints intact) and injects
// EnC deltas on save.
[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public sealed class SkeleKitHost
{
	static readonly ILog OurLog = Log.GetLog<SkeleKitHost>();

	public SkeleKitHost(
		Lifetime lifetime,
		ISolution solution)
	{
		// TODO: derive these from the project model (the iOS runnable project's build output).
		const string gallery = "/Users/kevin/Repos/SkeleKit/Samples/SkeleKit.Gallery";
		string cscArgs = Path.Combine(gallery, "obj/Debug/net10.0-ios/iossimulator-arm64/skelekit-hotreload.args");
		string dll = Path.Combine(gallery, "bin/Debug/net10.0-ios/iossimulator-arm64/SkeleKit.Gallery.dll");

		NativeBridge bridge = new(cscArgs, dll, gallery, line => OurLog.Info($"[native] {line}"));
		try
		{
			bridge.Start(lifetime);
		}
		catch (Exception exception)
		{
			OurLog.Error(exception, "native bridge failed to start");
		}
	}
}
