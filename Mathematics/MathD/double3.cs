namespace RayEngine.Mathematics;

public readonly struct double3 : IEquatable<double3>
{
    public readonly double x;
    public readonly double y;
    public readonly double z;

    public double3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static double3 Zero => new double3(0d, 0d, 0d);
    public static double3 One => new double3(1d, 1d, 1d);

    public static double3 Forward => new(0d, 0d, -1d);
    public static double3 Up => new(0d, 1d, 0d);
    public static double3 Right => new(1d, 0d, 0d);



    public double this[int i]
    {
        get
        {
            switch (i)
            {
                case 0: return x;
                case 1: return y;
                case 2: return z;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public bool Equals(double3 other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
    }

    public override bool Equals(object? obj)
    {
        return obj is double3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z);
    }

    public static bool operator ==(double3 a, double3 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(double3 a, double3 b)
    {
        return !(a == b);
    }

    public static double3 operator +(double3 a, double3 b)
    {
        return new double3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static double3 operator -(double3 a, double3 b)
    {
        return new double3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static double3 operator *(double3 a, double b)
    {
        return new double3(a.x * b, a.y * b, a.z * b);
    }

    public static double3 operator *(double a, double3 b)
    {
        return b * a;
    }

    public static double3 operator /(double3 a, double b)
    {
        return new double3(a.x / b, a.y / b, a.z / b);
    }

    public double Magnitude => Math.Sqrt(x * x + y * y + z * z);
    public double SqrMagnitude => x * x + y * y + z * z;

    public double3 Normalized
    {
        get
        {
            double magnitude = Magnitude;
            return new double3(x / magnitude, y / magnitude, z / magnitude);
        }
    }

    public double3 Cross(double3 other) =>
        new double3(
            x: y * other.z - z * other.y,
            y: z * other.x - x * other.z,
            z: x * other.y - y * other.x
        );

    public double Dot(double3 other) =>
        x * other.x + y * other.y + z * other.z;
}
