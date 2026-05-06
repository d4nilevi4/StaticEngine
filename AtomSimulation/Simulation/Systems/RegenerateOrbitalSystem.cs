using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;

namespace AtomSimulation;

// Rebuilds the particle cloud when an OrbitalDirty marker is present.
// The whole population is destroyed and resampled because the distribution
// depends on (n, l, m) globally.
public struct RegenerateOrbitalSystem : ISystem
{
    private OrbitalSampler _sampler;
    private Random _rng;

    public void Init()
    {
        _sampler = new OrbitalSampler();
        // Fixed seed: identical (n, l, m) reproduces the same cloud across runs.
        _rng = new Random(42);
    }

    public void Update()
    {
        if (W.Query<All<OrbitalDirty>>().EntitiesCount() == 0)
        {
            Profiler.Record("OrbitalRegen", 0f);
            return;
        }

        long t0 = Profiler.Timestamp();

        ref readonly var state = ref W.GetResource<OrbitalState>();

        W.Query<All<OrbitalParticle>>().BatchDestroy();
        W.Query<All<OrbitalDirty>>().BatchDestroy();

        _sampler.EnsureCdfs(state.N, state.L, state.M);

        // Billboard size grows with n to keep on-screen density similar.
        float scale = state.N / 30f;

        // Skip near-black particles to save vertex bandwidth.
        const int DimThreshold = 30;

        for (int i = 0; i < state.ParticleCount; i++)
        {
            double r = _sampler.SampleR(_rng);
            double theta = _sampler.SampleTheta(_rng);
            double phi = _sampler.SamplePhi(_rng);

            var (R, G, B, A) = HeatmapFire.Inferno(r, theta, state.N, state.L, state.M);
            if (R + G + B < DimThreshold) continue;

            // Spherical -> Cartesian with y as the polar axis:
            //   x = r sin(theta) cos(phi), y = r cos(theta), z = r sin(theta) sin(phi).
            float sinT = (float)Math.Sin(theta);
            float cosT = (float)Math.Cos(theta);
            float cosP = (float)Math.Cos(phi);
            float sinP = (float)Math.Sin(phi);
            float fr = (float)r;
            float3 pos = new float3(fr * sinT * cosP, fr * cosT, fr * sinT * sinP);

            W.NewEntity<Default>().Set(
                new Transform { Position = pos, Scale = float3.One * scale, Rotation = quaternion.Identity },
                new Velocity { Value = float3.Zero },
                new ParticleColor { R = R, G = G, B = B, A = A }
            ).Set<OrbitalParticle>();
        }

        Profiler.Record("OrbitalRegen", Profiler.ElapsedMs(t0));
    }
}