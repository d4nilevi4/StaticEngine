using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;
using Raylib_cs;

namespace GravitySimulation;

public struct BillboardRenderSystem : ISystem
{
    private Shader _shader;

    public void Init()
    {
        string baseDir = AppContext.BaseDirectory;
        string vs = Path.Combine(baseDir, "Assets", "Shaders", "billboard.vs");
        string fs = Path.Combine(baseDir, "Assets", "Shaders", "billboard.fs");

        _shader = Raylib.LoadShader(vs, fs);
        if (_shader.Id == 0)
            throw new InvalidOperationException($"Failed to load billboard shader from {vs} / {fs}");
    }

    public void Update()
    {
        long t0 = Profiler.Timestamp();

        W.Query<All<CameraPerspectiveProjection, Transform, IsMainCamera>>().For(_shader, static (
            ref Shader shader,
            ref Transform ct,
            in CameraPerspectiveProjection cam
        ) =>
        {
            // Camera basis in world space
            float3 camPos = ct.Position;
            float3 camFwd = ct.Forward;
            float3 camUp = ct.Up;

            // Orthonormalize: right ⟂ (fwd, up), then re-derive up strictly ⟂ fwd
            float3 camRight = camFwd.Cross(camUp).Normalized;
            camUp = camRight.Cross(camFwd).Normalized;

            float farSq = cam.Far * cam.Far;
            float aspect = (float)Raylib.GetScreenWidth() / Raylib.GetScreenHeight();
            // Projection: frustum → clip space [-1,1]
            var projection = float4x4.Perspective(cam.Fov, aspect, cam.Near, cam.Far);
            // View: world → camera space (camera at origin, looking down -Z)
            var view = float4x4.LookAt(camPos, camPos + camFwd, camUp);

            // Flush pending batch so matrix swap doesn't affect prior vertices
            Rlgl.DrawRenderBatchActive();
            // Save current projection, install ours
            Rlgl.MatrixMode(MatrixMode.Projection);
            Rlgl.PushMatrix();
            Rlgl.LoadIdentity();
            Rlgl.MultMatrixf(projection.ToNumerics());
            // Install view as ModelView (model = identity, vertices fed in world space)
            Rlgl.MatrixMode(MatrixMode.ModelView);
            Rlgl.LoadIdentity();
            Rlgl.MultMatrixf(view.ToNumerics());
            // Enable z-buffer so far billboards don't overdraw near ones
            Rlgl.EnableDepthTest();

            // Bind our shader for upcoming draw calls
            Raylib.BeginShaderMode(shader);
            // Open immediate-mode vertex sink; every 4 verts = 1 quad
            Rlgl.Begin((int)DrawMode.Quads);

            W.Query<All<Transform, MeshColor>>().For((camRight, camUp, camPos, camFwd, farSq), static (
                ref (float3 right, float3 up, float3 pos, float3 fwd, float farSq) cam,
                in Transform tr,
                in MeshColor col) =>
            {
                float3 p = tr.Position;
                float s = tr.Scale.x;

                // Cull: behind camera (signed projection on fwd, with radius slack)
                float3 toObj = p - cam.pos;
                if (toObj.Dot(cam.fwd) < -s) return;
                // Cull: beyond far plane (squared distance — no sqrt)
                if (toObj.SqrMagnitude > cam.farSq) return;

                // Half-extents along camera's screen plane axes
                float3 r = cam.right * s;
                float3 u = cam.up * s;

                // Quad corners in world space, facing the camera
                float3 bl = p - r - u;
                float3 br = p + r - u;
                float3 trn = p + r + u;
                float3 tl = p - r + u;

                // Per-vertex color for following Vertex3f calls
                Rlgl.Color4ub(col.R, col.G, col.B, col.A);

                // Emit 4 verts CCW with UVs (0,0)→(1,0)→(1,1)→(0,1)
                Rlgl.TexCoord2f(0f, 0f);
                Rlgl.Vertex3f(bl.x, bl.y, bl.z);
                Rlgl.TexCoord2f(1f, 0f);
                Rlgl.Vertex3f(br.x, br.y, br.z);
                Rlgl.TexCoord2f(1f, 1f);
                Rlgl.Vertex3f(trn.x, trn.y, trn.z);
                Rlgl.TexCoord2f(0f, 1f);
                Rlgl.Vertex3f(tl.x, tl.y, tl.z);
            });

            // Close vertex sink
            Rlgl.End();
            // Submit the whole batch — single draw call for all billboards
            Rlgl.DrawRenderBatchActive();
            // Unbind our shader
            Raylib.EndShaderMode();

            // Restore previous projection from stack
            Rlgl.MatrixMode(MatrixMode.Projection);
            Rlgl.PopMatrix();
            // Reset ModelView so we don't leak our view to later systems
            Rlgl.MatrixMode(MatrixMode.ModelView);
            Rlgl.LoadIdentity();
            Rlgl.DisableDepthTest();
        });

        Profiler.Record("Render", Profiler.ElapsedMs(t0));
    }

    public void Destroy()
    {
        if (_shader.Id != 0)
            Raylib.UnloadShader(_shader);
    }
}