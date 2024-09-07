using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaConstruct.Serialization {
    public class ConstructionInfo {
        public static (IEnumerable<FreePoint> freePoints, Func<Point, int> getPointId, Func<Primitive, int> getPrimitiveId)
            CollectConstruction(IEnumerable<Entity> entities, ConstructionInfo constructionInfo) {
            var points = new Dictionary<Point, int>();
            var primitives = new Dictionary<Primitive, int>();
            void CollectPrimitives(Primitive primitive) {
                if(primitives.ContainsKey(primitive))
                    return;
                switch(primitive) {
                    case Line line:
                        CollectPoints(line.From);
                        CollectPoints(line.To);
                        break;
                    case Circle circle:
                        CollectPoints(circle.Center);
                        CollectPoints(circle.Radius1);
                        CollectPoints(circle.Radius2);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                primitives.Add(primitive, primitives.Count);
            }
            void CollectPoints(Point point) {
                if(points.ContainsKey(point))
                    return;

                switch(point) {
                    case FreePoint:
                        break;
                    case LineLinePoint p:
                        CollectPrimitives(p.Line1);
                        CollectPrimitives(p.Line2);
                        break;
                    case LineCirclePoint p:
                        CollectPrimitives(p.Line);
                        CollectPrimitives(p.Circle);
                        break;
                    case CircleCirclePoint p:
                        CollectPrimitives(p.Circle1);
                        CollectPrimitives(p.Circle2);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                points.Add(point, points.Count);
            }
            bool CollectSimpleSegment(Segment segment) {
                if(segment is LineSegment lineSegment) {
                    CollectPrimitives(lineSegment.Line);
                    CollectPoints(lineSegment.From);
                    CollectPoints(lineSegment.To);
                    return true;
                } else if(segment is CircleSegment circleSegment) {
                    CollectPrimitives(circleSegment.Circle);
                    CollectPoints(circleSegment.From);
                    CollectPoints(circleSegment.To);
                    return true;
                }
                return false;
            }
            foreach(var entity in entities) {
                if(entity is PointView pointView)
                    CollectPoints(pointView.point);
                else if(entity is PrimitiveView primitiveView)
                    CollectPrimitives(primitiveView.primitive);
                else if(entity is Segment segment) {
                    if(!CollectSimpleSegment(segment) && segment is Contour contour) {
                        foreach(var subSegment in contour.Segments) {
                            CollectSimpleSegment(subSegment);
                        }
                    }
                } else
                    throw new InvalidOperationException();
            }
            foreach(var pair in points) {
                if(pair.Key is FreePoint point) {
                    constructionInfo.FreePoints.Add(new FreePointInfo {
                        Id = pair.Value,
                    });
                } else if(pair.Key is LineLinePoint lineLinePoint) {
                    constructionInfo.LineLinePoints.Add(new LineLinePointInfo {
                        Id = pair.Value,
                        Line1 = primitives[lineLinePoint.Line1],
                        Line2 = primitives[lineLinePoint.Line2],
                    });
                } else if(pair.Key is LineCirclePoint lineCirclePoint) {
                    constructionInfo.LineCirclePoints.Add(new LineCirclePointInfo {
                        Id = pair.Value,
                        Line = primitives[lineCirclePoint.Line],
                        Circle = primitives[lineCirclePoint.Circle],
                        Intersection = lineCirclePoint.Intersection
                    });
                } else if(pair.Key is CircleCirclePoint circleCirclePoint) {
                    constructionInfo.CircleCirclePoints.Add(new CircleCirclePointInfo {
                        Id = pair.Value,
                        Circle1 = primitives[circleCirclePoint.Circle1],
                        Circle2 = primitives[circleCirclePoint.Circle2],
                        Intersection = circleCirclePoint.Intersection
                    });
                } else {
                    throw new InvalidOperationException();
                }
            }
            foreach(var pair in primitives) {
                if(pair.Key is Line line) {
                    constructionInfo.Lines.Add(new LineInfo {
                        Id = pair.Value,
                        From = points[line.From],
                        To = points[line.To],
                    });
                } else if(pair.Key is Circle circle) {
                    constructionInfo.Circles.Add(new CircleInfo {
                        Id = pair.Value,
                        Center = points[circle.Center],
                        Radius1 = points[circle.Radius1],
                        Radius2 = points[circle.Radius2],
                    });
                } else {
                    throw new InvalidOperationException();
                }
            }
            return (
                points.Keys.OfType<FreePoint>(), 
                p => points[p], 
                p => primitives[p]
            );
        }
        public static (Func<int, Point> getPoint, Func<int, Line> getLine, Func<int, Circle> getCircle)
            Construct(Constructor constructor, ConstructionInfo construction) {
            var freePoints = construction.FreePoints.ToDictionary(x => x.Id);
            var lineLinePoints = construction.LineLinePoints.ToDictionary(x => x.Id);
            var lineCirclePoints = construction.LineCirclePoints.ToDictionary(x => x.Id);
            var circleCirclePoints = construction.CircleCirclePoints.ToDictionary(x => x.Id);
            var lines = construction.Lines.ToDictionary(x => x.Id);
            var circles = construction.Circles.ToDictionary(x => x.Id);
            var createdPoints = new Dictionary<int, Point>();
            var createdPrimitives = new Dictionary<int, Primitive>();
            Point GetPoint(int index) {
                if(createdPoints.TryGetValue(index, out Point point))
                    return point;
                if(freePoints.ContainsKey(index)) {
                    var newPoint = constructor.Point();
                    return createdPoints[index] = newPoint;
                } else if(lineLinePoints.ContainsKey(index)) {
                    var info = lineLinePoints[index];
                    var line1 = GetLine(info.Line1);
                    var line2 = GetLine(info.Line2);
                    var newPoint = constructor.Intersect(line1, line2);
                    return createdPoints[index] = newPoint;
                } else if(lineCirclePoints.ContainsKey(index)) {
                    var info = lineCirclePoints[index];
                    var line = GetLine(info.Line);
                    var circle = GetCircle(info.Circle);
                    var intersection = constructor.Intersect(line, circle);
                    return createdPoints[index] = info.Intersection switch {
                        CircleIntersectionKind.First => intersection.Point1,
                        CircleIntersectionKind.Second or CircleIntersectionKind.Secondary => intersection.Point2,
                    };
                } else if(circleCirclePoints.ContainsKey(index)) {
                    var info = circleCirclePoints[index];
                    var circle1 = GetCircle(info.Circle1);
                    var circle2 = GetCircle(info.Circle2);
                    var intersection = constructor.Intersect(circle1, circle2);
                    return createdPoints[index] = info.Intersection switch {
                        CircleIntersectionKind.First => intersection.Point1,
                        CircleIntersectionKind.Second or CircleIntersectionKind.Secondary => intersection.Point2,
                    };
                } else {
                    throw new InvalidOperationException();
                }
            }
            Line GetLine(int index) {
                if(createdPrimitives.TryGetValue(index, out Primitive primitive))
                    return (Line)primitive;
                var lineInfo = lines[index];
                var from = GetPoint(lineInfo.From);
                var to = GetPoint(lineInfo.To);
                var line = constructor.Line(from, to);
                createdPrimitives[index] = line;
                return line;
            }
            Circle GetCircle(int index) {
                if(createdPrimitives.TryGetValue(index, out Primitive primitive))
                    return (Circle)primitive;
                var lineCircleInfo = circles[index];
                var center = GetPoint(lineCircleInfo.Center);
                var radius1 = GetPoint(lineCircleInfo.Radius1);
                var radius2 = GetPoint(lineCircleInfo.Radius2);
                var circle = constructor.Circle(center, radius1, radius2);
                createdPrimitives[index] = circle;
                return circle;
            }
            return (GetPoint, GetLine, GetCircle);
        }
        public List<FreePointInfo> FreePoints { get; set; } = new();
        public List<LineLinePointInfo> LineLinePoints { get; set; } = new();
        public List<LineCirclePointInfo> LineCirclePoints { get; set; } = new();
        public List<CircleCirclePointInfo> CircleCirclePoints { get; set; } = new();
        public List<LineInfo> Lines { get; set; } = new();
        public List<CircleInfo> Circles { get; set; } = new();

        public class FreePointInfo {
            public int Id { get; set; }
        }
        public class LineLinePointInfo {
            public int Id { get; set; }
            public int Line1 { get; set; }
            public int Line2 { get; set; }
        }
        public class LineCirclePointInfo {
            public int Id { get; set; }
            public int Line { get; set; }
            public int Circle { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter<CircleIntersectionKind>))]
            public CircleIntersectionKind Intersection { get; set; }
        }
        public class CircleCirclePointInfo {
            public int Id { get; set; }
            public int Circle1 { get; set; }
            public int Circle2 { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter<CircleIntersectionKind>))]
            public CircleIntersectionKind Intersection { get; set; }
        }
        public class LineInfo {
            public int Id { get; set; }
            public int From { get; set; }
            public int To { get; set; }
        }
        public class CircleInfo {
            public int Id { get; set; }
            public int Center { get; set; }
            public int Radius1 { get; set; }
            public int Radius2 { get; set; }
        }
    }
}
