using FFS.Libraries.StaticEcs;
using Raylib_cs;

namespace GravitySimulation;

public struct ProfilerSystem : ISystem
{
    public void Init()
    {
        Profiler.Register("GravityCollect", new Color(255, 100, 100, 220));
        Profiler.Register("GravityForce", new Color(255, 100, 100, 220));
        Profiler.Register("MoveBodies", new Color(255, 100, 100, 220));
        Profiler.Register("Render", new Color(100, 150, 255, 220));
        Profiler.Register("SpacetimeGrid", new Color(180, 100, 255, 220));
    }

    public void Update()
    {
        Profiler.Draw(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        Profiler.EndFrame();
    }
}