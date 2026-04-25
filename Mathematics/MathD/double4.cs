namespace RayEngine.Mathematics;

public readonly struct double4 : IEquatable<double4>
{
    public readonly double x;
    public readonly double y;
    public readonly double z;
    public readonly double w;

    public double4(double x, double y, double z, double w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public static double4 Zero => new double4(0d, 0d, 0d, 0d);

    public double this[int i]
    {
        get
        {
            switch (i)
            {
                case 0: return x;
                case 1: return y;
                case 2: return z;
                case 3: return w;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public bool Equals(double4 other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
    }

    public override bool Equals(object? obj)
    {
        return obj is double4 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z, w);
    }

    public static bool operator ==(double4 a, double4 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(double4 a, double4 b)
    {
        return !(a == b);
    }

    public static double4 operator +(double4 a, double4 b)
    {
        return new double4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    public static double4 operator -(double4 a, double4 b)
    {
        return new double4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    }

    public static double4 operator *(double4 a, double b)
    {
        return new double4(a.x * b, a.y * b, a.z * b, a.w * b);
    }

    public static double4 operator /(double4 a, double b)
    {
        return new double4(a.x / b, a.y / b, a.z / b, a.w / b);
    }

    public double Magnitude => Math.Sqrt(x * x + y * y + z * z + w * w);
    public double SqrMagnitude => x * x + y * y + z * z + w * w;

    public double4 Normalized
    {
        get
        {
            double magnitude = Magnitude;
            return new double4(x / magnitude, y / magnitude, z / magnitude, w / magnitude);
        }
    }

    public double Dot(double4 other) =>
        x * other.x + y * other.y + z * other.z + w * other.w;
}
