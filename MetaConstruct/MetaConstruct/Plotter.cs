﻿using MetaArt.Core;
using MetaCore;
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
            if(Contains(entity))
                throw new InvalidOperationException();
            entities.Add(new EntityViewInfo(entity, style));
        }

        public bool Contains(Entity entity) {
            return entities.Any(x => x.Entity == entity);
        }

        public void Remove(Entity entity) {
            entities.Remove(entities.Single(x => x.Entity == entity));
        }
        public void Remove(Point point, bool keepLocation = false) {
            entities.Remove(entities.Single(x => (x.Entity as PointView)?.point == point));
            if(!keepLocation && point is FreePoint freePoint)
                points.Remove(freePoint);
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

        public Point? HitTestIntersection(Vector2 point) {
            var calculator = CreateCalculator();
            var closeEntities = this.entities
                .Where(x => {
                    var distance = float.MaxValue;
                    switch(x.Entity) {
                        case Line l:
                            var lineF = calculator.CalcLine(l);
                            distance = ConstructHelper.DistanceToLine(point, lineF.from, lineF.to);
                            break;
                        case LineSegment s:
                            var lineSegmentF = calculator.CalcLineSegment(s.From, s.To);
                            distance = ConstructHelper.DistanceToLineSegment(point, lineSegmentF.from, lineSegmentF.to);
                            break;
                        case Circle c:
                            var circleF = calculator.CalcCircle(c);
                            distance = ConstructHelper.DistanceToCircle(point, circleF.center, circleF.radius);
                            break;
                        case CircleSegment c:
                            var circleSegmentF = calculator.CalcCircleSegment(c).circle;
                            distance = ConstructHelper.DistanceToCircle(point, circleSegmentF.center, circleSegmentF.radius);
                            break;
                    };
                    return distance < PointHitTestDistance;
                })
                .ToArray();
            var intersections = closeEntities
                .SelectMany((x, i) => closeEntities.Skip(i + 1).Select(y => (x.Entity, y.Entity)))
                .SelectMany(x => {
                    static Either<Line, Circle> ToLineOrCircle(Entity e) => e switch {
                        Line l => l.AsLeft(),
                        LineSegment l => l.Line.AsLeft(),
                        Circle c => c.AsRight(),
                        CircleSegment c => c.Circle.AsRight(),
                    };
                    var p = (ToLineOrCircle(x.Item1), ToLineOrCircle(x.Item2)) switch {
                        ((Line l1, null), (Line l2, null)) => Constructor.Intersect(l1, l2).Yield(),
                        ((Line l, null), (null, Circle c)) => Constructor.Intersect(l, c).Trasform(x => (x.Point1, x.Point2)).Yield(),
                        ((null, Circle c), (Line l, null)) => Constructor.Intersect(l, c).Trasform(x => (x.Point1, x.Point2)).Yield(),
                        ((null, Circle c1), (null, Circle c2)) => Constructor.Intersect(c1, c2).Trasform(x => (x.Point1, x.Point2)).Yield(),
                    };
                    return p;
                })
                .Select(x => (point: x, calculator.CalcPoint(x)))
                .OrderBy(x => Vector2.DistanceSquared(x.Item2, point))
                ;
            return intersections.FirstOrDefault().point;
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

        public static CircleSegmentF CalcCircleSegment(this Calculator calculator, CircleSegment segment) {
            var circle = calculator.CalcCircle(segment.Circle);
            var from = calculator.CalcPoint(segment.From);
            var to = calculator.CalcPoint(segment.To);
            float angleFrom = ConstructHelper.AngleTo(from - circle.center);
            float angleTo = ConstructHelper.AngleTo(to - circle.center);
            if(angleFrom > angleTo)
                angleFrom -= MathF.PI * 2;
            return new CircleSegmentF(circle, angleFrom, angleTo);
        }

        public static LineF CalcLineSegment(this Calculator calculator, Point p1, Point p2) {
            var from = calculator.CalcPoint(p1);
            var to = calculator.CalcPoint(p2);
            return new LineF(from, to);
        }
    }

    public static class SurfaceExtensions {
        public static T Add<T>(this T entity, Surface surface, DisplayStyle? style = null) where T : Entity {
            style = style ?? entity switch {
                Primitive => DisplayStyle.Background,
                Segment or PointView => DisplayStyle.Visible,
                _ => throw new InvalidOperationException()
            };
            surface.Add(entity, style.Value);
            return entity;
        }
    }
}
