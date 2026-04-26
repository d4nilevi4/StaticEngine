using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;

namespace GravitySimulation;

public struct Velocity : IComponent { public float3 Value; }
public struct Mass : IComponent { public float Value; }
public struct GravityBody : ITag { }