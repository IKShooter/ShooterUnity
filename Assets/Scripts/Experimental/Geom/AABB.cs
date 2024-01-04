using System.Numerics;

public class AABB
{
    public Vector3 Min { get { return min; } }
    public Vector3 Max { get { return max; } }
    public Vector3 Size => max - min;
    public Vector3 Center { get { return (min + max) * 0.5f; } }

    protected Vector3 min, max;
    
    public AABB(Vector3 min, Vector3 max) : base()
    {
        this.min = Vector3.Min(min, max);
        this.max = Vector3.Max(min, max);
    }
}