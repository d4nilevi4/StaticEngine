namespace RayEngine.Mathematics;

public readonly struct float3 : IEquatable<float3>
{
    public readonly float x;
    public readonly float y;
    public readonly float z;
    
    public float3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public static float3 Zero => new float3(0f, 0f, 0f);
    public static float3 One => new float3(1f, 1f, 1f);
    
    public static float3 Forward => new(0f, 0f, -1f);
    public static float3 Up => new(0f, 1f, 0f);
    public static float3 Right => new(1f, 0f, 0f);
    

    
    public float this[int i]
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
    
    public bool Equals(float3 other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
    }

    public override bool Equals(object? obj)
    {
        return obj is float3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z);
    }

    public static bool operator ==(float3 a, float3 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(float3 a, float3 b)
    {
        return !(a == b);
    }

    public static float3 operator +(float3 a, float3 b)
    {
        return new float3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static float3 operator -(float3 a, float3 b)
    {
        return new float3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static float3 operator *(float3 a, float b)
    {
        return new float3(a.x * b, a.y * b, a.z * b);
    }
    
    public static float3 operator *(float a, float3 b)
    {
        return b * a;
    }

    public static float3 operator /(float3 a, float b)
    {
        return new float3(a.x / b, a.y / b, a.z / b);
    }

    public float Magnitude => MathF.Sqrt(x * x + y * y + z * z);
    public float SqrMagnitude => x * x + y * y + z * z;

    public float3 Normalized
    {
        get
        {
            float magnitude = Magnitude;
            return new float3(x / magnitude, y / magnitude, z / magnitude);
        }
    }

    public float3 Cross(float3 other) =>
        new float3(
            x: y * other.z - z * other.y,
            y: z * other.x - x * other.z,
            z: x * other.y - y * other.x
        );

    public float Dot(float3 other) =>
        x * other.x + y * other.y + z * other.z;
}