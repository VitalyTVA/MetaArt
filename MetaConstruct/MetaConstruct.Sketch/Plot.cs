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
        stroke(White);
        noFill();

        var center = Point();
        var top = Point();
        var centerCircle = Circle(center, top);
        var c1 = new Circle(top, center);

        var c2 = new Circle(Intersect(centerCircle, c1).Point1, center);
        var c3 = Circle(Intersect(centerCircle, c2).Point1, center);
        var c4 = Circle(Intersect(centerCircle, c3).Point1, center);
        var c5 = Circle(Intersect(centerCircle, c4).Point1, center);
        var c6 = Circle(Intersect(centerCircle, c5).Point1, center);

        var plotter = new Plotter(new[] { 
            (center, new Vector2(400, 400)), 
            (top, new Vector2(400, 300)),
        });

        foreach(var c in new[] { centerCircle, c1, c2, c3, c4, c5, c6 }) {
            var circleF = plotter.CalcCircle(c);
            circle(circleF.center.X, circleF.center.Y, circleF.radius * 2);
        }
    }
}

record struct CircleF(Vector2 center, float radius);
record struct LineF(Vector2 from, float to);
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

    //public LineF CalcLine(Line c) {
    //    throw new NotImplementedException();
    //}
    public Vector2 CalcPoint(Point p) {
        return p switch { 
            FixedPoint fixedPoint 
                => points[fixedPoint],
            CircleCirclePoint circleCirclePoint 
                => Intersect(CalcCircle(circleCirclePoint.Circle1), CalcCircle(circleCirclePoint.Circle2), circleCirclePoint.Intersection),
            _ => throw new NotImplementedException() 
        };
    }

    static Vector2 Intersect(CircleF c1, CircleF c2, CircleIntersectionKind intersection) {
        var (p1, p2) = GetCircleIntersections(c1.center, c2.center, c1.radius, c2.radius);
        return intersection == CircleIntersectionKind.First ? p1 : p2;
    }

    static (Vector2, Vector2) GetCircleIntersections(Vector2 center1, Vector2 center2, float radius1, float radius2) {
        var (r1, r2) = (radius1, radius2);
        (float x1, float y1, float x2, float y2) = (center1.X, center1.Y, center2.X, center2.Y);
        // d = distance from center1 to center2
        float d = MathF.Sqrt(MathF.Pow(x1 - x2, 2) + MathF.Pow(y1 - y2, 2));
        // Return an empty array if there are no intersections
        if(!(MathF.Abs(r1 - r2) <= d && d <= r1 + r2)) throw new NotImplementedException();

        // Intersections i1 and possibly i2 exist
        var dsq = d * d;
        var (r1sq, r2sq) = (r1 * r1, r2 * r2);
        var r1sq_r2sq = r1sq - r2sq;
        var a = r1sq_r2sq / (2 * dsq);
        var c = MathF.Sqrt(2 * (r1sq + r2sq) / dsq - (r1sq_r2sq * r1sq_r2sq) / (dsq * dsq) - 1);

        var fx = (x1 + x2) / 2 + a * (x2 - x1);
        var gx = c * (y2 - y1) / 2;

        var fy = (y1 + y2) / 2 + a * (y2 - y1);
        var gy = c * (x1 - x2) / 2;

        var i1 = new Vector2((float)(fx + gx), (float)(fy + gy));
        var i2 = new Vector2((float)(fx - gx), (float)(fy - gy));

        return (i1, i2);
    }
}

abstract record Point;
sealed record FixedPoint : Point {
    public readonly object obj = new object();
    public override int GetHashCode() {
        throw new NotImplementedException();
    }
}
sealed record LineLinePoint(Line Line1, Line Lin2) : Point;
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
