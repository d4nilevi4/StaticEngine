using FFS.Libraries.StaticEcs;
using Raylib_cs;

namespace GravitySimulation;

public struct DrawHudSystem : ISystem
{
    public void Update()
    {
        int sw = Raylib.GetScreenWidth();
        int sh = Raylib.GetScreenHeight();

        Raylib.DrawFPS(sw - 90, 10);
        Raylib.DrawText($"Bodies: {CountBodies()}", 10, 10, 20, Color.White);

        Raylib.DrawText(
            "WASD - move | Space/LeftShift - up/down move |  Mouse - look  |  Scroll - speed  |  K - pause  |  Esc - cursor  |  Q - quit",
            10, sh - 22, 14, new Color(160, 160, 160, 180));
    }

    private static int CountBodies() =>
        W.Query<All<GravityBody>>().EntitiesCount();
}