using System.Numerics;
using MetaArt.D3;
using static MetaArt.D3.MathFEx;

namespace MetaArt.D3;
public static class Extensions {
    public static Vector3 GetDirection(this YawPitchContoller controller) {
        return Vector3.Transform(new Vector3(0, 0, 1), Quaternion.Conjugate(controller.CreateRotation()));
    }

    public static Camera CreateCamera(this YawPitchContoller controller) {
        var q = controller.CreateRotation();

        var radius = 600;

        var v = controller.GetDirection() * (-radius);

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


    public static void Rotate<T>(this Model<T> model, Camera c, float dx, float dy) {
        var rotationSpeed = .01f;
        if(dx == 0 && dy == 0) 
            return;
        var v = new Vector3(dy, -dx, 0);
        var axis = Vector3.Transform(Vector3.Normalize(v), Quaternion.Conjugate(c.Rotation));
        var rotation = Quaternion.CreateFromAxisAngle(axis, v.Length() * rotationSpeed);
        model.Rotate(rotation);

        //var r = Quaternion.Conjugate(c.Rotation);
        //var axisX = Vector3.Transform(new Vector3(0, 1, 0), r);
        //var axisY = Vector3.Transform(new Vector3(1, 0, 0), r);
        //var rotationX = Quaternion.CreateFromAxisAngle(axisX, -dx * rotationSpeed);
        //var rotationY = Quaternion.CreateFromAxisAngle(axisY, -dy * rotationSpeed);
        //model.Rotation = rotationX * rotationY * model.Rotation;
    }
    public static void Rotate<T>(this Model<T> model, Quaternion rotation) {
        model.Rotation = rotation * model.Rotation;
    }
    public static Model<T> CreateCube<T>(float side, (T front, T back, T left, T right, T top, T bottom) sides) {
        return new Model<T>(new[] {
            new Vector3(side, side, side),
            new Vector3(side, -side, side),
            new Vector3(-side, -side, side),
            new Vector3(-side, side, side),

            new Vector3(side, side, -side),
            new Vector3(side, -side, -side),
            new Vector3(-side, -side, -side),
            new Vector3(-side, side, -side),
        },
        new Quad<T>[] {
            (7, 6, 5, 4, sides.front),
            (0, 1, 2, 3, sides.back),
            (4, 5, 1, 0, sides.right),
            (3, 2, 6, 7, sides.left),
            (3, 7, 4, 0, sides.top),
            (1, 5, 6, 2, sides.bottom),
        });
    }
    public static Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3) {
        return Vector3.Cross(p3 - p2, p2 - p1);
    }
    public static Vector2 GetNormal(Vector2 p1, Vector2 p2) {
        var (x, y) = p2 - p1;
        return new Vector2(-y, x);
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

    public static bool VerticesOnSameSideOfPlane((Vector3 v1, Vector3 v2, Vector3 v3) plane, (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) vertices, bool cameraOnSameSide) {
        var n = Vector3.Normalize(GetNormal(plane.v1, plane.v2, plane.v3));
        var s1 = Vector3.Dot(n, (plane.v1 - vertices.v1));
        var s2 = Vector3.Dot(n, (plane.v1 - vertices.v2));
        var s3 = Vector3.Dot(n, (plane.v1 - vertices.v3));
        var s4 = Vector3.Dot(n, (plane.v1 - vertices.v4));
        var s_cam = Vector3.Dot(n, plane.v1); //camera is at origin here
        if(!cameraOnSameSide)
            s_cam = -s_cam;
        return (Greater(s1, 0) && Greater(s2, 0) && Greater(s3, 0) && Greater(s4, 0) && Greater(s_cam, 0))
            ||
            (Less(s1, 0) && Less(s2, 0) && Less(s3, 0) && Less(s4, 0) && Less(s_cam, 0));
    }

    public static Vector2 NoZ(this Vector3 v) => new Vector2(v.X, v.Y);

    public static bool Intersects((Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4) q1, (Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4) q2) {
        return PointInside(q1, q2.v1)
            || PointInside(q1, q2.v2)
            || PointInside(q1, q2.v3)
            || PointInside(q1, q2.v4)
            || PointInside(q2, q1.v1)
            || PointInside(q2, q1.v2)
            || PointInside(q2, q1.v3)
            || PointInside(q2, q1.v4);
    }
    public static bool PointInside((Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4) q, Vector2 p) { 
        if(q.v3 == q.v4)
            throw new NotImplementedException();
        return PointOnSameSideOfLine((q.v1, q.v2), (q.v3, q.v4, p))
            && PointOnSameSideOfLine((q.v2, q.v3), (q.v1, q.v4, p))
            && (/*q.v3 == q.v4 || */PointOnSameSideOfLine((q.v3, q.v4), (q.v1, q.v2, p)))
            && PointOnSameSideOfLine((q.v4, q.v1), (q.v2, q.v3, p));
    }
    public static bool PointOnSameSideOfLine((Vector2 v1, Vector2 v2) line, (Vector2 v1, Vector2 v2, Vector2 v3) points) {
        var n = Vector2.Normalize(GetNormal(line.v1, line.v2));
        var s1 = Vector2.Dot(n, (line.v1 - points.v1));
        var s2 = Vector2.Dot(n, (line.v1 - points.v2));
        var s3 = Vector2.Dot(n, (line.v1 - points.v3));
        return (Greater(s1, 0) && Greater(s2, 0) && Greater(s3, 0))
            ||
            (Less(s1, 0) && Less(s2, 0) && Less(s3, 0));
    }
}
