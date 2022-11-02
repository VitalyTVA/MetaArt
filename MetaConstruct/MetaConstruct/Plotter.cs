using MetaArt.Core;
using System.Numerics;

namespace MetaConstruct {
    public record Painter(
        Action<CircleF, DisplayStyle> DrawCircle,
        Action<CircleSegmentF, DisplayStyle> DrawCircleSegment,
        Action<LineF, DisplayStyle> DrawLine,
        Action<LineF, DisplayStyle> DrawLineSegment,
        Action<CircleSegmentF[], DisplayStyle> FillContour,
        Action<Vector2, PointKind, DisplayStyle> DrawPoint
    );
    public enum PointKind { Free, Circles, Lines, LineCircle }
    public record struct CircleSegmentF(CircleF circle, float from, float to);

    public class Surface {
        readonly int PointHitTestDistance;
        public Surface(int pointHitTestDistance) {
            PointHitTestDistance = pointHitTestDistance;
        }

        public Constructor Constructor { get; } = new();
        readonly List<(Entity, DisplayStyle)> entities = new List<(Entity, DisplayStyle)>();
        public void Add(Entity entity, DisplayStyle style) => entities.Add((entity, style));

        public IEnumerable<(Entity, DisplayStyle)> GetEntities() => entities;
        Dictionary<FreePoint, Vector2> points = null!;

        public void SetPoints((FreePoint, Vector2)[] points) {
            this.points = points.ToDictionary(x => x.Item1, x => x.Item2);
        }

        public Vector2 GetPointLocation(FreePoint p) => points[p];

        public FreePoint? HitTest(Vector2 point) {
            var closestPoint = points.OrderBy(x => Vector2.DistanceSquared(x.Value, point)).First();
            if(Vector2.DistanceSquared(closestPoint.Value, point) < PointHitTestDistance * PointHitTestDistance)
                return closestPoint.Key;
            return null;
        }

        public void SetPointLocation(FreePoint point, Vector2 location) { 
            points[point] = location;
        }
    }

    public static class Plotter {
        public static void Draw(Surface surface, Painter painter) {
            var calculator = new Calculator(surface.GetPointLocation);
            foreach(var (primitive, style) in surface.GetEntities()) {
                switch(primitive) {
                    case Line l:
                        var lineF = calculator.CalcLine(l);
                        painter.DrawLine(lineF, style);
                        break;
                    case LineSegment l:
                        var lineSegmentF = calculator.CalcLineSegment(l.From, l.To);
                        painter.DrawLineSegment(lineSegmentF, style);
                        break;
                    case Circle c:
                        var circleF = calculator.CalcCircle(c);
                        painter.DrawCircle(circleF, style);
                        break;
                    case CircleSegment segment:
                        var circleSegmentF = calculator.CalcCircleSegment(segment);
                        painter.DrawCircleSegment(circleSegmentF, style);
                        break;
                    case Contour contour:
                        var segments = calculator.CalcContour(contour);
                        painter.FillContour(segments, style);
                        break;
                    case PointView point:
                        var kind = point.point switch { 
                            FreePoint => PointKind.Free,
                            LineCirclePoint => PointKind.LineCircle,
                            CircleCirclePoint => PointKind.Circles,
                            LineLinePoint => PointKind.Lines,
                            _ => throw new NotImplementedException(),
                        };
                        var pointF = calculator.CalcPoint(point.point);
                        painter.DrawPoint(pointF, kind, style);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        static CircleSegmentF[] CalcContour(this Calculator calculator, Contour contour) {
            var segments = contour.Segments
                .Cast<CircleSegment>()
                .Select(s => calculator.CalcCircleSegment(s))
                .ToArray();
            return segments;
        }

        static CircleSegmentF CalcCircleSegment(this Calculator calculator, CircleSegment segment) {
            var circle = calculator.CalcCircle(segment.Circle);
            var from = calculator.CalcPoint(segment.From);
            var to = calculator.CalcPoint(segment.To);
            float angleFrom = ConstructHelper.AngleTo(from - circle.center);
            float angleTo = ConstructHelper.AngleTo(to - circle.center);
            if(angleFrom > angleTo)
                angleFrom -= MathF.PI * 2;
            return new CircleSegmentF(circle, angleFrom, angleTo);
        }

        static LineF CalcLineSegment(this Calculator calculator, Point p1, Point p2) {
            var from = calculator.CalcPoint(p1);
            var to = calculator.CalcPoint(p2);
            return new LineF(from, to);
        }
    }
}
