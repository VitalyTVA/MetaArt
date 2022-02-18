using System.Diagnostics;
using System.Numerics;
using MetaArt.D3;
using static MetaArt.D3.MathFEx;

namespace MetaArt.D3;
public static class Extensions {
    public static Vector3 GetDirection(this YawPitchContoller controller) {
        return Vector3.Transform(new Vector3(0, 0, 1), Quaternion.Conjugate(controller.CreateRotation()));
    }

    public static Camera CreateCamera(this YawPitchContoller controller, float radius, float focalDistance) {
        var q = controller.CreateRotation();

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

            focalDistance
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
        return (GreaterOrEqual(s1, 0) && GreaterOrEqual(s2, 0) && GreaterOrEqual(s3, 0) && GreaterOrEqual(s4, 0) && GreaterOrEqual(s_cam, 0))
            ||
            (LessOrEqual(s1, 0) && LessOrEqual(s2, 0) && LessOrEqual(s3, 0) && LessOrEqual(s4, 0) && LessOrEqual(s_cam, 0));
    }

    //public static Vector2 NoZ(this Vector3 v) => new Vector2(v.X, v.Y);

    public static bool PointInside((Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4) q, Vector2 p) {
        if(q.v3 == q.v4) {
            return PointsOnSameSideOfLine((q.v1, q.v2), (q.v3, p, p))
                && PointsOnSameSideOfLine((q.v2, q.v3), (q.v1, p, p))
                && PointsOnSameSideOfLine((q.v3, q.v1), (q.v2, p, p));
        }
        return PointsOnSameSideOfLine((q.v1, q.v2), (q.v3, q.v4, p))
            && PointsOnSameSideOfLine((q.v2, q.v3), (q.v1, q.v4, p))
            && PointsOnSameSideOfLine((q.v3, q.v4), (q.v1, q.v2, p))
            && PointsOnSameSideOfLine((q.v4, q.v1), (q.v2, q.v3, p));
    }
    public static Vector2? GetPointInside((Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4) q, Vector2 p) {
        return PointInside(q, p) ? p : default(Vector2?);
    }
    public static bool PointsOnSameSideOfLine((Vector2 v1, Vector2 v2) line, (Vector2 v1, Vector2 v2, Vector2 v3) points) {
        var n = Vector2.Normalize(GetNormal(line.v1, line.v2));
        var s1 = Vector2.Dot(n, (line.v1 - points.v1));
        var s2 = Vector2.Dot(n, (line.v1 - points.v2));
        var s3 = Vector2.Dot(n, (line.v1 - points.v3));
        return (Greater(s1, 0) && Greater(s2, 0) && Greater(s3, 0))
            ||
            (Less(s1, 0) && Less(s2, 0) && Less(s3, 0));
    }

    public static bool SegmentsIntesect((Vector2 v1, Vector2 v2) s1, (Vector2 v1, Vector2 v2) s2) {
        //TODO optimize segments intersection
        var (x1, y1) = s1.v1;
        var (x2, y2) = s1.v2;
        var (x3, y3) = s2.v1;
        var (x4, y4) = s2.v2;

        var d = (y4 - y3) * (x2 - x1) - (x4 - x3)* (y2 - y1);
        var ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / d;
        var ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / d;
        return Greater(ua, 0) && Less(ua, 1) && Greater(ub, 0) && Less(ub, 1);
    }

    public static Vector2? GetQuadsIntersection((Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4) q1, (Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4) q2) {
        //TODO Test all variants
        var intersection =
            GetPointInside(q1, q2.v1)
            ?? GetPointInside(q1, q2.v2)
            ?? GetPointInside(q1, q2.v3)
            ?? GetPointInside(q1, q2.v4)
            ?? GetPointInside(q2, q1.v1)
            ?? GetPointInside(q2, q1.v2)
            ?? GetPointInside(q2, q1.v3)
            ?? GetPointInside(q2, q1.v4)
            ?? GetSegmentsIntesection((q1.v1, q1.v2), (q2.v1, q2.v2))
            ?? GetSegmentsIntesection((q1.v1, q1.v2), (q2.v2, q2.v3))
            ?? GetSegmentsIntesection((q1.v1, q1.v2), (q2.v3, q2.v4))
            ?? GetSegmentsIntesection((q1.v1, q1.v2), (q2.v4, q2.v1))

            ?? GetSegmentsIntesection((q1.v2, q1.v3), (q2.v1, q2.v2))
            ?? GetSegmentsIntesection((q1.v2, q1.v3), (q2.v2, q2.v3))
            ?? GetSegmentsIntesection((q1.v2, q1.v3), (q2.v3, q2.v4))
            ?? GetSegmentsIntesection((q1.v2, q1.v3), (q2.v4, q2.v1))

            ?? GetSegmentsIntesection((q1.v3, q1.v4), (q2.v1, q2.v2))
            ?? GetSegmentsIntesection((q1.v3, q1.v4), (q2.v2, q2.v3))
            ?? GetSegmentsIntesection((q1.v3, q1.v4), (q2.v3, q2.v4))
            ?? GetSegmentsIntesection((q1.v3, q1.v4), (q2.v4, q2.v1))

            ?? GetSegmentsIntesection((q1.v4, q1.v1), (q2.v1, q2.v2))
            ?? GetSegmentsIntesection((q1.v4, q1.v1), (q2.v2, q2.v3))
            ?? GetSegmentsIntesection((q1.v4, q1.v1), (q2.v3, q2.v4))
            ?? GetSegmentsIntesection((q1.v4, q1.v1), (q2.v4, q2.v1));
        return intersection;
    }

    static Vector2? GetSegmentsIntesection((Vector2 v1, Vector2 v2) s1, (Vector2 v1, Vector2 v2) s2) {
        //TODO optimize segments intersection
        var (x1, y1) = s1.v1;
        var (x2, y2) = s1.v2;
        var (x3, y3) = s2.v1;
        var (x4, y4) = s2.v2;

        var d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
        var ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / d;
        var ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / d;
        if(Greater(ua, 0) && Less(ua, 1) && Greater(ub, 0) && Less(ub, 1)) {
            return new Vector2(x1 + (x2 - x1) * ua, y1 + (y2 - y1) * ua);
        }
        return null;
    }

    public static Vector3 PlaneLineIntersection((Vector3 v1, Vector3 v2, Vector3 v3) plane, (Vector3 l0, Vector3 l) line) {
        //TODO test it
        //https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection

        var n = Vector3.Normalize(GetNormal(plane.v1, plane.v2, plane.v3));

        var (l0, l) = line;
        var d = Vector3.Dot(plane.v1 - l0, n) / Vector3.Dot(l, n);
        return l0 + l * d;
    }
}
