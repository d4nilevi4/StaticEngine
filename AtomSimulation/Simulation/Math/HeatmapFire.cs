namespace AtomSimulation;

// Maps a probability density value to an inferno-style RGB color.
public static class HeatmapFire
{
    static readonly (float r, float g, float b)[] Stops =
    {
        (0.00f, 0.00f, 0.00f), // 0.0: black
        (0.50f, 0.00f, 0.99f), // 0.2: dark purple
        (0.80f, 0.00f, 0.00f), // 0.4: deep red
        (1.00f, 0.50f, 0.00f), // 0.6: orange
        (1.00f, 1.00f, 0.00f), // 0.8: yellow
        (1.00f, 1.00f, 1.00f), // 1.0: white
    };

    // Linearly interpolates between gradient stops.
    public static (float r, float g, float b) Fire(float value)
    {
        value = Math.Clamp(value, 0f, 1f);
        float scaled = value * (Stops.Length - 1);
        int i = (int)scaled;
        int next = Math.Min(i + 1, Stops.Length - 1);
        float t = scaled - i;
        var a = Stops[i];
        var b = Stops[next];
        return (
            a.r + t * (b.r - a.r),
            a.g + t * (b.g - a.g),
            a.b + t * (b.b - a.b)
        );
    }

    // Color of a sample at (r, theta) for orbital (n, l, m).
    // |Psi|^2 has no phi dependence, so density ~ R_nl(r)^2 * P_l^m(cos theta)^2.
    // The 1.5 * 5^n factor is a heuristic gain so high-n clouds stay visible
    // (peak density falls fast as n grows).
    public static (byte R, byte G, byte B, byte A) Inferno(double r, double theta, int n, int l, int m)
    {
        double R = HydrogenWaveFunction.RadialR(n, l, r);
        double radial = R * R;

        double Plm = HydrogenWaveFunction.AssociatedLegendre(l, m, Math.Cos(theta));
        double angular = Plm * Plm;

        double intensity = radial * angular;
        float v = (float)(intensity * 1.5 * Math.Pow(5, n));
        var (cr, cg, cb) = Fire(v);
        return (
            (byte)Math.Clamp(cr * 255f, 0f, 255f),
            (byte)Math.Clamp(cg * 255f, 0f, 255f),
            (byte)Math.Clamp(cb * 255f, 0f, 255f),
            (byte)255
        );
    }
}