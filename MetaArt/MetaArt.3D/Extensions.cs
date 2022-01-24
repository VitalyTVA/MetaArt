using System.Numerics;
using MetaArt.D3;
using static MetaArt.D3.MathF;

namespace MetaArt.D3;
public class Model {
    public readonly Vector3[] Vertices;
    public readonly (int, int, int, int)[] Quads;
    public Model(Vector3[] vertices, (int, int, int, int)[] quads) {
        Vertices = vertices;
        Quads = quads;
    }
}
public enum MoveDirection {
    Left, Right, Up, Down
}

public class SphereCameraContoller {
    float yaw;
    float pitch;

    public void Move(MoveDirection direction) {
        float step = PI / 60;

        if(direction == MoveDirection.Left) yaw -= step;
        if(direction == MoveDirection.Right) yaw += step;
        if(direction == MoveDirection.Up && pitch > -PI / 4) pitch -= step;
        if(direction == MoveDirection.Down && pitch < PI / 4) pitch += step;
    }

    public Camera CreateCamera() {
        var q = Quaternion.CreateFromYawPitchRoll(0, pitch, 0) * Quaternion.CreateFromYawPitchRoll(yaw, 0, 0);

        var v = Vector3.Transform(new Vector3(0, 0, -600), Quaternion.Conjugate(q));

        //circle(0, 0, 50);
        var c = new Camera(
            //new Vector3(0, 300, -300),
            //Quaternion.CreateFromYawPitchRoll(0.0f, -PI / 4, 0.0f),

            //new Vector3(300, 0, -300),
            //Quaternion.CreateFromYawPitchRoll(PI / 4, 0.0f, 0.0f),

            //new Vector3(300, 0, -300),
            //Quaternion.CreateFromAxisAngle(new Vector3(0, 1f, 0), PI / 4), 

            //new Vector3(0, 300, -300),
            //Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -PI / 4), 

            //new Vector3(0, 0, -300),
            //Quaternion.Identity,

            v,
            q,

            400
        );
        return c;

    }
}
public static class MathF {
    public static readonly float PI = (float)Math.PI;
}
public static class Extensions {
    public static Model CreateCube(float side) {
        return new Model(new[] {
            new Vector3(side, side, side),
            new Vector3(side, -side, side),
            new Vector3(-side, -side, side),
            new Vector3(-side, side, side),

            new Vector3(side, side, -side),
            new Vector3(side, -side, -side),
            new Vector3(-side, -side, -side),
            new Vector3(-side, side, -side),
        },
        new[] {
            (0, 1, 2, 3),
            (7, 6, 5, 4),
            (4, 5, 1, 0),
            (3, 2, 6, 7),
            (3, 7, 4, 0),
            (1, 5, 6, 2),
        });
    }
    public static Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3) {
        return Vector3.Cross(p2 - p1, p3 - p2);
    }

    public static bool IsVisible(this Camera c, Vector3 vertex, Vector3 normal) {
        return Vector3.Dot(vertex - c.Location, normal) > 0;
    }

    public static void Deconstruct(this Vector3 v, out float x, out float y, out float z) {
        x = v.X;
        y = v.Y;
        z = v.Z;
    }
    public static void Deconstruct(this Vector2 v, out float x, out float y) {
        x = v.X;
        y = v.Y;
    }
}
