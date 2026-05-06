using FFS.Libraries.StaticEcs;

namespace AtomSimulation;

public struct SpawnSystem : ISystem
{
    public void Init()
    {
        SpawnCamera();
        W.NewEntity<Default>().Set<OrbitalDirty>();
    }

    static void SpawnCamera()
    {
        W.NewEntity<Default>().Set(
            new Transform(),
            CameraPerspectiveProjection.Default
        ).Set<IsMainCamera>();
    }
}