namespace RayEngine.Mathematics;

public readonly struct double3x3 : IEquatable<double3x3>
{
    public readonly double3 a1;
    public readonly double3 a2;
    public readonly double3 a3;

    public double3x3(double3 a1, double3 a2, double3 a3)
    {
        this.a1 = a1;
        this.a2 = a2;
        this.a3 = a3;
    }

    public double3x3(double a11, double a12, double a13,
        double a21, double a22, double a23,
        double a31, double a32, double a33)
    {
        this.a1 = new double3(a11, a12, a13);
        this.a2 = new double3(a21, a22, a23);
        this.a3 = new double3(a31, a32, a33);
    }

    public static double3x3 Zero => default;
    public static double3x3 Identity => new double3x3(
        1d, 0d, 0d,
        0d, 1d, 0d,
        0d, 0d, 1d);

    public double3 this[int i]
    {
        get
        {
            switch (i)
            {
                case 0: return a1;
                case 1: return a2;
                case 2: return a3;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public bool Equals(double3x3 other)
    {
        return a1.Equals(other.a1) && a2.Equals(other.a2) && a3.Equals(other.a3);
    }

    public override bool Equals(object? obj)
    {
        return obj is double3x3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(a1, a2, a3);
    }

    public static bool operator ==(double3x3 a, double3x3 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(double3x3 a, double3x3 b)
    {
        return !(a == b);
    }

    public static double3x3 operator *(double3x3 a, double b)
    {
        return new double3x3(a.a1 * b, a.a2 * b, a.a3 * b);
    }

    public static double3x3 operator *(double a, double3x3 b)
    {
        return b * a;
    }

    public static double3x3 operator +(double3x3 a, double3x3 b)
    {
        return new double3x3(a.a1 + b.a1, a.a2 + b.a2, a.a3 + b.a3);
    }

    public double Determinant =>
        a1.x * (a2.y * a3.z - a2.z * a3.y) -
        a1.y * (a2.x * a3.z - a2.z * a3.x) +
        a1.z * (a2.x * a3.y - a2.y * a3.x);

    public double Trace => a1.x + a2.y + a3.z;

    public double3x3 Transpose => new double3x3(
        a1.x, a2.x, a3.x,
        a1.y, a2.y, a3.y,
        a1.z, a2.z, a3.z);

    public double2x2 Minor(int i, int j)
    {
        double m00 = 0, m01 = 0, m10 = 0, m11 = 0;
        int yy = 0;

        for (int y = 0; y < 3; y++)
        {
            if (y == j) continue;

            int xx = 0;
            for (int x = 0; x < 3; x++)
            {
                if (x == i) continue;

                if (xx == 0 && yy == 0) m00 = this[x][y];
                else if (xx == 1 && yy == 0) m10 = this[x][y];
                else if (xx == 0 && yy == 1) m01 = this[x][y];
                else m11 = this[x][y];

                xx++;
            }
            yy++;
        }

        return new double2x2(m00, m01, m10, m11);
    }

    public double Cofactor(int i, int j)
    {
        double2x2 minor = Minor(i, j);
        double sign = ((i + j) % 2 == 0) ? 1d : -1d;
        return sign * minor.Determinant;
    }

    public double3x3 Inverse()
    {
        double det = Determinant;

        if (Math.Abs(det) < double.Epsilon)
            return Identity;

        double invDet = 1d / det;

        return new double3x3(
            Cofactor(0, 0) * invDet, Cofactor(1, 0) * invDet, Cofactor(2, 0) * invDet,
            Cofactor(0, 1) * invDet, Cofactor(1, 1) * invDet, Cofactor(2, 1) * invDet,
            Cofactor(0, 2) * invDet, Cofactor(1, 2) * invDet, Cofactor(2, 2) * invDet
        );
    }
}
