using Raylib_cs;

namespace GravitySimulation;

// 1×N RGBA32F texture sampled by the spacetime shader.
// Pixel layout per body: (worldX, worldZ, wellStrength, _).
internal struct SpacetimeBodyTexture
{
    private uint _texId;
    private float[] _data;

    public void Create(int maxBodies)
    {
        _data = new float[maxBodies * 4];
        unsafe
        {
            fixed (float* p = _data)
                _texId = Rlgl.LoadTexture(p, maxBodies, 1, PixelFormat.UncompressedR32G32B32A32, 1);
        }

        // texelFetch ignores filter/wrap, but set them explicitly to be safe
        Rlgl.TextureParameters(_texId, Rlgl.TEXTURE_MIN_FILTER, Rlgl.TEXTURE_FILTER_NEAREST);
        Rlgl.TextureParameters(_texId, Rlgl.TEXTURE_MAG_FILTER, Rlgl.TEXTURE_FILTER_NEAREST);
        Rlgl.TextureParameters(_texId, Rlgl.TEXTURE_WRAP_S, Rlgl.TEXTURE_WRAP_CLAMP);
        Rlgl.TextureParameters(_texId, Rlgl.TEXTURE_WRAP_T, Rlgl.TEXTURE_WRAP_CLAMP);
    }

    // Pack the first `count` bodies from GravityBodyBuffer into the texture and
    // upload it. Returns the mass-weighted COM Y coordinate.
    public float PackAndUpload(int count, float massUnit, float wellScale)
    {
        float comY = 0f;
        float totalMass = 0f;

        for (int i = 0; i < count; ++i)
        {
            float m = GravityBodyBuffer.Masses[i];

            int o = i * 4;
            _data[o + 0] = GravityBodyBuffer.Xs[i];
            _data[o + 1] = GravityBodyBuffer.Zs[i];
            _data[o + 2] = MathF.Log(1f + m / massUnit) * wellScale;
            _data[o + 3] = 0f;

            comY += m * GravityBodyBuffer.Ys[i];
            totalMass += m;
        }

        unsafe
        {
            fixed (float* p = _data)
                Rlgl.UpdateTexture(_texId, 0, 0, count, 1, PixelFormat.UncompressedR32G32B32A32, p);
        }

        return totalMass > 0f ? comY / totalMass : 0f;
    }

    public void Bind(int slot)
    {
        Rlgl.ActiveTextureSlot(slot);
        Rlgl.EnableTexture(_texId);
    }

    public static void Unbind(int slot)
    {
        Rlgl.ActiveTextureSlot(slot);
        Rlgl.DisableTexture();
    }

    public void Unload()
    {
        if (_texId != 0) Rlgl.UnloadTexture(_texId);
    }
}