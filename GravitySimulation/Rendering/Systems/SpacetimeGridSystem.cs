using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;
using Raylib_cs;

namespace GravitySimulation;

// Renders the gravity-well grid: a flat XZ line lattice deformed in the
// vertex shader by the gravitational potential of all live bodies.
public struct SpacetimeGridSystem : ISystem
{
    const int GridDivisions = 600;
    const float GridSize = 85000f;

    const float MassUnit = 1e20f;
    const float WellScale = 5e8f;

    const float Softening = 1100f;

    // Vertical offset of the grid plane below the bodies' centre of mass
    const float WellPlaneOffset = 1200f;

    const int MaxBodies = 10000;
    const int BodyTextureSlot = 0;

    private SpacetimeShader _shader;
    private SpacetimeGridMesh _mesh;
    private SpacetimeBodyTexture _bodyTex;

    public void Init()
    {
        _shader.Load();
        _mesh.Build(GridDivisions, GridSize);
        _bodyTex.Create(MaxBodies);
    }

    public void Update()
    {
        long t0 = Profiler.Timestamp();

        if (!W.Query<All<CameraPerspectiveProjection, Transform, IsMainCamera>>().Any(out var camEntity))
        {
            Profiler.Record("SpacetimeGrid", Profiler.ElapsedMs(t0));
            return;
        }

        int n = Math.Min(GravityBodyBuffer.Count, MaxBodies);
        if (n == 0)
        {
            Profiler.Record("SpacetimeGrid", Profiler.ElapsedMs(t0));
            return;
        }

        ref readonly var cam = ref camEntity.Read<CameraPerspectiveProjection>();
        ref readonly var ct = ref camEntity.Read<Transform>();

        // Pack body data into the GPU texture and place the grid plane below the COM
        float comY = _bodyTex.PackAndUpload(n, MassUnit, WellScale);
        float baseY = comY - WellPlaneOffset;

        // mvp = projection * view (column-vector GLSL convention)
        float aspect = (float)Raylib.GetScreenWidth() / Raylib.GetScreenHeight();
        var projection = float4x4.Perspective(cam.Fov, aspect, cam.Near, cam.Far).ToNumerics();
        var view = float4x4.LookAt(ct.Position, ct.Position + ct.Forward, ct.Up).ToNumerics();
        var mvp = projection * view;

        DrawGrid(mvp, n, baseY);

        // The grid is the last rendered pass — close the frame here so the profiler
        // overlay (next system) draws into a fresh BeginDrawing in the next iteration
        Raylib.EndDrawing();

        Profiler.Record("SpacetimeGrid", Profiler.ElapsedMs(t0));
    }

    private void DrawGrid(System.Numerics.Matrix4x4 mvp, int bodyCount, float baseY)
    {
        // Flush raylib's pending immediate batch so our manual draw doesn't share state
        Rlgl.DrawRenderBatchActive();

        Rlgl.EnableShader(_shader.Shader.Id);
        SetUniforms(mvp, bodyCount, baseY);

        _bodyTex.Bind(BodyTextureSlot);

        Rlgl.EnableDepthTest();
        Rlgl.EnableColorBlend();
        Rlgl.SetBlendMode(BlendMode.Alpha);

        _mesh.Draw();

        SpacetimeBodyTexture.Unbind(BodyTextureSlot);
        Rlgl.DisableShader();
        Rlgl.DisableDepthTest();
    }

    private void SetUniforms(System.Numerics.Matrix4x4 mvp, int bodyCount, float baseY)
    {
        Rlgl.SetUniformMatrix(_shader.LocMvp, mvp);

        unsafe
        {
            int n = bodyCount;
            float by = baseY;
            float softSq = Softening * Softening;
            int samplerSlot = BodyTextureSlot;

            Rlgl.SetUniform(_shader.LocBodyCount, &n, (int)ShaderUniformDataType.Int, 1);
            Rlgl.SetUniform(_shader.LocBaseY, &by, (int)ShaderUniformDataType.Float, 1);
            Rlgl.SetUniform(_shader.LocSoftSq, &softSq, (int)ShaderUniformDataType.Float, 1);
            Rlgl.SetUniform(_shader.LocBodies, &samplerSlot, (int)ShaderUniformDataType.Int, 1);
        }
    }

    public void Destroy()
    {
        _mesh.Unload();
        _bodyTex.Unload();
        _shader.Unload();
    }
}