using Raylib_cs;

namespace GravitySimulation;

// Spacetime grid shader handle plus cached uniform locations.
internal struct SpacetimeShader
{
    public Shader Shader;
    public int LocMvp;
    public int LocBodies;
    public int LocBodyCount;
    public int LocBaseY;
    public int LocSoftSq;

    public void Load()
    {
        var baseDir = AppContext.BaseDirectory;
        var vs = Path.Combine(baseDir, "Assets", "Shaders", "spacetime.vs");
        var fs = Path.Combine(baseDir, "Assets", "Shaders", "spacetime.fs");

        Shader = Raylib.LoadShader(vs, fs);
        if (Shader.Id == 0)
            throw new InvalidOperationException($"Failed to load spacetime shader from {vs} / {fs}");

        LocMvp = Raylib.GetShaderLocation(Shader, "mvp");
        LocBodies = Raylib.GetShaderLocation(Shader, "uBodies");
        LocBodyCount = Raylib.GetShaderLocation(Shader, "uBodyCount");
        LocBaseY = Raylib.GetShaderLocation(Shader, "uBaseY");
        LocSoftSq = Raylib.GetShaderLocation(Shader, "uSoftSq");
    }

    public void Unload()
    {
        if (Shader.Id != 0) Raylib.UnloadShader(Shader);
    }
}