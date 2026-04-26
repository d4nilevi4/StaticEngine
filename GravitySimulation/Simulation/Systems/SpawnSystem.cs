using FFS.Libraries.StaticEcs;
using RayEngine.Mathematics;

namespace GravitySimulation;

public struct SpawnSystem : ISystem
{
    public int SmallCount;
    public int MediumCount;
    public int LargeCount;

    public SpawnSystem()
    {
        SmallCount = 2000;
        MediumCount = 1000;
        LargeCount = 10;
    }

    const float SunMass = 1.989e30f;

    public void Init()
    {
        int sphereId = MeshRegistry.Register(MeshRegistry.BuildUnitSphere());

        SpawnCamera();
        SpawnSun(sphereId);
        SpawnBodies(sphereId);
    }

    static void SpawnCamera()
    {
        float3 pos = new float3(0f, 20_000f, 65_000f);
        W.NewEntity<Default>().Set(
            new Transform
            {
                Position = pos,
                Rotation = quaternion.LookAt(pos - float3.Up * -10_000, float3.Zero),
                Scale = float3.One
            },
            CameraPerspectiveProjection.Default
        ).Set<IsMainCamera>();
    }

    static void SpawnSun(int meshId)
    {
        var e = W.NewEntity<Default>().Set(
            new Transform { Position = float3.Zero, Scale = float3.One * 3000f },
            new Velocity { Value = float3.Zero },
            new Mass { Value = SunMass },
            new MeshHandle { Id = meshId },
            new MeshColor { R = 255, G = 220, B = 80, A = 255 }
        );
        e.Set<GravityBody>();
    }

    void SpawnBodies(int meshId)
    {
        var rng = new Random(42);

        for (int i = 0; i < SmallCount; i++)
            SpawnOrbiter(meshId, rng,
                minRadius: 8_000f, maxRadius: 40_000f,
                minMass: 1e16f, maxMass: 5e17f,
                minSize: 5f, maxSize: 15f,
                r: 160, g: 150, b: 140);

        for (int i = 0; i < MediumCount; i++)
            SpawnOrbiter(meshId, rng,
                minRadius: 8_000f, maxRadius: 40_000f,
                minMass: 5e19f, maxMass: 5e20f,
                minSize: 40f, maxSize: 120f,
                r: 80, g: 140, b: 220);

        for (int i = 0; i < LargeCount; i++)
            SpawnOrbiter(meshId, rng,
                minRadius: 8_000f, maxRadius: 40_000f,
                minMass: 1e21f, maxMass: 2e24f,
                minSize: 200f, maxSize: 500f,
                r: 200, g: 170, b: 110);
    }

    static void SpawnOrbiter(int meshId, Random rng,
        float minRadius, float maxRadius,
        float minMass, float maxMass,
        float minSize, float maxSize,
        byte r, byte g, byte b)
    {
        float orbitR = Lerp(minRadius, maxRadius, rng);
        float mass = Lerp(minMass, maxMass, rng);
        float size = Lerp(minSize, maxSize, rng);
        float angle = (float)(rng.NextDouble() * 2.0 * Math.PI);
        float incl = (float)(rng.NextDouble() * 0.1 - 0.05);

        float3 pos = new float3(
            orbitR * MathF.Cos(angle),
            orbitR * incl,
            orbitR * MathF.Sin(angle)
        );

        float v = OrbitalVelocity(orbitR, SunMass);
        float3 vel = new float3(-MathF.Sin(angle), 0f, MathF.Cos(angle)) * v;

        var e = W.NewEntity<Default>().Set(
            new Transform { Position = pos, Scale = float3.One * size },
            new Velocity { Value = vel },
            new Mass { Value = mass },
            new MeshHandle { Id = meshId },
            new MeshColor { R = r, G = g, B = b, A = 255 }
        );
        e.Set<GravityBody>();
    }

    static float OrbitalVelocity(float orbitalRadius, float centralMass)
    {
        const double G = 6.6743e-11;
        const float MetersPerUnit = 1000f;
        double distM = orbitalRadius * MetersPerUnit;
        return (float)(Math.Sqrt(G * centralMass / distM) / MetersPerUnit);
    }

    static float Lerp(float min, float max, Random rng) =>
        min + (float)rng.NextDouble() * (max - min);
}