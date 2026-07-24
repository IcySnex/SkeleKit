using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Rider.Model;

namespace SkeleKit.Rider.Backend;

// Components here ride Rider's (active) model zone, so no custom zone/activator is needed.
[ZoneMarker]
public class ZoneMarker : IRequire<IRiderModelZone>;
