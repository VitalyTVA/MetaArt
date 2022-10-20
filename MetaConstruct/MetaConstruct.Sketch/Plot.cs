using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;
using static MetaConstruct.Constructor;

namespace MetaConstruct;

public class Plot {

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(800, 800);
        //fullRedraw();
    }

    void draw() {
        background(Black);
        stroke(White);
        noFill();

        var center = Point();
        var top = Point();
        var centerCircle = Circle(center, top);
        var c1 = Circle(top, center);

        var c2 = Circle(Intersect(centerCircle, c1).Point1, center);
        var c3 = Circle(Intersect(centerCircle, c2).Point1, center);
        var c4 = Circle(Intersect(centerCircle, c3).Point1, center);
        var c5 = Circle(Intersect(centerCircle, c4).Point1, center);
        var c6 = Circle(Intersect(centerCircle, c5).Point1, center);

        var l1 = Line(c2.Center, c4.Center);
        var l2 = Line(c1.Center, c3.Center);

        var p1 = Intersect(l1, l2);
        var p2 = Intersect(Line(p1, c6.Center), c6);
        var l3 = Line(p1, p2.Point1);

        var plotter = new Plotter(new[] { 
            (center, new Vector2(400, 400)), 
            (top, new Vector2(400, 300)),
        });

        foreach(var c in new[] { centerCircle, c1, c2, c3, c4, c5, c6 }) {
            var circleF = plotter.CalcCircle(c);
            circle(circleF.center.X, circleF.center.Y, circleF.radius * 2);
        }

        foreach(var l in new[] { l1, l2, l3 }) {
            var lineF = plotter.CalcLine(l);
            line(lineF.from.X, lineF.from.Y, lineF.to.X, lineF.to.Y);
        }
    }
}

record struct CircleF(Vector2 center, float radius);
record struct LineF(Vector2 from, Vector2 to);
class Plotter {
    readonly Dictionary<FixedPoint, Vector2> points;
    public Plotter((FixedPoint, Vector2)[] points) {
        this.points = points.ToDictionary(
            x => x.Item1, 
            x => x.Item2,
            new DelegateEqualityComparer<FixedPoint>((x, y) => Object.ReferenceEquals(x, y), x => x.obj.GetHashCode())
        );
    }
    public CircleF CalcCircle(Circle c) {
        var center = CalcPoint(c.Center);
        var point = CalcPoint(c.Point);
        return new CircleF(center, (point - center).Length());
    }

    public LineF CalcLine(Line l) {
        var from = CalcPoint(l.From);
        var to = CalcPoint(l.To);
        return new LineF(from, to);
    }

    public Vector2 CalcPoint(Point p) {
        return p switch { 
            FixedPoint fixedPoint 
                => points[fixedPoint],
            CircleCirclePoint circleCirclePoint 
                => Intersect(CalcCircle(circleCirclePoint.Circle1), CalcCircle(circleCirclePoint.Circle2), circleCirclePoint.Intersection),
            LineLinePoint lineLinePoint
                => Intersect(CalcLine(lineLinePoint.Line1), CalcLine(lineLinePoint.Line2)),
            LineCirclePoint lineCirclePoint
                => Intersect(CalcCircle(lineCirclePoint.Circle), CalcLine(lineCirclePoint.Line), lineCirclePoint.Intersection),
            _ => throw new NotImplementedException() 
        };
    }

    static Vector2 Intersect(CircleF c, LineF l, CircleIntersectionKind intersection) {
        var (p1, p2) = FindLineCircleIntersections(c.center.X, c.center.Y, c.radius, l.from, l.to);
        return intersection == CircleIntersectionKind.First ? p1 : p2;
    }

