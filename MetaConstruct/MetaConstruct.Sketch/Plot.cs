using MetaArt.Core;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using static MetaConstruct.Constructor;

namespace MetaConstruct;

class Plot {
    Painter painter = null!;
    readonly PlotInfo info;

    public Plot(Func<PlotInfo> getPlot) {
        info = getPlot();
    }

    void setup() {
        if(deviceType() == DeviceType.Desktop)
            size(800, 800);
        //fullRedraw();

        painter = new Painter(
            DrawCircle: circle => {
                stroke(White);
                Sketch.circle(circle.center.X, circle.center.Y, circle.radius * 2);
            },
            DrawLine: l => {
                var v = Vector2.Normalize(l.from - l.to);
                var p1 = l.to + v * (width + height);
                var p2 = l.from - v * (width + height);
                stroke(color(50));
                line(p1.X, p1.Y, p2.X, p2.Y);
            },
            DrawLineSegment: l => {
                stroke(White);
                line(l.from.X, l.from.Y, l.to.X, l.to.Y);
            }
        );
    }

    void draw() {
        background(Black);
        stroke(White);
        noFill();
        Plotter.Draw(info.Points, painter, info.Entities);
    }
}

static class PlotsHelpers {
    public static readonly (Func<PlotInfo> action, string name)[] Plots = new[] {
            RegisterPlot(Test),
            RegisterPlot(SixCircles),
        };
    static (Func<PlotInfo>, string) RegisterPlot(Func<PlotInfo> action, [CallerArgumentExpression("action")] string name = "") {
        return (action, name);
    }

    static PlotInfo Test() {
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

        return new PlotInfo(
            new[] {
                (center, new Vector2(400, 400)),
                (top, new Vector2(400, 300)),
            },
            new Entity[] { 
                centerCircle, 
                c1, c2, c3, c4, c5, c6, l1, l2, l3, 
                LineSegment(l1), LineSegment(l2), LineSegment(l3) 
            }
        );
    }

    static PlotInfo SixCircles() {
        var center = Point();
        var top = Point();
        var centerCircle = Circle(center, top);
        var c1 = Circle(top, center);

        var c2 = Circle(Intersect(centerCircle, c1).Point1, center);
        var c3 = Circle(Intersect(centerCircle, c2).Point1, center);
        var c4 = Circle(Intersect(centerCircle, c3).Point1, center);
        var c5 = Circle(Intersect(centerCircle, c4).Point1, center);
        var c6 = Circle(Intersect(centerCircle, c5).Point1, center);

        return new PlotInfo(
            new[] {
                (center, new Vector2(400, 400)),
                (top, new Vector2(400, 300)),
            },
            new Entity[] { centerCircle, c1, c2, c3, c4, c5, c6 }
        );
    }
}

record struct PlotInfo((FreePoint, Vector2)[] Points, Entity[] Entities);

record Painter(Action<CircleF> DrawCircle, Action<LineF> DrawLine, Action<LineF> DrawLineSegment);

record struct CircleF(Vector2 center, float radius);
record struct LineF(Vector2 from, Vector2 to);

class Plotter {
    public static void Draw((FreePoint, Vector2)[] points, Painter painter, IEnumerable<Entity> primitives) { 
        var plotter = new Plotter(points);
        foreach(var primitive in primitives) {
            switch(primitive) {
                case Line l:
                    var lineF = plotter.CalcLine(l);
                    painter.DrawLine(lineF);
                    break;
                case LineSegment l:
                    l.Verify();
                    var lineSegmentF = plotter.CalcLine(l.From, l.To);
                    painter.DrawLineSegment(lineSegmentF);
                    break;
                case Circle c:
                    var circleF = plotter.CalcCircle(c);
                    painter.DrawCircle(circleF);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    readonly Dictionary<FreePoint, Vector2> points;
    Plotter((FreePoint, Vector2)[] points) {
        this.points = points.ToDictionary(
            x => x.Item1, 
            x => x.Item2,
            new DelegateEqualityComparer<FreePoint>((x, y) => ReferenceEquals(x, y), x => x.GetHashCodeEx())
        );
    }
    CircleF CalcCircle(Circle c) {
        var center = CalcPoint(c.Center);
        var point = CalcPoint(c.Point);
        return new CircleF(center, (point - center).Length());
    }

    LineF CalcLine(Point p1, Point p2) {
        var from = CalcPoint(p1);
        var to = CalcPoint(p2);
        return new LineF(from, to);
    }

    LineF CalcLine(Line l) {
        var from = CalcPoint(l.From);
        var to = CalcPoint(l.To);
        return new LineF(from, to);
    }

    Vector2 CalcPoint(Point p) {
        return p switch { 
            FreePoint fixedPoint 
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
        var (p1, p2) = ConstructHelper.GetLineCircleIntersections(c.center, c.radius, l.from, l.to)!.Value;
        return intersection == CircleIntersectionKind.First ? p1 : p2;
    }

    static Vector2 Intersect(LineF l1, LineF l2) {
        return ConstructHelper.GetLinesIntersection(l1.from, l1.to, l2.from, l2.to)!.Value;
    }

    static Vector2 Intersect(CircleF c1, CircleF c2, CircleIntersectionKind intersection) {
        var (p1, p2) = ConstructHelper.GetCirclesIntersections(c1.center, c2.center, c1.radius, c2.radius)!.Value;
        return intersection == CircleIntersectionKind.First ? p1 : p2;
    }
}

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
