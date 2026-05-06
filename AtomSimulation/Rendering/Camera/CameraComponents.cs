using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;

namespace AtomSimulation;

public struct IsMainCamera : ITag { }

public struct CameraPerspectiveProjection : IComponent
{
    public float Fov;
    public float Near;
    public float Far;

    public static CameraPerspectiveProjection Default => new()
    {
        Fov = 45f,
        Near = 0.1f,
        Far = 2000f
    };
}

public struct CameraControl
{
    public float3 Target;
    public float Radius;
    public float Azimuth;
    public float Elevation;

    public float OrbitSpeed;
    public float ZoomSpeed;
    public float MinRadius;
    public float MaxRadius;

    public static CameraControl Default => new()
    {
        Target = float3.Zero,
        Radius = 120f,
        Azimuth = 0f,
        Elevation = MathF.PI / 2f,
        OrbitSpeed = 0.01f,
        ZoomSpeed = 10f,
        MinRadius = 1f,
        MaxRadius = 2000f,
    };
}