namespace RayEngine.Mathematics;

public readonly struct double2 : IEquatable<double2>
{
    public readonly double x;
    public readonly double y;

    public double2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public double this[int i]
    {
        get
        {
            switch (i)
            {
                case 0: return x;
                case 1: return y;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public bool Equals(double2 other)
    {
        return x.Equals(other.x) && y.Equals(other.y);
    }

    public override bool Equals(object? obj)
    {
        return obj is double2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public static bool operator ==(double2 a, double2 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(double2 a, double2 b)
    {
        return !(a == b);
    }

    public static double2 operator +(double2 a, double2 b)
    {
        return new double2(a.x + b.x, a.y + b.y);
    }

    public static double2 operator -(double2 a, double2 b)
    {
        return new double2(a.x - b.x, a.y - b.y);
    }

    public static double2 operator *(double2 a, double b)
    {
        return new double2(a.x * b, a.y * b);
    }

    public static double2 operator *(double a, double2 b)
    {
        return b * a;
    }

    public static double2 operator /(double2 a, double b)
    {
        return new double2(a.x / b, a.y / b);
    }

    public double Magnitude => Math.Sqrt(x * x + y * y);
    public double SqrMagnitude => x * x + y * y;

    public double2 Normalized
    {
        get
        {
            double magnitude = Magnitude;
            return new double2(x / magnitude, y / magnitude);
        }
    }
}
