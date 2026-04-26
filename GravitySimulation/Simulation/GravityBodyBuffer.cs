namespace GravitySimulation;

internal static class GravityBodyBuffer
{
    public static float[] Xs = Array.Empty<float>();
    public static float[] Ys = Array.Empty<float>();
    public static float[] Zs = Array.Empty<float>();
    public static float[] Masses = Array.Empty<float>();
    public static int Count;
    public static float Dt;

    public static void EnsureCapacity(int n)
    {
        if (n <= Xs.Length) return;
        int newLen = Math.Max(Xs.Length * 2, 64);
        while (newLen < n) newLen *= 2;
        Array.Resize(ref Xs, newLen);
        Array.Resize(ref Ys, newLen);
        Array.Resize(ref Zs, newLen);
        Array.Resize(ref Masses, newLen);
    }
}