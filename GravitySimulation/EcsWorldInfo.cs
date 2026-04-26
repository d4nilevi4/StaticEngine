using FFS.Libraries.StaticEcs;

namespace GravitySimulation;

public struct WT : IWorldType {}
public abstract class W : World<WT> {}

public struct GravSimSystems : ISystemsType { }

public abstract class Sys : W.Systems<GravSimSystems>
{
}