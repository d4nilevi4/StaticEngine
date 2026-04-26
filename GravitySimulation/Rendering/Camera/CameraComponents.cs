using FFS.Libraries.StaticEcs;

namespace GravitySimulation;

public struct IsMainCamera : ITag
{
}

public struct CameraPerspectiveProjection : IComponent
{
    public float Fov;
    public float Near;
    public float Far;

    public static CameraPerspectiveProjection Default => new()
    {
        Fov = 45f,
        Near = 100f,
        Far = 750000f
    };
}

public struct CameraControl
{
    public float Yaw;
    public float Pitch;
    public float Speed;

    public float MouseSensitivity;
    public float ZoomStep;
    public float MinSpeed;
    public float MaxSpeed;
    public float PitchLimit;

    public static CameraControl Default => new()
    {
        Yaw = 0f,
        Pitch = 0f,
        Speed = 10000f,
        MouseSensitivity = 0.05f,
        ZoomStep = 0.15f,
        MinSpeed = 1f,
        MaxSpeed = 1_000_000f,
        PitchLimit = 89f,
    };
}