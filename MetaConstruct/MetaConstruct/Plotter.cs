using MetaArt.Core;
using System.Numerics;

namespace MetaConstruct {
    public record Painter(
        Action<CircleF> DrawCircle,
        Action<CircleSegmentF> DrawCircleSegment,
        Action<LineF> DrawLine,
        Action<LineF> DrawLineSegment);

    public record struct CircleF(Vector2 center, float radius);
    public record struct CircleSegmentF(CircleF circle, float from, float to);
    public record struct LineF(Vector2 from, Vector2 to);
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
    public class Plotter {
        public static void Draw((FreePoint, Vector2)[] points, Painter painter, IEnumerable<Entity> primitives) {
            var plotter = new Plotter(points);
            foreach(var primitive in primitives) {
                switch(primitive) {
                    case Line l:
                        var lineF = plotter.CalcLine(l);
                        painter.DrawLine(lineF);
                        break;
                    case LineSegment l:
                        var lineSegmentF = plotter.CalcLineSegment(l.From, l.To);
                        painter.DrawLineSegment(lineSegmentF);
                        break;
                    case Circle c:
                        var circleF = plotter.CalcCircle(c);
                        painter.DrawCircle(circleF);
                        break;
                    case CircleSegment segment:
                        var circleSegmentF = plotter.CalcCircleSegment(segment);
                        painter.DrawCircleSegment(circleSegmentF);
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

        CircleSegmentF CalcCircleSegment(CircleSegment segment) {
            var circle = CalcCircle(segment.Circle);
            var from = CalcPoint(segment.From);
            var to = CalcPoint(segment.To);
            float angleFrom = ConstructHelper.AngleTo(from - circle.center);
            float angleTo = ConstructHelper.AngleTo(to - circle.center);
            if(angleFrom > angleTo)
                angleFrom -= MathF.PI * 2;
            return new CircleSegmentF(circle, angleFrom, angleTo);
        }

        LineF CalcLineSegment(Point p1, Point p2) {
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
}
