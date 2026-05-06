using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;

namespace AtomSimulation;

// Per-particle linear velocity.
public struct Velocity : IComponent { public float3 Value; }

// Marks an entity as a sampled point of the orbital cloud.
public struct OrbitalParticle : ITag { }

// Marker requesting the cloud to be rebuilt.
public struct OrbitalDirty : ITag { }
