using System.Numerics;
using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;

namespace GravitySimulation;

public struct ApplyGravityForceSystem : ISystem
{
    public void Update()
    {
        long t0 = Profiler.Timestamp();

        if (GravityBodyBuffer.Count > 0)
        {
            W.Query<All<GravityBody, Mass>>()
                .WriteBlock<Velocity>().Read<Transform>()
                .ForParallel<Kernel>(minEntitiesPerThread: 32);
        }

        Profiler.Record("GravityForce", Profiler.ElapsedMs(t0));
    }

    public bool UpdateIsActive()
    {
        return !W.GetResource<Time>().Paused;
    }

    private readonly struct Kernel : W.IQueryBlock.Write<Velocity>.Read<Transform>
    {
        // Folded constant: a[unit/s²] = (G[m³·kg⁻¹·s⁻²] / MetersPerUnit³) · m[kg] / r²[unit²].
        // With MetersPerUnit = 1000 (1 unit = 1 km), Gscale = 6.6743e-11 / 1e9 = 6.6743e-20.
        const float Gscale = 6.6743e-20f;

        // Plummer softening squared. Replaces r² with r² + Eps² in the denominator to:
        //   1) remove the r → 0 singularity (self-pair and very close encounters)
        //   2) keep the inner loop branch-free so SIMD can run straight through
        // Effective softening radius ≈ √Eps² = 0.01 unit (10 m at this scale).
        const float Eps2 = 1e-4f;

        public void Invoke(uint count, W.EntityBlock entities, Block<Velocity> vels, BlockR<Transform> tfs)
        {
            var xs = GravityBodyBuffer.Xs;
            var ys = GravityBodyBuffer.Ys;
            var zs = GravityBodyBuffer.Zs;
            var ms = GravityBodyBuffer.Masses;
            int n = GravityBodyBuffer.Count;
            float dt = GravityBodyBuffer.Dt;

            // SIMD setup: lane count is platform-defined (NEON: 4, AVX2: 8, AVX-512: 16).
            // Constants are broadcast once (replicated across all lanes) and reused for every body.
            int lanes = Vector<float>.Count;
            int simdN = n - (n % lanes);
            var Eps2V = new Vector<float>(Eps2);
            var GscaleV = new Vector<float>(Gscale);
            var oneV = Vector<float>.One;

            for (uint i = 0; i < count; i++)
            {
                // Broadcast self-position into vectors (every lane = same self component)
                // so it can be subtracted from a packed group of "other" positions in one op.
                var selfPos = tfs[i].Position;
                var sx = new Vector<float>(selfPos.x);
                var sy = new Vector<float>(selfPos.y);
                var sz = new Vector<float>(selfPos.z);
                var ax = Vector<float>.Zero;
                var ay = Vector<float>.Zero;
                var az = Vector<float>.Zero;

                int j = 0;
                for (; j < simdN; j += lanes)
                {
                    // Contiguous SoA load: one SIMD-load per array pulls `lanes` neighbouring bodies.
                    var vx = new Vector<float>(xs, j);
                    var vy = new Vector<float>(ys, j);
                    var vz = new Vector<float>(zs, j);
                    var vm = new Vector<float>(ms, j);

                    // d = p_other - p_self  (direction toward each of the 4 other bodies)
                    var dx = vx - sx;
                    var dy = vy - sy;
                    var dz = vz - sz;

                    // Newton's gravity, rearranged to avoid an extra division:
                    //   a = (d / r) · (Gscale · m / r²) = d · (Gscale · m / r³)
                    // → compute 1/r once, cube it, then multiply by mass and Gscale.
                    var distSq = dx * dx + dy * dy + dz * dz + Eps2V;
                    var invDist = oneV / Vector.SquareRoot(distSq);
                    var invDist3 = invDist * invDist * invDist;
                    var coef = GscaleV * vm * invDist3;

                    // Accumulate per-axis acceleration. After the loop ax/ay/az hold
                    // partial sums spread across SIMD lanes — reduced below.
                    ax += dx * coef;
                    ay += dy * coef;
                    az += dz * coef;
                }

                // Horizontal reduction: collapse SIMD-lane partial sums into scalar totals.
                float aX = Vector.Sum(ax);
                float aY = Vector.Sum(ay);
                float aZ = Vector.Sum(az);

                // Tail loop: same physics, scalar, for the (n % lanes) remaining bodies
                // that didn't fit a full SIMD step.

                for (; j < n; j++)
                {
                    float ddx = xs[j] - selfPos.x;
                    float ddy = ys[j] - selfPos.y;
                    float ddz = zs[j] - selfPos.z;
                    float distSq = ddx * ddx + ddy * ddy + ddz * ddz + Eps2;
                    float invDist = 1f / MathF.Sqrt(distSq);
                    float coef = Gscale * ms[j] * invDist * invDist * invDist;
                    aX += ddx * coef;
                    aY += ddy * coef;
                    aZ += ddz * coef;
                }

                // Semi-implicit Euler: velocity gets the new acceleration this step;
                // IntegrateMotionSystem then advances position using the updated velocity.
                vels[i].Value += new float3(aX, aY, aZ) * dt;
            }
        }
    }
}