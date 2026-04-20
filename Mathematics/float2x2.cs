namespace RayEngine.Mathematics;

public readonly struct float2x2 : IEquatable<float2x2>
{
    public readonly float2 a1;
    public readonly float2 a2;

    public float2x2(float2 a1, float2 a2)
    {
        this.a1 = a1;
        this.a2 = a2;
    }

    public float2x2(float a11, float a12, float a21, float a22)
    {
        this.a1 = new float2(a11, a12);
        this.a2 = new float2(a21, a22);
    }
    
    public float2 this[int i]
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

    public bool Equals(float2x2 other)
    {
        return a1.Equals(other.a1) && a2.Equals(other.a2);
    }

    public override bool Equals(object? obj)
    {
        return obj is float2x2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(a1, a2);
    }

    public static bool operator ==(float2x2 a, float2x2 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(float2x2 a, float2x2 b)
    {
        return !(a == b);
    }

    public static float2x2 operator *(float2x2 a, float b)
    {
        return new float2x2(a.a1 * b, a.a2 * b);
    }
    
    public static float2x2 operator *(float a, float2x2 b)
    {
        return b * a;
    }

    public static float2x2 operator +(float2x2 a, float2x2 b)
    {
        return new float2x2(a.a1 + b.a1, a.a2 + b.a2);
    }

    public float Determinant => a1.x * a2.y - a1.y * a2.x;
}