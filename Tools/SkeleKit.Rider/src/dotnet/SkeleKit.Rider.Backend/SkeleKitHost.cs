using JetBrains.Application.Parts;
using JetBrains.Core;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.Rd.Tasks;
using JetBrains.ReSharper.Feature.Services.Protocol;
using JetBrains.Rider.Model;
using SkeleKit.Rider.Backend.HotReload;

namespace SkeleKit.Rider.Backend;

// Backend entry point wired to the frontend over rd. Owns the in-process sdb proxy (Bridge) and its
// Roslyn EmitDifference pipeline, which run against the host's bundled Roslyn.
[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public sealed class SkeleKitHost
{
	static readonly ILog OurLog = Log.GetLog<SkeleKitHost>();

	LifetimeDefinition? bridgeLifetime;

	public SkeleKitHost(
		Lifetime lifetime,
		ISolution solution)
	{
		SkeleKitModel model = solution.GetProtocolSolution().GetSkeleKitModel();

		model.StartBridge.Set((_, request) =>
		{
			OurLog.Info($"startBridge: {request.AssemblyName} @ {request.ProjectDir}");

			bridgeLifetime?.Terminate();
			LifetimeDefinition definition = lifetime.CreateNested();
			bridgeLifetime = definition;

			Bridge bridge = new(request.CscArgs, request.DeployedDll, request.ProjectDir, line => OurLog.Info($"[bridge] {line}"));
			(int idePort, int appPort) = bridge.Start(definition.Lifetime);

			model.Log.Fire($"bridge up: ide={idePort} app={appPort}");
			return RdTask<BridgeInfo>.Successful(new BridgeInfo(idePort, appPort));
		});

		model.StopBridge.Set((_, _) =>
		{
			bridgeLifetime?.Terminate();
			bridgeLifetime = null;

			return RdTask<Unit>.Successful(Unit.Instance);
		});

		RunEngineSelfTest();
	}

	// Temporary: exercise the ported Roslyn engine on the real Gallery, headless, so it shows in the
	// backend log without the frontend executor. Remove once Run/Debug executors land.
	static void RunEngineSelfTest()
	{
		const string gallery = "/Users/kevin/Repos/SkeleKit/Samples/SkeleKit.Gallery";
		string cscArgs = Path.Combine(gallery, "obj/Debug/net10.0-ios/iossimulator-arm64/skelekit-hotreload.args");
		string dll = Path.Combine(gallery, "bin/Debug/net10.0-ios/iossimulator-arm64/SkeleKit.Gallery.dll");
		if (!File.Exists(cscArgs) || !File.Exists(dll))
			return;

		Bridge bridge = new(cscArgs, dll, gallery, line => OurLog.Info($"[selftest] {line}"));
		Thread thread = new(() =>
		{
			try
			{
				bridge.SelfTest();
			}
			catch (Exception exception)
			{
				OurLog.Error(exception, "engine self-test failed");
			}
		})
		{
			IsBackground = true,
			Name = "skele-selftest"
		};
		thread.Start();
	}
}
