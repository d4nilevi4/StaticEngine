using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;

namespace AtomSimulation;

// Draws every particle as a camera-facing quad packed into one dynamic mesh.
// CPU billboarding + a single draw call keeps the GPU happy at 100k+ particles.
public unsafe struct BillboardRenderSystem : ISystem
{
    private Shader _shader;
    private Material _material;
    private Mesh _mesh;
    private int _capacity;

    public void Init()
    {
        string baseDir = AppContext.BaseDirectory;
        string vs = Path.Combine(baseDir, "Assets", "Shaders", "billboard.vs");
        string fs = Path.Combine(baseDir, "Assets", "Shaders", "billboard.fs");

        _shader = Raylib.LoadShader(vs, fs);
        if (_shader.Id == 0)
            throw new InvalidOperationException($"Failed to load billboard shader from {vs} / {fs}");

        _material = Raylib.LoadMaterialDefault();
        _material.Shader = _shader;
        _capacity = 0;
    }

    public void Update()
    {
        long t0 = Profiler.Timestamp();

        if (!W.Query<All<CameraPerspectiveProjection, Transform, IsMainCamera>>().Any(out var cam))
        {
            Profiler.Record("Render", Profiler.ElapsedMs(t0));
            return;
        }

        ref readonly var ct = ref cam.Read<Transform>();
        ref readonly var cp = ref cam.Read<CameraPerspectiveProjection>();

        // Re-orthonormalize so the basis stays right-handed even if camera Up isn't perpendicular to Forward.
        float3 camPos = ct.Position;
        float3 camFwd = ct.Forward;
        float3 camUp = ct.Up;
        float3 camRight = camFwd.Cross(camUp).Normalized;
        camUp = camRight.Cross(camFwd).Normalized;
        // Squared distance avoids a sqrt per particle in the far-plane cull.
        float farSq = cp.Far * cp.Far;

        int particleCount = W.Query<All<Transform, ParticleColor>>().EntitiesCount();
        if (particleCount == 0)
        {
            Profiler.Record("Render", Profiler.ElapsedMs(t0));
            return;
        }

        EnsureCapacity(particleCount);

        // Direct pointers into native mesh buffers; idx counts quads after culling.
        float* verts = _mesh.Vertices;
        byte* colors = _mesh.Colors;
        int idx = 0;

        foreach (var e in W.Query<All<Transform, ParticleColor>>().Entities())
        {
            ref readonly var tr = ref e.Read<Transform>();
            ref readonly var col = ref e.Read<ParticleColor>();

            float3 p = tr.Position;
            float scl = tr.Scale.x;
            float3 toObj = p - camPos;
            // Behind-camera cull with a -scl slack for quads straddling the plane.
            if (toObj.Dot(camFwd) < -scl) continue;
            if (toObj.SqrMagnitude > farSq) continue;

            // Quad corners along camera right/up axes.
            float3 r = camRight * scl;
            float3 u = camUp * scl;
            float3 bl = p - r - u;
            float3 br = p + r - u;
            float3 trn = p + r + u;
            float3 tl = p - r + u;

            // 6 verts * 3 floats per quad. Winding (bl, br, tr) and (bl, tr, tl) is CCW from the camera.
            float* v = verts + idx * 18;
            v[0] = bl.x;
            v[1] = bl.y;
            v[2] = bl.z;
            v[3] = br.x;
            v[4] = br.y;
            v[5] = br.z;
            v[6] = trn.x;
            v[7] = trn.y;
            v[8] = trn.z;
            v[9] = bl.x;
            v[10] = bl.y;
            v[11] = bl.z;
            v[12] = trn.x;
            v[13] = trn.y;
            v[14] = trn.z;
            v[15] = tl.x;
            v[16] = tl.y;
            v[17] = tl.z;

            // Pack RGBA into one uint and write 6 uint stores instead of 24 byte stores.
            // Little-endian: LSB maps to R, matching Raylib's R,G,B,A byte layout.
            uint packed = col.R | ((uint)col.G << 8) | ((uint)col.B << 16) | ((uint)col.A << 24);
            uint* cu = (uint*)(colors + idx * 24);
            cu[0] = packed;
            cu[1] = packed;
            cu[2] = packed;
            cu[3] = packed;
            cu[4] = packed;
            cu[5] = packed;

            idx++;
        }

        if (idx == 0)
        {
            Profiler.Record("Render", Profiler.ElapsedMs(t0));
            return;
        }

        // Buffer slots 0 and 3 are vertices and colors in Raylib's mesh layout.
        Raylib.UpdateMeshBuffer(_mesh, 0, verts, idx * 18 * sizeof(float), 0);
        Raylib.UpdateMeshBuffer(_mesh, 3, colors, idx * 24, 0);

        _mesh.VertexCount = idx * 6;
        _mesh.TriangleCount = idx * 2;

        // Camera lives as ECS components, so push view/projection straight into rlgl.
        float aspect = (float)Raylib.GetScreenWidth() / Raylib.GetScreenHeight();
        var projection = float4x4.Perspective(cp.Fov, aspect, cp.Near, cp.Far);
        var view = float4x4.LookAt(camPos, camPos + camFwd, camUp);

        // Flush queued 2D, swap matrices, draw, then pop so HUD draws afterward.
        Rlgl.DrawRenderBatchActive();
        Rlgl.MatrixMode(MatrixMode.Projection);
        Rlgl.PushMatrix();
        Rlgl.LoadIdentity();
        Rlgl.MultMatrixf(projection.ToNumerics());
        Rlgl.MatrixMode(MatrixMode.ModelView);
        Rlgl.LoadIdentity();
        Rlgl.MultMatrixf(view.ToNumerics());

        // Additive blending so overlapping particles read as a density volume.
        Raylib.BeginBlendMode(BlendMode.Additive);
        Raylib.DrawMesh(_mesh, _material, Matrix4x4.Identity);
        Raylib.EndBlendMode();

        Rlgl.MatrixMode(MatrixMode.Projection);
        Rlgl.PopMatrix();
        Rlgl.MatrixMode(MatrixMode.ModelView);
        Rlgl.LoadIdentity();

        Profiler.Record("Render", Profiler.ElapsedMs(t0));
    }

