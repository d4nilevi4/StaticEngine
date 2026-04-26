using RayEngine.Mathematics;

namespace GravitySimulation;

public static class MeshRegistry
{
    static readonly MeshData[] _meshes = new MeshData[256];
    static int _count;

    public static int Register(MeshData data)
    {
        _meshes[_count] = data;
        return _count++;
    }

    public static MeshData Get(int id) => _meshes[id];

    public static MeshData BuildUnitSphere(int stacks = 10, int sectors = 10)
    {
        var vertices = new float3[(stacks + 1) * (sectors + 1)];
        var indices = new ushort[stacks * sectors * 6];

        int vi = 0;
        for (int i = 0; i <= stacks; i++)
        {
            float theta = i / (float)stacks * MathF.PI;
            for (int j = 0; j <= sectors; j++)
            {
                float phi = j / (float)sectors * 2f * MathF.PI;
                vertices[vi++] = new float3(
                    MathF.Sin(theta) * MathF.Cos(phi),
                    MathF.Cos(theta),
                    MathF.Sin(theta) * MathF.Sin(phi)
                );
            }
        }

        int ii = 0;
        for (int i = 0; i < stacks; i++)
        {
            for (int j = 0; j < sectors; j++)
            {
                ushort v0 = (ushort)(i * (sectors + 1) + j);
                ushort v1 = (ushort)(v0 + 1);
                ushort v2 = (ushort)((i + 1) * (sectors + 1) + j);
                ushort v3 = (ushort)(v2 + 1);

                indices[ii++] = v0;
                indices[ii++] = v1;
                indices[ii++] = v2;
                indices[ii++] = v1;
                indices[ii++] = v3;
                indices[ii++] = v2;
            }
        }

        return new MeshData { Vertices = vertices, Indices = indices };
    }
}