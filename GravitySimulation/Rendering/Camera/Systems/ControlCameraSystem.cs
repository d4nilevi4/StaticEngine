using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;
using Raylib_cs;

namespace GravitySimulation;

public struct ControlCameraSystem : ISystem
{
    public void Init()
    {
        if (!W.Query<All<CameraPerspectiveProjection, Transform, IsMainCamera>>().Any(out var cam))
            return;

        ref var ctrl = ref W.GetResource<CameraControl>();
        ref readonly var t = ref cam.Read<Transform>();

        float3 fwd = t.Rotation.RotatePoint(float3.Forward);
        ctrl.Pitch = MathF.Asin(Math.Clamp(fwd.y, -1f, 1f)) * 180f / MathF.PI;
        ctrl.Yaw = MathF.Atan2(fwd.z, fwd.x) * 180f / MathF.PI;
    }

    public void Update()
    {
        if (!Raylib.IsCursorHidden())
            return;

        if (!W.Query<All<CameraPerspectiveProjection, Transform, IsMainCamera>>().Any(out var cam))
            return;

        ref var ctrl = ref W.GetResource<CameraControl>();
        ref var t = ref cam.Ref<Transform>();
        var dt = Raylib.GetFrameTime();

        var delta = Raylib.GetMouseDelta();
        ctrl.Yaw += delta.X * ctrl.MouseSensitivity;
        ctrl.Pitch -= delta.Y * ctrl.MouseSensitivity;
        ctrl.Pitch = Math.Clamp(ctrl.Pitch, -ctrl.PitchLimit, ctrl.PitchLimit);

        float yawRad = ctrl.Yaw * MathF.PI / 180f;
        float pitchRad = ctrl.Pitch * MathF.PI / 180f;
        float3 fwd = new float3(
            MathF.Cos(yawRad) * MathF.Cos(pitchRad),
            MathF.Sin(pitchRad),
            MathF.Sin(yawRad) * MathF.Cos(pitchRad)
        );
        float3 right = fwd.Cross(float3.Up).Normalized;
        float3 up = right.Cross(fwd);

        ctrl.Speed *= 1f + Raylib.GetMouseWheelMove() * ctrl.ZoomStep;
        ctrl.Speed = Math.Clamp(ctrl.Speed, ctrl.MinSpeed, ctrl.MaxSpeed);

        float move = ctrl.Speed * dt;
        if (Raylib.IsKeyDown(KeyboardKey.W)) t.Position += fwd * move;
        if (Raylib.IsKeyDown(KeyboardKey.S)) t.Position -= fwd * move;
        if (Raylib.IsKeyDown(KeyboardKey.D)) t.Position += right * move;
        if (Raylib.IsKeyDown(KeyboardKey.A)) t.Position -= right * move;
        if (Raylib.IsKeyDown(KeyboardKey.Space)) t.Position += up * move;
        if (Raylib.IsKeyDown(KeyboardKey.LeftShift)) t.Position -= up * move;

        t.Rotation = quaternion.LookAt(t.Position, t.Position + fwd);
    }
}