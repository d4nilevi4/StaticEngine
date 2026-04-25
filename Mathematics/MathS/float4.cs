namespace RayEngine.Mathematics;

public readonly struct float4 : IEquatable<float4>
{
    public readonly float x;
    public readonly float y;
    public readonly float z;
    public readonly float w;
    
    public float4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    
    public static float4 Zero => new float4(0f, 0f, 0f, 0f);
    
    public float this[int i]
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

    public bool Equals(float4 other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
    }

    public override bool Equals(object? obj)
    {
        return obj is float4 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z, w);
    }

    public static bool operator ==(float4 a, float4 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(float4 a, float4 b)
    {
        return !(a == b);
    }

    public static float4 operator +(float4 a, float4 b)
    {
        return new float4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    public static float4 operator -(float4 a, float4 b)
    {
        return new float4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    }

    public static float4 operator *(float4 a, float b)
    {
        return new float4(a.x * b, a.y * b, a.z * b, a.w * b);
    }

    public static float4 operator /(float4 a, float b)
    {
        return new float4(a.x / b, a.y / b, a.z / b, a.w / b);
    }

    public float Magnitude => MathF.Sqrt(x * x + y * y + z * z + w * w);
    public float SqrMagnitude => x * x + y * y + z * z + w * w;

    public float4 Normalized
    {
        get
        {
            float magnitude = Magnitude;
            return new float4(x / magnitude, y / magnitude, z / magnitude, w / magnitude);
        }
    }

    public float Dot(float4 other) =>
        x * other.x + y * other.y + z * other.z + w * other.w;
}