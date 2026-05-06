using FFS.Libraries.StaticEcs;
using Raylib_cs;

namespace AtomSimulation;

// Entry point. Boots Raylib, registers ECS types and systems, runs the loop.
static class Program
{
    static void Main()
    {
        Raylib.InitWindow(1600, 900, "Atom Simulation");
        Raylib.SetTargetFPS(6000);
        Raylib.SetExitKey(0);

        W.Create(WorldConfig.MaxThreads());
        W.Types().RegisterAll();
        W.Initialize();

        W.SetResource(new Time { TimeScale = 100f });
        W.SetResource(CameraControl.Default);
        W.SetResource(OrbitalState.Default);

        // System order groups stages: input/regen (0..9), sim (10..),
        // rendering (1000..), profiler last so it captures everything.
        Sys.Create();
        Sys.Add(new SpawnSystem(), order: 0);
        Sys.Add(new OrbitalInputSystem(), order: 1);
        Sys.Add(new RegenerateOrbitalSystem(), order: 5);
        Sys.Add(new ProbabilityFlowSystem(), order: 10);

        Sys.Add(new ControlCameraSystem(), order: 1000);
        Sys.Add(new BillboardRenderSystem(), order: 1010);
        Sys.Add(new DrawHudSystem(), order: 1020);

        Sys.Add(new ProfilerSystem(), order: short.MaxValue);
        Sys.Initialize();

        ref var time = ref W.GetResource<Time>();

        while (!Raylib.WindowShouldClose())
        {
            if (Raylib.IsKeyPressed(KeyboardKey.K)) time.Paused = !time.Paused;
            if (Raylib.IsKeyPressed(KeyboardKey.Q)) break;

            float dt = Raylib.GetFrameTime();
            time.DeltaTime = time.Paused ? 0f : dt;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Sys.Update();

            Raylib.EndDrawing();
        }

        Sys.Destroy();
        W.Destroy();
        Raylib.CloseWindow();
    }
}