    // Doubles capacity on growth so a stream of small increases doesn't reupload every frame.
    private void EnsureCapacity(int particles)
    {
        if (particles <= _capacity) return;

        int newCap = _capacity == 0
            ? Math.Max(particles, 16384)
            : Math.Max(particles, _capacity * 2);

        if (_capacity > 0)
            Raylib.UnloadMesh(_mesh);

        // Native allocations: Raylib's Mesh holds raw pointers and uploads via OpenGL.
        int verts = newCap * 6;
        _mesh = new Mesh
        {
            VertexCount = verts,
            TriangleCount = newCap * 2,
            Vertices = (float*)NativeMemory.AllocZeroed((nuint)(verts * 3 * sizeof(float))),
            TexCoords = (float*)NativeMemory.AllocZeroed((nuint)(verts * 2 * sizeof(float))),
            Colors = (byte*)NativeMemory.AllocZeroed((nuint)(verts * 4))
        };

        // Tex coords are the same per quad - write them once at allocation.
        for (int i = 0; i < newCap; i++)
        {
            int o = i * 12;
            _mesh.TexCoords[o + 0] = 0f;
            _mesh.TexCoords[o + 1] = 0f;
            _mesh.TexCoords[o + 2] = 1f;
            _mesh.TexCoords[o + 3] = 0f;
            _mesh.TexCoords[o + 4] = 1f;
            _mesh.TexCoords[o + 5] = 1f;
            _mesh.TexCoords[o + 6] = 0f;
            _mesh.TexCoords[o + 7] = 0f;
            _mesh.TexCoords[o + 8] = 1f;
            _mesh.TexCoords[o + 9] = 1f;
            _mesh.TexCoords[o + 10] = 0f;
            _mesh.TexCoords[o + 11] = 1f;
        }

        // Dynamic upload so future UpdateMeshBuffer calls don't reallocate the GPU side.
        Raylib.UploadMesh(ref _mesh, true);
        _capacity = newCap;
    }

    public void Destroy()
    {
        if (_capacity > 0)
        {
            Raylib.UnloadMesh(_mesh);
            _capacity = 0;
        }

        if (_shader.Id != 0)
            Raylib.UnloadMaterial(_material);
    }
}