    // Find the points of intersection.
    static (Vector2, Vector2) FindLineCircleIntersections(float cx, float cy, float radius,
                                            Vector2 point1, Vector2 point2) {
        float dx, dy, A, B, C, det, t;

        dx = point2.X - point1.X;
        dy = point2.Y - point1.Y;

        A = dx * dx + dy * dy;
        B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
        C = (point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - radius * radius;

        det = B * B - 4 * A * C;
        if((A <= 0.0000001) || (det < 0)) {
            throw new InvalidOperationException();
            // No real solutions.
            //intersection1 = new PointF(float.NaN, float.NaN);
            //intersection2 = new PointF(float.NaN, float.NaN);
            //return 0;
        } else if(det == 0) {
            throw new InvalidOperationException();
            // One solution.
            //t = -B / (2 * A);
            //intersection1 = new PointF(point1.X + t * dx, point1.Y + t * dy);
            //intersection2 = new PointF(float.NaN, float.NaN);
            //return 1;
        } else {
            // Two solutions.
            t = (float)((-B + Math.Sqrt(det)) / (2 * A));
            var intersection1 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
            t = (float)((-B - Math.Sqrt(det)) / (2 * A));
            var intersection2 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
            return (intersection1, intersection2);
        }
    }

    static Vector2 Intersect(LineF l1, LineF l2) {
        return GetIntersectingPoint(l1.from, l1.to, l2.from, l2.to);
    }

    static Vector2 GetIntersectingPoint(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2) {

        // determinant

        float d = (a1.X - a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X - b2.X);

        // check if lines are parallel
        if(MathF.FloatsEqual(d, 0)) throw new NotImplementedException();

        float px = (a1.X * a2.Y - a1.Y * a2.X) * (b1.X - b2.X) - (a1.X - a2.X) * (b1.X * b2.Y - b1.Y * b2.X);
        float py = (a1.X * a2.Y - a1.Y * a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X * b2.Y - b1.Y * b2.X);

        return new Vector2(px, py) / d;
    }

    static Vector2 Intersect(CircleF c1, CircleF c2, CircleIntersectionKind intersection) {
        var (p1, p2) = ConstructHelper.GetCircleIntersections(c1.center, c2.center, c1.radius, c2.radius)!.Value;
        return intersection == CircleIntersectionKind.First ? p1 : p2;
    }
}
abstract record Point;
sealed record FixedPoint : Point {
    public readonly object obj = new object();
    public override int GetHashCode() {
        throw new NotImplementedException();
    }
}
sealed record LineLinePoint(Line Line1, Line Line2) : Point;
enum CircleIntersectionKind { First, Second }
sealed record LineCirclePoint(Line Line, Circle Circle, CircleIntersectionKind Intersection) : Point;
sealed record CircleCirclePoint(Circle Circle1, Circle Circle2, CircleIntersectionKind Intersection) : Point;

record struct CircleIntersection(Point Point1, Point Point2);
record struct Line(Point From, Point To);
record struct Circle(Point Center, Point Point);


public class DelegateEqualityComparer<T> : IEqualityComparer<T> {
    private readonly Func<T, T, bool> _equals;
    private readonly Func<T, int> _hashCode;
    public DelegateEqualityComparer(Func<T, T, bool> equals, Func<T, int> hashCode) {
        _equals = equals;
        _hashCode = hashCode;
    }

    public bool Equals(T x, T y) {
        return _equals(x, y);
    }

    public int GetHashCode(T obj) {
        return _hashCode(obj);
    }
}

static class Constructor {
    public static FixedPoint Point() => new FixedPoint();
    public static Line Line(Point p1, Point p2) => new Line(p1, p2);
    public static Circle Circle(Point center, Point point) => new Circle(center, point);

    public static CircleIntersection Intersect(Circle c1, Circle c2) 
        => new CircleIntersection(new CircleCirclePoint(c1, c2, CircleIntersectionKind.First), new CircleCirclePoint(c1, c2, CircleIntersectionKind.Second));
    public static CircleIntersection Intersect(Line l, Circle c)
        => new CircleIntersection(new LineCirclePoint(l, c, CircleIntersectionKind.First), new LineCirclePoint(l, c, CircleIntersectionKind.Second));

    public static Point Intersect(Line l1, Line l2) => new LineLinePoint(l1, l2);

}
