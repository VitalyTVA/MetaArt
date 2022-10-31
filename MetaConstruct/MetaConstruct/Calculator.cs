using MetaArt.Core;

namespace MetaConstruct {
    public record struct CircleF(Vector2 center, float radius);
    public record struct LineF(Vector2 from, Vector2 to);
    public class Calculator {
        readonly Dictionary<FreePoint, Vector2> points;
        public Calculator((FreePoint, Vector2)[] points) {
            this.points = points.ToDictionary(x => x.Item1, x => x.Item2);
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
        
        readonly Dictionary<Point, Vector2> cache = new();

        public Vector2 CalcPoint(Point p) {
            if(p is FreePoint freePoint)
                return points[freePoint];
            if(cache.TryGetValue(p, out var value))
                return value;
            return cache[p] = p switch {
                CircleCirclePoint circleCirclePoint
                    => Intersect(CalcCircle(circleCirclePoint.Circle1), CalcCircle(circleCirclePoint.Circle2), CalcPoint(circleCirclePoint.Circle1.Point), circleCirclePoint.Intersection),
                LineLinePoint lineLinePoint
                    => Intersect(CalcLine(lineLinePoint.Line1), CalcLine(lineLinePoint.Line2)),
                LineCirclePoint lineCirclePoint
                    => Intersect(CalcCircle(lineCirclePoint.Circle), CalcLine(lineCirclePoint.Line), CalcPoint(lineCirclePoint.Circle.Point), lineCirclePoint.Intersection),
                _ => throw new NotImplementedException()
            };
        }
#if DEBUG
        public int LineCircleCalcCountForTests { get; private set; }
        public int LinesCalcCountForTests { get; private set; }
        public int CirclesCalcCountForTests { get; private set; }
#endif
        Vector2 Intersect(CircleF c, LineF l, Vector2 circlePoint, CircleIntersectionKind intersection) {
#if DEBUG
            LineCircleCalcCountForTests++;
#endif
            var (p1, p2) = ConstructHelper.GetLineCircleIntersections(c.center, c.radius, l.from, l.to)!.Value;
            if(intersection == CircleIntersectionKind.Secondary) {
                if(MathF.VectorsEqual(p2, circlePoint))
                    return p1;
                else if(MathF.VectorsEqual(p1, circlePoint))
                    return p2;
                throw new InvalidOperationException();
            }
            return intersection == CircleIntersectionKind.First ? p1 : p2;
        }

        Vector2 Intersect(LineF l1, LineF l2) {
#if DEBUG
            LinesCalcCountForTests++;
#endif
            return ConstructHelper.GetLinesIntersection(l1.from, l1.to, l2.from, l2.to)!.Value;
        }

        Vector2 Intersect(CircleF c1, CircleF c2, Vector2 commonPoint, CircleIntersectionKind intersection) {
#if DEBUG
            CirclesCalcCountForTests++;
#endif
            var (p1, p2) = ConstructHelper.GetCirclesIntersections(c1.center, c2.center, c1.radius, c2.radius)!.Value;
            if(intersection == CircleIntersectionKind.Secondary) {
                if(MathF.VectorsEqual(p1, commonPoint))
                    return p2;
                else if(MathF.VectorsEqual(p2, commonPoint))
                    return p1;
                else
                    throw new InvalidOperationException();
            }
            return intersection == CircleIntersectionKind.First ? p1 : p2;
        }
    }
}
