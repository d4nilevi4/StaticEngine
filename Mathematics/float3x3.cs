namespace RayEngine.Mathematics;

public readonly struct float3x3 : IEquatable<float3x3>
{
    public readonly float3 a1;
    public readonly float3 a2;
    public readonly float3 a3;

    public float3x3(float3 a1, float3 a2, float3 a3)
    {
        this.a1 = a1;
        this.a2 = a2;
        this.a3 = a3;
    }

    public float3x3(float a11, float a12, float a13,
        float a21, float a22, float a23,
        float a31, float a32, float a33)
    {
        this.a1 = new float3(a11, a12, a13);
        this.a2 = new float3(a21, a22, a23);
        this.a3 = new float3(a31, a32, a33);
    }

    public static float3x3 Zero => default;
    public static float3x3 Identity => new float3x3(
        1f, 0f, 0f,
        0f, 1f, 0f,
        0f, 0f, 1f);
    
    public float3 this[int i]
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
    
    public bool Equals(float3x3 other)
    {
        return a1.Equals(other.a1) && a2.Equals(other.a2) && a3.Equals(other.a3);
    }

    public override bool Equals(object? obj)
    {
        return obj is float3x3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(a1, a2, a3);
    }
    
    public static bool operator ==(float3x3 a, float3x3 b)
    {
        return a.Equals(b);
    }
    
    public static bool operator !=(float3x3 a, float3x3 b)
    {
        return !(a == b);
    }
    
    public static float3x3 operator *(float3x3 a, float b)
    {
        return new float3x3(a.a1 * b, a.a2 * b, a.a3 * b);
    }
    
    public static float3x3 operator *(float a, float3x3 b)
    {
        return b * a;
    }
    
    public static float3x3 operator +(float3x3 a, float3x3 b)
    {
        return new float3x3(a.a1 + b.a1, a.a2 + b.a2, a.a3 + b.a3);
    }
    
    public float Determinant =>
        a1.x * (a2.y * a3.z - a2.z * a3.y) -
        a1.y * (a2.x * a3.z - a2.z * a3.x) +
        a1.z * (a2.x * a3.y - a2.y * a3.x);
    
    public float Trace => a1.x + a2.y + a3.z;
    
    public float3x3 Transpose => new float3x3(
        a1.x, a2.x, a3.x,
        a1.y, a2.y, a3.y,
        a1.z, a2.z, a3.z);
    
    public float2x2 Minor(int i, int j)
    {
        float m00 = 0, m01 = 0, m10 = 0, m11 = 0;
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

        return new float2x2(m00, m01, m10, m11);
    }

    public float Cofactor(int i, int j)
    {
        float2x2 minor = Minor(i, j);
        float sign = ((i + j) % 2 == 0) ? 1f : -1f;
        return sign * minor.Determinant;
    }
    
    public float3x3 Inverse()
    {
        float det = Determinant;

        if (MathF.Abs(det) < float.Epsilon)
            return Identity;

        float invDet = 1f / det;

        return new float3x3(
            Cofactor(0, 0) * invDet, Cofactor(1, 0) * invDet, Cofactor(2, 0) * invDet,
            Cofactor(0, 1) * invDet, Cofactor(1, 1) * invDet, Cofactor(2, 1) * invDet,
            Cofactor(0, 2) * invDet, Cofactor(1, 2) * invDet, Cofactor(2, 2) * invDet
        );
    }
}