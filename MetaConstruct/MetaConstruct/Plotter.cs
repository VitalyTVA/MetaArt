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

    public record struct EntityViewInfo(Entity Entity, DisplayStyle Style);
    public class Surface {
        public readonly int PointHitTestDistance;
        public Surface(Constructor constructor, int pointHitTestDistance) {
            PointHitTestDistance = pointHitTestDistance;
            Constructor = constructor;
        }

        public Constructor Constructor { get; }
        readonly List<EntityViewInfo> entities = new();
        public void Add(Entity entity, DisplayStyle style) {
            if(entities.Any(x => x.Entity == entity))
                throw new InvalidOperationException();
            entities.Add(new EntityViewInfo(entity, style));
        }

        public void Remove(FreePoint point) {
            entities.Remove(entities.Single(x => (x.Entity as PointView)?.point == point));
            points.Remove(point);
        }
        public void Remove(Line line) {
            entities.Remove(entities.Single(x => x.Entity == line));
        }

        public IEnumerable<EntityViewInfo> GetEntities() => entities;
        Dictionary<FreePoint, Vector2> points = new();

        public void SetPoints((FreePoint, Vector2)[] points) {
            foreach(var (point, location) in points) {
                this.points.Add(point, location);
            }
        }

        public Vector2 GetPointLocation(FreePoint p) => points[p];

        public IEnumerable<Point> HitTest(Vector2 point) {
            var calculator = CreateCalculator();
            return entities
                .Where(x => x.Entity is PointView)
                .Select(x => (point: ((PointView)x.Entity).point, location: calculator.CalcPoint(((PointView)x.Entity).point)))
                .OrderBy(x => Vector2.DistanceSquared(x.location, point))
                .Where(x => Vector2.DistanceSquared(x.location, point) < PointHitTestDistance * PointHitTestDistance)
                .Select(x => x.point);
        }

        public Calculator CreateCalculator() {
            return new Calculator(GetPointLocation);
        }

        public void SetPointLocation(FreePoint point, Vector2 location) { 
            points[point] = location;
        }
    }

    public static class Plotter {
        public static void Draw(Surface surface, Painter painter) {
            var calculator = surface.CreateCalculator();
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
