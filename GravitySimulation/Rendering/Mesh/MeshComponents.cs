using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;

namespace GravitySimulation;

public class MeshData
{
    public required float3[] Vertices;
    public required ushort[] Indices;
}

public struct MeshHandle : IComponent { public int Id; }
public struct MeshColor : IComponent { public byte R, G, B, A; }
