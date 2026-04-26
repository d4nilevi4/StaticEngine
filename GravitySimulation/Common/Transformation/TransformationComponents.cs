using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;

namespace GravitySimulation;

public struct Transform : IComponent
{
    public float3 Position;
    public float3 Scale;
    public quaternion Rotation;

    public Transform()
    {
        Position = float3.Zero;
        Rotation = quaternion.Identity;
        Scale = float3.One;
    }

    public float3 Forward => Rotation.RotatePoint(float3.Forward);
    public float3 Up => Rotation.RotatePoint(float3.Up);
    public float3 Right => Rotation.RotatePoint(float3.Right);
}