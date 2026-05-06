using FFS.Libraries.StaticEcs;
using Raylib_cs;

namespace AtomSimulation;

public struct DrawHudSystem : ISystem
{
    public void Update()
    {
        int sw = Raylib.GetScreenWidth();
        int sh = Raylib.GetScreenHeight();

        Raylib.DrawFPS(sw - 90, 10);

        ref readonly var state = ref W.GetResource<OrbitalState>();
        Raylib.DrawText(
            $"n = {state.N}   l = {state.L}   m = {state.M}   particles = {state.ParticleCount}",
            10, 10, 22, Color.White);

        Raylib.DrawText(
            "Up/Down - n  |  Right/Left - l  |  ]/[ - m  |  =/- - count  |  LMB drag - orbit  |  Scroll - zoom  |  K - pause  |  Q - quit",
            10, sh - 22, 14, new Color(160, 160, 160, 180));
    }
}