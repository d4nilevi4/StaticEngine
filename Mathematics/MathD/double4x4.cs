namespace RayEngine.Mathematics;

public readonly struct double4x4 : IEquatable<double4x4>
{
    public readonly double4 a1;
    public readonly double4 a2;
    public readonly double4 a3;
    public readonly double4 a4;

    public double4x4(double4 a1, double4 a2, double4 a3, double4 a4)
    {
        this.a1 = a1;
        this.a2 = a2;
        this.a3 = a3;
        this.a4 = a4;
    }

    public double4x4(
        double a11, double a12, double a13, double a14,
        double a21, double a22, double a23, double a24,
        double a31, double a32, double a33, double a34,
        double a41, double a42, double a43, double a44)
    {
        a1 = new double4(a11, a12, a13, a14);
        a2 = new double4(a21, a22, a23, a24);
        a3 = new double4(a31, a32, a33, a34);
        a4 = new double4(a41, a42, a43, a44);
    }

    public static double4x4 Zero => default;

    public static double4x4 Identity => new double4x4
    (new double4(1d, 0d, 0d, 0d),
        new double4(0d, 1d, 0d, 0d),
        new double4(0d, 0d, 1d, 0d),
        new double4(0d, 0d, 0d, 1d));

    public double4 this[int i]
    {
        get
        {
            switch (i)
            {
                case 0: return a1;
                case 1: return a2;
                case 2: return a3;
                case 3: return a4;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public bool Equals(double4x4 other)
    {
        return a1.Equals(other.a1) && a2.Equals(other.a2) && a3.Equals(other.a3) && a4.Equals(other.a4);
    }

    public override bool Equals(object? obj)
    {
        return obj is double4x4 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(a1, a2, a3, a4);
    }

    public static bool operator ==(double4x4 a, double4x4 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(double4x4 a, double4x4 b)
    {
        return !(a == b);
    }

    public static double4x4 operator *(double4x4 lhs, double4x4 rhs)
    {
        return new double4x4(
            lhs.a1.x * rhs.a1.x + lhs.a1.y * rhs.a2.x + lhs.a1.z * rhs.a3.x + lhs.a1.w * rhs.a4.x,
            lhs.a1.x * rhs.a1.y + lhs.a1.y * rhs.a2.y + lhs.a1.z * rhs.a3.y + lhs.a1.w * rhs.a4.y,
            lhs.a1.x * rhs.a1.z + lhs.a1.y * rhs.a2.z + lhs.a1.z * rhs.a3.z + lhs.a1.w * rhs.a4.z,
            lhs.a1.x * rhs.a1.w + lhs.a1.y * rhs.a2.w + lhs.a1.z * rhs.a3.w + lhs.a1.w * rhs.a4.w,

            lhs.a2.x * rhs.a1.x + lhs.a2.y * rhs.a2.x + lhs.a2.z * rhs.a3.x + lhs.a2.w * rhs.a4.x,
            lhs.a2.x * rhs.a1.y + lhs.a2.y * rhs.a2.y + lhs.a2.z * rhs.a3.y + lhs.a2.w * rhs.a4.y,
            lhs.a2.x * rhs.a1.z + lhs.a2.y * rhs.a2.z + lhs.a2.z * rhs.a3.z + lhs.a2.w * rhs.a4.z,
            lhs.a2.x * rhs.a1.w + lhs.a2.y * rhs.a2.w + lhs.a2.z * rhs.a3.w + lhs.a2.w * rhs.a4.w,

            lhs.a3.x * rhs.a1.x + lhs.a3.y * rhs.a2.x + lhs.a3.z * rhs.a3.x + lhs.a3.w * rhs.a4.x,
            lhs.a3.x * rhs.a1.y + lhs.a3.y * rhs.a2.y + lhs.a3.z * rhs.a3.y + lhs.a3.w * rhs.a4.y,
            lhs.a3.x * rhs.a1.z + lhs.a3.y * rhs.a2.z + lhs.a3.z * rhs.a3.z + lhs.a3.w * rhs.a4.z,
            lhs.a3.x * rhs.a1.w + lhs.a3.y * rhs.a2.w + lhs.a3.z * rhs.a3.w + lhs.a3.w * rhs.a4.w,

            lhs.a4.x * rhs.a1.x + lhs.a4.y * rhs.a2.x + lhs.a4.z * rhs.a3.x + lhs.a4.w * rhs.a4.x,
            lhs.a4.x * rhs.a1.y + lhs.a4.y * rhs.a2.y + lhs.a4.z * rhs.a3.y + lhs.a4.w * rhs.a4.y,
            lhs.a4.x * rhs.a1.z + lhs.a4.y * rhs.a2.z + lhs.a4.z * rhs.a3.z + lhs.a4.w * rhs.a4.z,
            lhs.a4.x * rhs.a1.w + lhs.a4.y * rhs.a2.w + lhs.a4.z * rhs.a3.w + lhs.a4.w * rhs.a4.w
        );
    }

    public static double4x4 operator *(double4x4 a, double b)
    {
        return new double4x4(a.a1 * b, a.a2 * b, a.a3 * b, a.a4 * b);
    }

    public static double4x4 operator *(double a, double4x4 b)
    {
        return b * a;
    }

    public static double4x4 operator /(double4x4 a, double b)
    {
        return new double4x4(a.a1 / b, a.a2 / b, a.a3 / b, a.a4 / b);
    }

    public static double4x4 operator +(double4x4 a, double4x4 b)
    {
        return new double4x4(a.a1 + b.a1, a.a2 + b.a2, a.a3 + b.a3, a.a4 + b.a4);
    }

    public static double4x4 operator -(double4x4 a, double4x4 b)
    {
        return new double4x4(a.a1 - b.a1, a.a2 - b.a2, a.a3 - b.a3, a.a4 - b.a4);
    }

    public double Trace =>
        a1.x + a2.y + a3.z + a4.w;

    public double Determinant
    {
        get
        {
            double s0 = a2.y * a3.z - a2.z * a3.y;
            double s1 = a2.y * a3.w - a2.w * a3.y;
            double s2 = a2.z * a3.w - a2.w * a3.z;
            double s3 = a2.x * a3.z - a2.z * a3.x;
            double s4 = a2.x * a3.w - a2.w * a3.x;
            double s5 = a2.x * a3.y - a2.y * a3.x;

            return
                a1.x * (s0 * a4.w - s1 * a4.z + s2 * a4.y) -
                a1.y * (s3 * a4.w - s4 * a4.z + s2 * a4.x) +
                a1.z * (s5 * a4.w - s4 * a4.y + s1 * a4.x) -
                a1.w * (s5 * a4.z - s3 * a4.y + s0 * a4.x);
        }
    }

    public double4x4 Transpose => new double4x4(
        a1.x, a2.x, a3.x, a4.x,
        a1.y, a2.y, a3.y, a4.y,
        a1.z, a2.z, a3.z, a4.z,
        a1.w, a2.w, a3.w, a4.w);

    public double3x3 Minor(int i, int j)
    {
        double
            m00 = 0,
            m01 = 0,
            m02 = 0,
            m10 = 0,
            m11 = 0,
            m12 = 0,
            m20 = 0,
            m21 = 0,
            m22 = 0;

        int yy = 0;

        for (int y = 0; y < 4; y++)
        {
            if (y == j) continue;

            int xx = 0;
            for (int x = 0; x < 4; x++)
            {
                if (x == i) continue;

                switch (xx * 3 + yy)
                {
                    case 0: m00 = this[x][y]; break;
                    case 1: m01 = this[x][y]; break;
                    case 2: m02 = this[x][y]; break;
                    case 3: m10 = this[x][y]; break;
                    case 4: m11 = this[x][y]; break;
                    case 5: m12 = this[x][y]; break;
                    case 6: m20 = this[x][y]; break;
                    case 7: m21 = this[x][y]; break;
                    case 8: m22 = this[x][y]; break;
                }

                xx++;
            }

            yy++;
        }

        return new double3x3(
            m00, m01, m02,
            m10, m11, m12,
            m20, m21, m22
        );
    }

    public double Cofactor(int i, int j)
    {
        double3x3 minor = Minor(i, j);
        double sign = ((i + j) % 2 == 0) ? 1d : -1d;
        return sign * minor.Determinant;
    }

    public double4x4 Inverse() // Can be optimized
    {
        double det = Determinant;

        if (Math.Abs(det) < double.Epsilon)
            return Identity;

        double invDet = 1d / det;

        return new double4x4(
            Cofactor(0, 0) * invDet, Cofactor(1, 0) * invDet, Cofactor(2, 0) * invDet, Cofactor(3, 0) * invDet,
            Cofactor(0, 1) * invDet, Cofactor(1, 1) * invDet, Cofactor(2, 1) * invDet, Cofactor(3, 1) * invDet,
            Cofactor(0, 2) * invDet, Cofactor(1, 2) * invDet, Cofactor(2, 2) * invDet, Cofactor(3, 2) * invDet,
            Cofactor(0, 3) * invDet, Cofactor(1, 3) * invDet, Cofactor(2, 3) * invDet, Cofactor(3, 3) * invDet
        );
    }

    public static double4x4 Orient(double3 pos, double3 forward, double3 up)
    {
        // +x - right, +y - up, -z - forward
        double3 right = up.Cross(forward).Normalized;
        up = forward.Cross(right);

        return new double4x4(
            right.x, up.x, -forward.x, pos.x,
            right.y, up.y, -forward.y, pos.y,
            right.z, up.z, -forward.z, pos.z,
            0, 0, 0, 1);
    }

    public static double4x4 LookAt(double3 eye, double3 target, double3 up)
    {
        double3 forward = (eye - target).Normalized;
        double3 right = up.Cross(forward).Normalized;
        up = forward.Cross(right);

        return new double4x4(
            right.x,   right.y,   right.z,   -right.Dot(eye),
            up.x,      up.y,      up.z,      -up.Dot(eye),
            forward.x, forward.y, forward.z, -forward.Dot(eye),
            0,         0,         0,          1);
    }

    public static double4x4 TRS(double3 position, quaternionD rotation, double3 scale)
    {
        double3x3 r = rotation.ToMatrix3();
        return new double4x4(
            r.a1.x * scale.x, r.a1.y * scale.y, r.a1.z * scale.z, position.x,
            r.a2.x * scale.x, r.a2.y * scale.y, r.a2.z * scale.z, position.y,
            r.a3.x * scale.x, r.a3.y * scale.y, r.a3.z * scale.z, position.z,
            0,                 0,                 0,                 1
        );
    }

    public static double4x4 Perspective(double fovYDegrees, double aspect, double near, double far)
    {
        double top = near * Math.Tan(fovYDegrees * 0.5d * Math.PI / 180d);
        double right = top * aspect;
        double fn = far - near;

        return new double4x4(
            near / right, 0,          0,                       0,
            0,            near / top, 0,                       0,
            0,            0,          -(far + near) / fn,     -2d * far * near / fn,
            0,            0,          -1,                      0);
    }
}
