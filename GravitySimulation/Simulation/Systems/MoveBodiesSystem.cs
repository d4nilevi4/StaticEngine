using FFS.Libraries.StaticEcs;

namespace GravitySimulation;

public struct MoveBodiesSystem : ISystem
{
    public void Update()
    {
        long t0 = Profiler.Timestamp();

        ref var time = ref W.GetResource<Time>();
        float dt = time.DeltaTime * time.TimeScale;

        W.Query().ForParallel(dt,
            static (ref float deltaT, ref Transform t, in Velocity v) => { t.Position += v.Value * deltaT; });

        Profiler.Record("MoveBodies", Profiler.ElapsedMs(t0));
    }

    public bool UpdateIsActive()
    {
        return !W.GetResource<Time>().Paused;
    }
}