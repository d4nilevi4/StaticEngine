using System.Numerics;
using System.Runtime.CompilerServices;

namespace RayEngine.Mathematics;

public readonly struct float4x4 : IEquatable<float4x4>
{
    public readonly float4 a1;
    public readonly float4 a2;
    public readonly float4 a3;
    public readonly float4 a4;

    public float4x4(float4 a1, float4 a2, float4 a3, float4 a4)
    {
        this.a1 = a1;
        this.a2 = a2;
        this.a3 = a3;
        this.a4 = a4;
    }

    public float4x4(
        float a11, float a12, float a13, float a14,
        float a21, float a22, float a23, float a24,
        float a31, float a32, float a33, float a34,
        float a41, float a42, float a43, float a44)
    {
        a1 = new float4(a11, a12, a13, a14);
        a2 = new float4(a21, a22, a23, a24);
        a3 = new float4(a31, a32, a33, a34);
        a4 = new float4(a41, a42, a43, a44);
    }

    public static float4x4 Zero => default;

    public static float4x4 Identity => new float4x4
    (new float4(1f, 0f, 0f, 0f),
        new float4(0f, 1f, 0f, 0f),
        new float4(0f, 0f, 1f, 0f),
        new float4(0f, 0f, 0f, 1f));

    public float4 this[int i]
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

    public bool Equals(float4x4 other)
    {
        return a1.Equals(other.a1) && a2.Equals(other.a2) && a3.Equals(other.a3) && a4.Equals(other.a4);
    }

    public override bool Equals(object? obj)
    {
        return obj is float4x4 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(a1, a2, a3, a4);
    }

    public static bool operator ==(float4x4 a, float4x4 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(float4x4 a, float4x4 b)
    {
        return !(a == b);
    }

    public static float4x4 operator *(float4x4 lhs, float4x4 rhs)
    {
        return new float4x4(
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
    
    public static float4x4 operator *(float4x4 a, float b)
    {
        return new float4x4(a.a1 * b, a.a2 * b, a.a3 * b, a.a4 * b);
    }
    
    public static float4x4 operator *(float a, float4x4 b)
    {
        return b * a;
    }

    public static float4x4 operator /(float4x4 a, float b)
    {
        return new float4x4(a.a1 / b, a.a2 / b, a.a3 / b, a.a4 / b);
    }

    public static float4x4 operator +(float4x4 a, float4x4 b)
    {
        return new float4x4(a.a1 + b.a1, a.a2 + b.a2, a.a3 + b.a3, a.a4 + b.a4);
    }

    public static float4x4 operator -(float4x4 a, float4x4 b)
    {
        return new float4x4(a.a1 - b.a1, a.a2 - b.a2, a.a3 - b.a3, a.a4 - b.a4);
    }

    public float Trace =>
        a1.x + a2.y + a3.z + a4.w;

    public float Determinant
    {
        get
        {
            float s0 = a2.y * a3.z - a2.z * a3.y;
            float s1 = a2.y * a3.w - a2.w * a3.y;
            float s2 = a2.z * a3.w - a2.w * a3.z;
            float s3 = a2.x * a3.z - a2.z * a3.x;
            float s4 = a2.x * a3.w - a2.w * a3.x;
            float s5 = a2.x * a3.y - a2.y * a3.x;

            return
                a1.x * (s0 * a4.w - s1 * a4.z + s2 * a4.y) -
                a1.y * (s3 * a4.w - s4 * a4.z + s2 * a4.x) +
                a1.z * (s5 * a4.w - s4 * a4.y + s1 * a4.x) -
                a1.w * (s5 * a4.z - s3 * a4.y + s0 * a4.x);
        }
    }

    public float4x4 Transpose => new float4x4(
        a1.x, a2.x, a3.x, a4.x,
        a1.y, a2.y, a3.y, a4.y,
        a1.z, a2.z, a3.z, a4.z,
        a1.w, a2.w, a3.w, a4.w);

    public float3x3 Minor(int i, int j)
    {
        float
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

        return new float3x3(
            m00, m01, m02,
            m10, m11, m12,
            m20, m21, m22
        );
    }

    public float Cofactor(int i, int j)
    {
        float3x3 minor = Minor(i, j);
        float sign = ((i + j) % 2 == 0) ? 1f : -1f;
        return sign * minor.Determinant;
    }

    public float4x4 Inverse() // Can be optimized
    {
        float det = Determinant;

        if (MathF.Abs(det) < float.Epsilon)
            return Identity;

        float invDet = 1f / det;

        return new float4x4(
            Cofactor(0, 0) * invDet, Cofactor(1, 0) * invDet, Cofactor(2, 0) * invDet, Cofactor(3, 0) * invDet,
            Cofactor(0, 1) * invDet, Cofactor(1, 1) * invDet, Cofactor(2, 1) * invDet, Cofactor(3, 1) * invDet,
            Cofactor(0, 2) * invDet, Cofactor(1, 2) * invDet, Cofactor(2, 2) * invDet, Cofactor(3, 2) * invDet,
            Cofactor(0, 3) * invDet, Cofactor(1, 3) * invDet, Cofactor(2, 3) * invDet, Cofactor(3, 3) * invDet
        );
    }

    public static float4x4 Orient(float3 pos, float3 forward, float3 up)
    {
        // +x - right, +y - up, -z - forward
        float3 right = up.Cross(forward).Normalized;
        up = forward.Cross(right);

        return new float4x4(
            right.x, up.x, -forward.x, pos.x,
            right.y, up.y, -forward.y, pos.y,
            right.z, up.z, -forward.z, pos.z,
            0, 0, 0, 1);
    }

    public static float4x4 LookAt(float3 eye, float3 target, float3 up)
    {
        float3 forward = (eye - target).Normalized;
        float3 right = up.Cross(forward).Normalized;
        up = forward.Cross(right);

        return new float4x4(
            right.x,   right.y,   right.z,   -right.Dot(eye),
            up.x,      up.y,      up.z,      -up.Dot(eye),
            forward.x, forward.y, forward.z, -forward.Dot(eye),
            0,         0,         0,          1);
    }

    public static float4x4 TRS(float3 position, quaternion rotation, float3 scale)
    {
        float3x3 r = rotation.ToMatrix3();
        return new float4x4(
            r.a1.x * scale.x, r.a1.y * scale.y, r.a1.z * scale.z, position.x,
            r.a2.x * scale.x, r.a2.y * scale.y, r.a2.z * scale.z, position.y,
            r.a3.x * scale.x, r.a3.y * scale.y, r.a3.z * scale.z, position.z,
            0,                 0,                 0,                 1
        );
    }

    public static float4x4 Perspective(float fovYDegrees, float aspect, float near, float far)
    {
        float top = near * MathF.Tan(fovYDegrees * 0.5f * MathF.PI / 180f);
        float right = top * aspect;
        float fn = far - near;

        return new float4x4(
            near / right, 0,          0,                       0,
            0,            near / top, 0,                       0,
            0,            0,          -(far + near) / fn,     -2f * far * near / fn,
            0,            0,          -1,                      0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix4x4 ToNumerics()
    {
        return new Matrix4x4(
            a1.x, a1.y, a1.z, a1.w,
            a2.x, a2.y, a2.z, a2.w,
            a3.x, a3.y, a3.z, a3.w,
            a4.x, a4.y, a4.z, a4.w);
    }
}