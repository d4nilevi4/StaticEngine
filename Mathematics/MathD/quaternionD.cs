using System.Runtime.CompilerServices;

namespace RayEngine.Mathematics;

public readonly struct quaternionD : IEquatable<quaternionD>
{
    public readonly double x;
    public readonly double y;
    public readonly double z;
    public readonly double w;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public quaternionD(double x, double y, double z, double w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public quaternionD()
    {
        this.x = 0;
        this.y = 0;
        this.z = 0;
        this.w = 1;
    }

    public quaternionD(double3 axis, double angleRadians)
    {
        double halfAngle = 0.5d * angleRadians;
        double s = Math.Sin(halfAngle);
        double3 n = axis.Normalized;

        x = n.x * s;
        y = n.y * s;
        z = n.z * s;
        w = Math.Cos(halfAngle);
    }

    public static quaternionD Identity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new quaternionD(0d, 0d, 0d, 1d);
    }

    public double SqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => x * x + y * y + z * z + w * w;
    }

    public double Magnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Math.Sqrt(SqrMagnitude);
    }

    public double3 XYZ
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(x, y, z);
    }

    public bool IsValid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get =>
            double.IsFinite(x) && double.IsFinite(y) &&
            double.IsFinite(z) && double.IsFinite(w);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static quaternionD operator *(quaternionD a, quaternionD b)
    {
        return new quaternionD(
            a.x * b.w + a.w * b.x + a.y * b.z - a.z * b.y,
            a.y * b.w + a.w * b.y + a.z * b.x - a.x * b.z,
            a.z * b.w + a.w * b.z + a.x * b.y - a.y * b.x,
            a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static quaternionD operator *(quaternionD q, double s)
    {
        return new quaternionD(q.x * s, q.y * s, q.z * s, q.w * s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static quaternionD operator *(double s, quaternionD q) => q * s;

    public quaternionD Normalized
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            double mag = Magnitude;
            if (mag < double.Epsilon)
                return Identity;

            double inv = 1d / mag;
            return new quaternionD(x * inv, y * inv, z * inv, w * inv);
        }
    }


    public quaternionD Conjugated
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(-x, -y, -z, w);
    }

    public quaternionD Inversed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            double invMagSq = 1d / SqrMagnitude;
            return new quaternionD(-x * invMagSq, -y * invMagSq, -z * invMagSq, w * invMagSq);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double3 RotatePoint(double3 p)
    {
        // Rodrigues-like expansion of q * (p, 0) * q^-1:
        // p' = p + 2w * (v × p) + 2 * (v × (v × p))
        // where v = (x, y, z)
        double3 v = XYZ;
        double3 t = 2d * v.Cross(p);
        return p + w * t + v.Cross(t);
    }

    public double3x3 RotateMatrix(double3x3 m)
    {
        return new double3x3(
            RotatePoint(m.a1),
            RotatePoint(m.a2),
            RotatePoint(m.a3)
        );
    }

    public double3x3 ToMatrix3()
    {
        double xx = x * x, yy = y * y, zz = z * z;
        double xy = x * y, xz = x * z, yz = y * z;
        double wx = w * x, wy = w * y, wz = w * z;

        return new double3x3(
            1d - 2d * (yy + zz), 2d * (xy - wz), 2d * (xz + wy),
            2d * (xy + wz), 1d - 2d * (xx + zz), 2d * (yz - wx),
            2d * (xz - wy), 2d * (yz + wx), 1d - 2d * (xx + yy)
        );
    }

    public static quaternionD FromEuler(double pitch, double yaw, double roll)
    {
        double cx = Math.Cos(pitch * 0.5d), sx = Math.Sin(pitch * 0.5d);
        double cy = Math.Cos(yaw * 0.5d), sy = Math.Sin(yaw * 0.5d);
        double cz = Math.Cos(roll * 0.5d), sz = Math.Sin(roll * 0.5d);

        return new quaternionD(
            sx * cy * cz - cx * sy * sz,
            cx * sy * cz + sx * cy * sz,
            cx * cy * sz - sx * sy * cz,
            cx * cy * cz + sx * sy * sz
        );
    }

    public static quaternionD Slerp(quaternionD a, quaternionD b, double t)
    {
        double dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        // Take the shortest path
        if (dot < 0d)
        {
            b = new quaternionD(-b.x, -b.y, -b.z, -b.w);
            dot = -dot;
        }

        // If quaternions are very close, use linear interpolation
        if (dot > 0.9995d)
        {
            return new quaternionD(
                a.x + t * (b.x - a.x),
                a.y + t * (b.y - a.y),
                a.z + t * (b.z - a.z),
                a.w + t * (b.w - a.w)
            ).Normalized;
        }

        double theta = Math.Acos(dot);
        double sinTheta = Math.Sin(theta);
        double wa = Math.Sin((1d - t) * theta) / sinTheta;
        double wb = Math.Sin(t * theta) / sinTheta;

        return new quaternionD(
            wa * a.x + wb * b.x,
            wa * a.y + wb * b.y,
            wa * a.z + wb * b.z,
            wa * a.w + wb * b.w
        );
    }

    public bool Equals(quaternionD other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
    }

    public override bool Equals(object? obj)
    {
        return obj is quaternionD other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z, w);
    }

    public static bool operator ==(quaternionD a, quaternionD b) => a.Equals(b);
    public static bool operator !=(quaternionD a, quaternionD b) => !a.Equals(b);

    public static quaternionD LookAt(double3 from, double3 to)
    {
        double3 forward = (to - from).Normalized;
        double3 defaultForward = new(0, 0, -1);
        double dot = defaultForward.Dot(forward);

        if (dot > 0.9999d) return quaternionD.Identity;
        if (dot < -0.9999d) return new quaternionD(0, 1, 0, 0);

        double3 axis = defaultForward.Cross(forward).Normalized;
        double angle = Math.Acos(Math.Clamp(dot, -1d, 1d));
        return new quaternionD(axis, angle);
    }
}
