namespace RayEngine.Mathematics;

public readonly struct double2x2 : IEquatable<double2x2>
{
    public readonly double2 a1;
    public readonly double2 a2;

    public double2x2(double2 a1, double2 a2)
    {
        this.a1 = a1;
        this.a2 = a2;
    }

    public double2x2(double a11, double a12, double a21, double a22)
    {
        this.a1 = new double2(a11, a12);
        this.a2 = new double2(a21, a22);
    }

    public double2 this[int i]
    {
        get
        {
            switch (i)
            {
                case 0: return a1;
                case 1: return a2;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public bool Equals(double2x2 other)
    {
        return a1.Equals(other.a1) && a2.Equals(other.a2);
    }

    public override bool Equals(object? obj)
    {
        return obj is double2x2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(a1, a2);
    }

    public static bool operator ==(double2x2 a, double2x2 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(double2x2 a, double2x2 b)
    {
        return !(a == b);
    }

    public static double2x2 operator *(double2x2 a, double b)
    {
        return new double2x2(a.a1 * b, a.a2 * b);
    }

    public static double2x2 operator *(double a, double2x2 b)
    {
        return b * a;
    }

    public static double2x2 operator +(double2x2 a, double2x2 b)
    {
        return new double2x2(a.a1 + b.a1, a.a2 + b.a2);
    }

    public double Determinant => a1.x * a2.y - a1.y * a2.x;
}
