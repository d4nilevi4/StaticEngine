using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;
using Raylib_cs;

namespace AtomSimulation;

public struct ControlCameraSystem : ISystem
{
    public void Init()
    {
        ApplyOrbit();
    }

    public void Update()
    {
        ref var ctrl = ref W.GetResource<CameraControl>();

        if (Raylib.IsMouseButtonDown(MouseButton.Left) || Raylib.IsMouseButtonDown(MouseButton.Middle))
        {
            var d = Raylib.GetMouseDelta();
            ctrl.Azimuth += d.X * ctrl.OrbitSpeed;
            ctrl.Elevation -= d.Y * ctrl.OrbitSpeed;
        }

        ctrl.Elevation = Math.Clamp(ctrl.Elevation, 0.01f, MathF.PI - 0.01f);

        float wheel = Raylib.GetMouseWheelMove();
        if (wheel != 0f)
        {
            ctrl.Radius -= wheel * ctrl.ZoomSpeed;
            ctrl.Radius = Math.Clamp(ctrl.Radius, ctrl.MinRadius, ctrl.MaxRadius);
        }

        ApplyOrbit();
    }

    static void ApplyOrbit()
    {
        if (!W.Query<All<CameraPerspectiveProjection, Transform, IsMainCamera>>().Any(out var cam))
            return;

        ref readonly var ctrl = ref W.GetResource<CameraControl>();
        ref var t = ref cam.Ref<Transform>();

        float sinE = MathF.Sin(ctrl.Elevation);
        float cosE = MathF.Cos(ctrl.Elevation);
        float sinA = MathF.Sin(ctrl.Azimuth);
        float cosA = MathF.Cos(ctrl.Azimuth);

        float3 pos = ctrl.Target + new float3(
            ctrl.Radius * sinE * cosA,
            ctrl.Radius * cosE,
            ctrl.Radius * sinE * sinA
        );

        t.Position = pos;
        t.Rotation = quaternion.LookAt(pos, ctrl.Target);
    }
}