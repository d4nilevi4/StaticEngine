using Raylib_cs;

namespace GravitySimulation;

// Static line-grid mesh on the XZ plane (Y = 0). The vertex shader bends it
// according to the gravitational potential of all live bodies.
internal struct SpacetimeGridMesh
{
    private uint _vaoId;
    private uint _vboId;
    private uint _eboId;
    private int _indexCount;

    public void Build(int divisions, float size)
    {
        int points = divisions + 1;

        // Regular (points × points) lattice on the XZ plane
        var positions = new float[points * points * 3];
        float halfSize = size * 0.5f;
        float cell = size / divisions;

        for (int iz = 0; iz < points; ++iz)
        for (int ix = 0; ix < points; ++ix)
        {
            int v = (iz * points + ix) * 3;
            positions[v + 0] = -halfSize + ix * cell;
            positions[v + 1] = 0f;
            positions[v + 2] = -halfSize + iz * cell;
        }

        // Line indices: x-edges per row, then z-edges per column
        int xEdges = points * divisions;
        int zEdges = points * divisions;
        _indexCount = (xEdges + zEdges) * 2;
        var indices = new uint[_indexCount];

        int w = 0;
        for (int iz = 0; iz < points; ++iz)
        for (int ix = 0; ix < divisions; ++ix)
        {
            indices[w++] = (uint)(iz * points + ix);
            indices[w++] = (uint)(iz * points + ix + 1);
        }

        for (int ix = 0; ix < points; ++ix)
        for (int iz = 0; iz < divisions; ++iz)
        {
            indices[w++] = (uint)(iz * points + ix);
            indices[w++] = (uint)((iz + 1) * points + ix);
        }

        _vaoId = Rlgl.LoadVertexArray();
        Rlgl.EnableVertexArray(_vaoId);

        unsafe
        {
            fixed (float* p = positions)
                _vboId = Rlgl.LoadVertexBuffer(p, positions.Length * sizeof(float), false);

            // Attribute 0 = position (vec3 floats)
            Rlgl.EnableVertexAttribute(0);
            Rlgl.SetVertexAttribute(0, 3, Rlgl.FLOAT, false, 0, 0);

            fixed (uint* p = indices)
                _eboId = Rlgl.LoadVertexBufferElement(p, indices.Length * sizeof(uint), false);
        }

        Rlgl.DisableVertexArray();
    }

    public void Draw()
    {
        Rlgl.EnableVertexArray(_vaoId);
        // raylib's batch path forces GL_TRIANGLES, so call glDrawElements directly for GL_LINES
        GLNative.DrawElementsLines(_indexCount);
        Rlgl.DisableVertexArray();
    }

    public void Unload()
    {
        if (_vboId != 0) Rlgl.UnloadVertexBuffer(_vboId);
        if (_eboId != 0) Rlgl.UnloadVertexBuffer(_eboId);
        if (_vaoId != 0) Rlgl.UnloadVertexArray(_vaoId);
    }
}