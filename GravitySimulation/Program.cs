using FFS.Libraries.StaticEcs;
using Raylib_cs;

namespace GravitySimulation;

static class Program
{
    static void Main()
    {
        Raylib.InitWindow(1600, 900, "Gravity Simulation");
        Raylib.SetTargetFPS(6000);
        Raylib.SetExitKey(0);
        Raylib.DisableCursor();

        W.Create(WorldConfig.MaxThreads());
        W.Types().RegisterAll();
        W.Initialize();

        W.SetResource(new Time { TimeScale = 1f });
        W.SetResource(CameraControl.Default);

        Sys.Create();
        Sys.Add(new SpawnSystem(), order: 0);
        Sys.Add(new CollectGravityBodiesSystem(), order: 8);
        Sys.Add(new ApplyGravityForceSystem(), order: 9);
        Sys.Add(new MoveBodiesSystem(), order: 10);

        Sys.Add(new ControlCameraSystem(), order: 1000);
        Sys.Add(new BillboardRenderSystem(), order: 1010);
        Sys.Add(new SpacetimeGridSystem(), order: 1015);
        Sys.Add(new DrawHudSystem(), order: 1020);

        Sys.Add(new ProfilerSystem(), order: short.MaxValue);
        Sys.Initialize();

        ref var time = ref W.GetResource<Time>();

        while (!Raylib.WindowShouldClose())
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                if (Raylib.IsCursorHidden()) Raylib.EnableCursor();
                else Raylib.DisableCursor();
            }
            
            if (Raylib.IsKeyPressed(KeyboardKey.K)) time.Paused = !time.Paused;
            if (Raylib.IsKeyPressed(KeyboardKey.Q)) break;

            float dt = Raylib.GetFrameTime();

            time.DeltaTime = time.Paused ? 0f : dt;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Sys.Update();
            
            // Uncomment when SpacetimeGridSystem is commented out
            // Raylib.EndDrawing();
        }

        Sys.Destroy();
        W.Destroy();
        Raylib.CloseWindow();
    }
}