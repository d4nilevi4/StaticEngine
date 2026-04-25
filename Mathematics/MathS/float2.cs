namespace RayEngine.Mathematics;

public readonly struct float2 : IEquatable<float2>
{
    public readonly float x;
    public readonly float y;

    public float2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public float this[int i]
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

    public bool Equals(float2 other)
    {
        return x.Equals(other.x) && y.Equals(other.y);
    }

    public override bool Equals(object? obj)
    {
        return obj is float2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public static bool operator ==(float2 a, float2 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(float2 a, float2 b)
    {
        return !(a == b);
    }

    public static float2 operator +(float2 a, float2 b)
    {
        return new float2(a.x + b.x, a.y + b.y);
    }

    public static float2 operator -(float2 a, float2 b)
    {
        return new float2(a.x - b.x, a.y - b.y);
    }

    public static float2 operator *(float2 a, float b)
    {
        return new float2(a.x * b, a.y * b);
    }

    public static float2 operator *(float a, float2 b)
    {
        return b * a;
    }

    public static float2 operator /(float2 a, float b)
    {
        return new float2(a.x / b, a.y / b);
    }

    public float Magnitude => MathF.Sqrt(x * x + y * y);
    public float SqrMagnitude => x * x + y * y;

    public float2 Normalized
    {
        get
        {
            float magnitude = Magnitude;
            return new float2(x / magnitude, y / magnitude);
        }
    }
}