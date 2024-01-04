using System.Numerics;

public class Ray : Shape {

    public Vector3 Point { get { return point; } }
    public Vector3 Dir { get { return dir; } }
    public Vector3 InvDir { get { return invDir; } }
    public Vector3 Sign { get { return sign; } }

    public float rayLimit;
    protected Vector3 point, dir, invDir;
    protected Vector3 sign;

    public Ray(Vector3 point, Vector3 dir, float rayLimit) : base()
    {
        this.rayLimit = rayLimit;
        this.point = point;

        this.dir = dir;
        this.invDir = new Vector3(
            1f / dir.X,
            1f / dir.Y,
            1f / dir.Z
        );
        this.sign = new Vector3(
            invDir.X < 0f ? 1 : 0,
            invDir.Y < 0f ? 1 : 0,
            invDir.Z < 0f ? 1 : 0
        );
    }
}