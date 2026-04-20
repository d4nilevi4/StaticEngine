using System.Runtime.CompilerServices;

namespace RayEngine.Mathematics;

public readonly struct quaternion : IEquatable<quaternion>
{
    public readonly float x;
    public readonly float y;
    public readonly float z;
    public readonly float w;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public quaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public quaternion()
    {
        this.x = 0;
        this.y = 0;
        this.z = 0;
        this.w = 1;
    }

    public quaternion(float3 axis, float angleRadians)
    {
        float halfAngle = 0.5f * angleRadians;
        float s = MathF.Sin(halfAngle);
        float3 n = axis.Normalized;

        x = n.x * s;
        y = n.y * s;
        z = n.z * s;
        w = MathF.Cos(halfAngle);
    }

    public static quaternion Identity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new quaternion(0f, 0f, 0f, 1f);
    }

    public float SqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => x * x + y * y + z * z + w * w;
    }

    public float Magnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MathF.Sqrt(SqrMagnitude);
    }

    public float3 XYZ
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(x, y, z);
    }

    public bool IsValid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get =>
            float.IsFinite(x) && float.IsFinite(y) &&
            float.IsFinite(z) && float.IsFinite(w);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static quaternion operator *(quaternion a, quaternion b)
    {
        return new quaternion(
            a.x * b.w + a.w * b.x + a.y * b.z - a.z * b.y,
            a.y * b.w + a.w * b.y + a.z * b.x - a.x * b.z,
            a.z * b.w + a.w * b.z + a.x * b.y - a.y * b.x,
            a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static quaternion operator *(quaternion q, float s)
    {
        return new quaternion(q.x * s, q.y * s, q.z * s, q.w * s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static quaternion operator *(float s, quaternion q) => q * s;

    public quaternion Normalized
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            float mag = Magnitude;
            if (mag < float.Epsilon)
                return Identity;

            float inv = 1f / mag;
            return new quaternion(x * inv, y * inv, z * inv, w * inv);
        }
    }


    public quaternion Conjugated
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(-x, -y, -z, w);
    }

    public quaternion Inversed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            float invMagSq = 1f / SqrMagnitude;
            return new quaternion(-x * invMagSq, -y * invMagSq, -z * invMagSq, w * invMagSq);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float3 RotatePoint(float3 p)
    {
        // Rodrigues-like expansion of q * (p, 0) * q^-1:
        // p' = p + 2w * (v × p) + 2 * (v × (v × p))
        // where v = (x, y, z)
        float3 v = XYZ;
        float3 t = 2f * v.Cross(p);
        return p + w * t + v.Cross(t);
    }

    public float3x3 RotateMatrix(float3x3 m)
    {
        return new float3x3(
            RotatePoint(m.a1),
            RotatePoint(m.a2),
            RotatePoint(m.a3)
        );
    }

    public float3x3 ToMatrix3()
    {
        float xx = x * x, yy = y * y, zz = z * z;
        float xy = x * y, xz = x * z, yz = y * z;
        float wx = w * x, wy = w * y, wz = w * z;

        return new float3x3(
            1f - 2f * (yy + zz), 2f * (xy - wz), 2f * (xz + wy),
            2f * (xy + wz), 1f - 2f * (xx + zz), 2f * (yz - wx),
            2f * (xz - wy), 2f * (yz + wx), 1f - 2f * (xx + yy)
        );
    }

    public static quaternion FromEuler(float pitch, float yaw, float roll)
    {
        float cx = MathF.Cos(pitch * 0.5f), sx = MathF.Sin(pitch * 0.5f);
        float cy = MathF.Cos(yaw * 0.5f), sy = MathF.Sin(yaw * 0.5f);
        float cz = MathF.Cos(roll * 0.5f), sz = MathF.Sin(roll * 0.5f);

        return new quaternion(
            sx * cy * cz - cx * sy * sz,
            cx * sy * cz + sx * cy * sz,
            cx * cy * sz - sx * sy * cz,
            cx * cy * cz + sx * sy * sz
        );
    }

    public static quaternion Slerp(quaternion a, quaternion b, float t)
    {
        float dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        // Take the shortest path
        if (dot < 0f)
        {
            b = new quaternion(-b.x, -b.y, -b.z, -b.w);
            dot = -dot;
        }

        // If quaternions are very close, use linear interpolation
        if (dot > 0.9995f)
        {
            return new quaternion(
                a.x + t * (b.x - a.x),
                a.y + t * (b.y - a.y),
                a.z + t * (b.z - a.z),
                a.w + t * (b.w - a.w)
            ).Normalized;
        }

        float theta = MathF.Acos(dot);
        float sinTheta = MathF.Sin(theta);
        float wa = MathF.Sin((1f - t) * theta) / sinTheta;
        float wb = MathF.Sin(t * theta) / sinTheta;

        return new quaternion(
            wa * a.x + wb * b.x,
            wa * a.y + wb * b.y,
            wa * a.z + wb * b.z,
            wa * a.w + wb * b.w
        );
    }

    public bool Equals(quaternion other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
    }

    public override bool Equals(object? obj)
    {
        return obj is quaternion other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z, w);
    }

    public static bool operator ==(quaternion a, quaternion b) => a.Equals(b);
    public static bool operator !=(quaternion a, quaternion b) => !a.Equals(b);
    
    public static quaternion LookAt(float3 from, float3 to)
    {
        float3 forward = (to - from).Normalized;
        float3 defaultForward = new(0, 0, -1);
        float dot = defaultForward.Dot(forward);

        if (dot > 0.9999f) return quaternion.Identity;
        if (dot < -0.9999f) return new quaternion(0, 1, 0, 0);

        float3 axis = defaultForward.Cross(forward).Normalized;
        float angle = MathF.Acos(Math.Clamp(dot, -1f, 1f));
        return new quaternion(axis, angle);
    }
}