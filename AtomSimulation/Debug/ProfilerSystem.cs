using FFS.Libraries.StaticEcs;
using Raylib_cs;

namespace AtomSimulation;

public struct ProfilerSystem : ISystem
{
    public void Init()
    {
        Profiler.Register("OrbitalRegen", new Color(255, 200, 100, 220));
        Profiler.Register("ProbFlow", new Color(255, 100, 100, 220));
        Profiler.Register("Render", new Color(100, 150, 255, 220));
    }

    public void Update()
    {
        Profiler.Draw(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        Profiler.EndFrame();
    }
}