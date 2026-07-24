using JetBrains.Application.Parts;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Protocol;
using JetBrains.Rider.Model;
using SkeleKit.Rider.Backend.HotReload;

namespace SkeleKit.Rider.Backend;

// Brings the hot-reload bridge up with the solution and tells the frontend which ports it landed on.
// The frontend needs those to route an iOS debug session through the bridge; leaving them unpublished
// is how a solution with nothing to hot reload keeps its iOS runs untouched.
[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public sealed class SkeleKitHost
{
	static readonly ILog OurLog = Log.GetLog<SkeleKitHost>();

	public SkeleKitHost(
		Lifetime lifetime,
		ISolution solution,
		IShellLocks locks)
	{
		NativeBridge bridge = new(SolutionFile(solution), line => OurLog.Info($"[native] {line}"));

		// discovery walks the solution's build outputs, which is too much to do on the way in
		Thread start = new(() =>
		{
			bool ready;
			try
			{
				ready = bridge.Start(lifetime);
			}
			catch (Exception exception)
			{
				OurLog.Error(exception, "the hot-reload bridge failed to start");
				return;
			}

			if (!ready)
				return;

			locks.ExecuteOrQueueEx(lifetime, "SkeleKit bridge ports", () =>
				solution.GetProtocolSolution().GetSkeleKitModel().BridgePorts.Value = new BridgePorts(bridge.AppPort, bridge.RiderPort));
		})
		{
			IsBackground = true,
			Name = "skele-bridge-start"
		};
		start.Start();
	}

	static string SolutionFile(
		ISolution solution)
	{
		string path = solution.SolutionFilePath.FullPath;

		return string.IsNullOrEmpty(path) ? solution.SolutionDirectory.FullPath : path;
	}
}
