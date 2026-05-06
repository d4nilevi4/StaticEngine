using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;

namespace AtomSimulation;

// Animates particles along the quantum probability current.
// For Psi_nlm = R_nl * P_l^m * e^(i*m*phi) the current is purely azimuthal:
//   j_phi ~ m / (r * sin(theta)).
// Particles drift around the polar axis; m = 0 produces no motion.
// r and theta stay fixed (each step is reprojected onto the original sphere)
// so the cloud's density profile is preserved at any TimeScale.
public struct ProbabilityFlowSystem : ISystem
{
    public void Update()
    {
        long t0 = Profiler.Timestamp();

        ref readonly var time = ref W.GetResource<Time>();
        ref readonly var state = ref W.GetResource<OrbitalState>();

        float dt = time.DeltaTime * time.TimeScale;

        var args = (dt, state.M);

        W.Query<All<OrbitalParticle>>().ForParallel(args, static
            (ref (float dt, int m) a, ref Transform t, ref Velocity v) =>
        {
            float3 p = t.Position;
            double r = Math.Sqrt(p.x * p.x + p.y * p.y + p.z * p.z);
            // Origin: spherical coords undefined, freeze the particle.
            if (r < 1e-6)
            {
                v.Value = float3.Zero;
                return;
            }

            // Cartesian -> spherical (y is the polar axis). Clamp guards FP drift.
            double theta = Math.Acos(Math.Clamp(p.y / r, -1.0, 1.0));
            double phi = Math.Atan2(p.z, p.x);

            // Cap sin(theta) away from zero: |v| diverges at the poles, but
            // particles there carry negligible weight.
            double sinTheta = Math.Sin(theta);
            if (Math.Abs(sinTheta) < 1e-4) sinTheta = 1e-4;

            // |v| from j_phi above; e_phi = (-sin(phi), 0, cos(phi)) in Cartesian.
            double v_mag = a.m / (r * sinTheta);
            float vx = (float)(-v_mag * Math.Sin(phi));
            float vz = (float)(v_mag * Math.Cos(phi));
            v.Value = new float3(vx, 0f, vz);

            // Step phi only, keep r and theta fixed by reprojecting on the sphere.
            float3 temp = p + v.Value * a.dt;
            double newPhi = Math.Atan2(temp.z, temp.x);

            float sinT = (float)Math.Sin(theta);
            float cosT = (float)Math.Cos(theta);
            float cosNP = (float)Math.Cos(newPhi);
            float sinNP = (float)Math.Sin(newPhi);
            float fr = (float)r;
            t.Position = new float3(fr * sinT * cosNP, fr * cosT, fr * sinT * sinNP);
        });

        Profiler.Record("ProbFlow", Profiler.ElapsedMs(t0));
    }

    public bool UpdateIsActive()
    {
        return !W.GetResource<Time>().Paused;
    }
}