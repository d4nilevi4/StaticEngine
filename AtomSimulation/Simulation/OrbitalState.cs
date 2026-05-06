namespace AtomSimulation;

// Hydrogen orbital quantum numbers (n, l, m) and how many points to sample.
public struct OrbitalState
{
    public int N;
    public int L;
    public int M;
    public int ParticleCount;

    public static OrbitalState Default => new()
    {
        N = 5,
        L = 3,
        M = 1,
        ParticleCount = 250_000,
    };
}