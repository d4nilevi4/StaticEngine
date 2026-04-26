using FFS.Libraries.StaticEcs;

namespace GravitySimulation;

public struct CollectGravityBodiesSystem : ISystem
{
    public void Update()
    {
        long t0 = Profiler.Timestamp();

        ref var time = ref W.GetResource<Time>();
        GravityBodyBuffer.Dt = time.DeltaTime * time.TimeScale;

        int n = 0;
        foreach (var e in W.Query<All<GravityBody, Transform, Mass>>().Entities())
        {
            GravityBodyBuffer.EnsureCapacity(n + 1);

            var p = e.Read<Transform>().Position;
            GravityBodyBuffer.Xs[n] = p.x;
            GravityBodyBuffer.Ys[n] = p.y;
            GravityBodyBuffer.Zs[n] = p.z;
            GravityBodyBuffer.Masses[n] = e.Read<Mass>().Value;
            n++;
        }

        GravityBodyBuffer.Count = n;

        Profiler.Record("GravityCollect", Profiler.ElapsedMs(t0));
    }

    public bool UpdateIsActive()
    {
        return !W.GetResource<Time>().Paused;
    }
}