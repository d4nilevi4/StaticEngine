using FFS.Libraries.StaticEcs;

namespace AtomSimulation;

// ECS world handle. W is the project-wide alias for queries and resources.
public struct WT : IWorldType {}
public abstract class W : World<WT> {}

// System schedule. Sys is the alias used to register and tick systems.
public struct AtomSimSystems : ISystemsType { }

public abstract class Sys : W.Systems<AtomSimSystems> { }
