using FFS.Libraries.StaticEcs;
using Raylib_cs;

namespace AtomSimulation;

public struct OrbitalInputSystem : ISystem
{
    public void Update()
    {
        ref var state = ref W.GetResource<OrbitalState>();

        bool dirty = false;

        if (Raylib.IsKeyPressed(KeyboardKey.Up))    { state.N += 1; dirty = true; }
        if (Raylib.IsKeyPressed(KeyboardKey.Down))  { state.N -= 1; dirty = true; }
        if (Raylib.IsKeyPressed(KeyboardKey.Right)) { state.L += 1; dirty = true; }
        if (Raylib.IsKeyPressed(KeyboardKey.Left))  { state.L -= 1; dirty = true; }
        if (Raylib.IsKeyPressed(KeyboardKey.RightBracket)) { state.M += 1; dirty = true; }
        if (Raylib.IsKeyPressed(KeyboardKey.LeftBracket))  { state.M -= 1; dirty = true; }
        if (Raylib.IsKeyPressed(KeyboardKey.Equal)) { state.ParticleCount += 10_000; dirty = true; }
        if (Raylib.IsKeyPressed(KeyboardKey.Minus)) { state.ParticleCount -= 10_000; dirty = true; }

        if (!dirty) return;

        if (state.N < 1) state.N = 1;
        if (state.L < 0) state.L = 0;
        if (state.L > state.N - 1) state.L = state.N - 1;
        if (state.M >  state.L) state.M =  state.L;
        if (state.M < -state.L) state.M = -state.L;
        if (state.ParticleCount < 0) state.ParticleCount = 0;

        if (W.Query<All<OrbitalDirty>>().EntitiesCount() == 0)
            W.NewEntity<Default>().Set<OrbitalDirty>();
    }
}
