using MetaArt.Core;
using MetaCore;
using System.Numerics;

namespace MetaConstruct {
    public record Painter(
        Action<CircleF, DisplayStyle> DrawCircle,
        Action<CircleSegmentF, DisplayStyle> DrawCircleSegment,
        Action<LineF, DisplayStyle> DrawLine,
        Action<LineF, DisplayStyle> DrawLineSegment,
        Action<Either<CircleSegmentF, LineF>[], DisplayStyle> FillContour,
        Action<Vector2, PointKind, DisplayStyle> DrawPoint
    );
    public enum PointKind { Free, Circles, Lines, LineCircle }
    public record struct CircleSegmentF(CircleF circle, float from, float to);

    public record struct EntityViewInfo(Entity Entity, DisplayStyle Style);
    public static class Plotter {
        public static void Draw(Surface surface, Painter painter) {
            var calculator = surface.CreateCalculator();
            foreach(var (primitive, style) in surface.GetEntities()) {
                switch(primitive) {
                    case PrimitiveView p:
                        if(p.primitive is Line l) {
                            var lineF = calculator.CalcLine(l);
                            painter.DrawLine(lineF, style);
                        } else if(p.primitive is Circle c) {
                            var circleF = calculator.CalcCircle(c);
                            painter.DrawCircle(circleF, style);
                        } else
                            throw new InvalidOperationException();
                        break;
                    case LineSegment s:
                        var lineSegmentF = calculator.CalcLineSegment(s.From, s.To);
                        painter.DrawLineSegment(lineSegmentF, style);
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

        static Either<CircleSegmentF, LineF>[] CalcContour(this Calculator calculator, Contour contour) {
            var segments = contour.Segments
                .Select<Segment, Either<CircleSegmentF, LineF>>(s => {
                    return s switch {
                        CircleSegment c => calculator.CalcCircleSegment(c).AsLeft(),
                        LineSegment l => calculator.CalcLine(l.Line).AsRight(),
                    };
                })
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
        public static T Add<T>(this T entity, Surface surface, DisplayStyle style = DisplayStyle.Visible) where T : Entity {
            AddCore(entity, surface, style);
            return entity;
        }
        public static Point Add(this Point point, Surface surface, DisplayStyle style = DisplayStyle.Background) {
            AddCore(point, surface, style);
            return point;
        }
        public static Circle Add(this Circle primitive, Surface surface, DisplayStyle style = DisplayStyle.Background) {
            AddCore(primitive, surface, style);
            return primitive;
        }
        public static Line Add(this Line primitive, Surface surface, DisplayStyle style = DisplayStyle.Background) {
            AddCore(primitive, surface, style);
            return primitive;
        }
        static void AddCore(this Entity entity, Surface surface, DisplayStyle style) {
            surface.Add(entity, style);
        }
    }
}
