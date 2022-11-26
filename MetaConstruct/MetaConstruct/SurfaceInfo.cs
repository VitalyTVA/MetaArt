using System.Globalization; 
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaConstruct.Serialization {
    public class SurfaceInfo {
        public static string Serialize(Surface surface) {
            var surfaceInfo = new SurfaceInfo();
            var points = new Dictionary<Point, int>();
            var primitives = new Dictionary<Primitive, int>();
            var segments = new Dictionary<Segment, int>();
            int GetCount() => points.Count + primitives.Count + segments.Count;
            void CollectPrimitives(Primitive primitive) {
                if(primitives.ContainsKey(primitive))
                    return;
                switch(primitive) {
                    case Line line:
                        CollectPoints(line.From);
                        CollectPoints(line.To);
                        primitives.Add(line, GetCount());
                        break;
                    case Circle circle:
                        CollectPoints(circle.Center);
                        CollectPoints(circle.Radius1);
                        CollectPoints(circle.Radius2);
                        primitives.Add(circle, GetCount());
                        break;
                    default:
                        throw new NotImplementedException();
                }
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
                points.Add(point, GetCount());
            }

            foreach(var (entity, style) in surface.GetEntities()) {
                if(entity is PointView pointView)
                    CollectPoints(pointView.point);
                else if(entity is Primitive primitive)
                    CollectPrimitives(primitive);
                else if(entity is Segment segment) {
                    segments.Add(segment, GetCount());
                    if(segment is LineSegment lineSegment) {
                        CollectPrimitives(lineSegment.Line);
                        CollectPoints(lineSegment.From);
                        CollectPoints(lineSegment.To);
                    }
                } else
                    throw new InvalidOperationException();
            }
            foreach(var pair in points) {
                if(pair.Key is FreePoint point) {
                    surfaceInfo.FreePoints.Add(new FreePointInfo {
                        Index = pair.Value,
                        Location = surface.GetPointLocation(point)
                    });
                } else if(pair.Key is LineLinePoint lineLinePoint) {
                    surfaceInfo.LineLinePoints.Add(new LineLinePointInfo {
                        Index = pair.Value,
                        Line1 = primitives[lineLinePoint.Line1],
                        Line2 = primitives[lineLinePoint.Line2],
                    });
                } else if(pair.Key is LineCirclePoint lineCirclePoint) {
                    surfaceInfo.LineCirclePoints.Add(new LineCirclePointInfo {
                        Index = pair.Value,
                        Line = primitives[lineCirclePoint.Line],
                        Circle = primitives[lineCirclePoint.Circle],
                        Intersection = lineCirclePoint.Intersection
                    });
                } else if(pair.Key is CircleCirclePoint circleCirclePoint) {
                    surfaceInfo.CircleCirclePoints.Add(new CircleCirclePointInfo {
                        Index = pair.Value,
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
                    surfaceInfo.Lines.Add(new LineInfo {
                        Index = pair.Value,
                        From = points[line.From],
                        To = points[line.To],
                    });
                } else if(pair.Key is Circle circle) {
                    surfaceInfo.Circles.Add(new CircleInfo {
                        Index = pair.Value,
                        Center = points[circle.Center],
                        Radius1 = points[circle.Radius1],
                        Radius2 = points[circle.Radius2],
                    });
                } else { 
                    throw new InvalidOperationException();
                }
            }
            foreach(var pair in segments) {
                if(pair.Key is LineSegment lineSegment) {
                    surfaceInfo.LineSegments.Add(new LineSegmentInfo {
                        Index = pair.Value,
                        Line = primitives[lineSegment.Line],
                        From = points[lineSegment.From],
                        To = points[lineSegment.To],
                    });
                } else {
                    throw new InvalidOperationException();
                }
            }
            foreach(var (entity, style) in surface.GetEntities()) {
                var index = entity switch {
                    PointView p => points[p.point],
                    Primitive p => primitives[p],
                    Segment s => segments[s],
                };
                surfaceInfo.Views.Add(new ViewInfo {
                    Index = index,
                    DisplayStyle = style
                });
            }
            var jsonString = JsonSerializer.Serialize(surfaceInfo, SourceGenerationContext.Default.SurfaceInfo);
            return jsonString;
        }

        public static void Deserialize(Surface surface, string jsonString) {
            var info = JsonSerializer.Deserialize(jsonString, SourceGenerationContext.Default.SurfaceInfo)!;
            var freePoints = info.FreePoints.ToDictionary(x => x.Index);
            var lineLinePoints = info.LineLinePoints.ToDictionary(x => x.Index);
            var lineCirclePoints = info.LineCirclePoints.ToDictionary(x => x.Index);
            var circleCirclePoints = info.CircleCirclePoints.ToDictionary(x => x.Index);
            var lines = info.Lines.ToDictionary(x => x.Index);
            var circles = info.Circles.ToDictionary(x => x.Index);
            var lineSegments = info.LineSegments.ToDictionary(x => x.Index);
            var createdPoints = new Dictionary<int, Point>();
            var createdPrimitives = new Dictionary<int, Primitive>();
            Point GetPoint(int index) { 
                if(createdPoints.TryGetValue(index, out Point point))
                    return point;
                if(freePoints.ContainsKey(index)) {
                    var newPoint = surface.Constructor.Point();
                    surface.SetPointLocation(newPoint, freePoints[index].Location);
                    return createdPoints[index] = newPoint;
                } else if(lineLinePoints.ContainsKey(index)) {
                    var info = lineLinePoints[index];
                    var line1 = GetLine(info.Line1);
                    var line2 = GetLine(info.Line2);
                    var newPoint = surface.Constructor.Intersect(line1, line2);
                    return createdPoints[index] = newPoint;
                } else if(lineCirclePoints.ContainsKey(index)) {
                    var info = lineCirclePoints[index];
                    var line = GetLine(info.Line);
                    var circle = GetCircle(info.Circle);
                    var intersection = surface.Constructor.Intersect(line, circle);
                    return createdPoints[index] = info.Intersection switch { 
                        CircleIntersectionKind.First => intersection.Point1,
                        CircleIntersectionKind.Second or CircleIntersectionKind.Secondary => intersection.Point2,
                    };
                } else if(circleCirclePoints.ContainsKey(index)) {
                    var info = circleCirclePoints[index];
                    var circle1 = GetCircle(info.Circle1);
                    var circle2 = GetCircle(info.Circle2);
                    var intersection = surface.Constructor.Intersect(circle1, circle2);
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
                var line = surface.Constructor.Line(from, to);
                createdPrimitives[index] = line;
                return line;
            }
            Circle GetCircle(int index) {
                if(createdPrimitives.TryGetValue(index, out Primitive primitive))
                    return (Circle)primitive;
                var lineCircleInfo= circles[index];
                var center = GetPoint(lineCircleInfo.Center);
                var radius1 = GetPoint(lineCircleInfo.Radius1);
                var radius2 = GetPoint(lineCircleInfo.Radius2);
                var circle = surface.Constructor.Circle(center, radius1, radius2);
                createdPrimitives[index] = circle;
                return circle;
            }
            foreach(var item in info.Views) {
                if(freePoints.ContainsKey(item.Index) || 
                    lineLinePoints.ContainsKey(item.Index) || 
                    lineCirclePoints.ContainsKey(item.Index) ||
                    circleCirclePoints.ContainsKey(item.Index)
                ) {
                    var point = GetPoint(item.Index);
                    surface.Add(point.AsView(), item.DisplayStyle);
                } else if(lines.ContainsKey(item.Index)) {
                    surface.Add(GetLine(item.Index), item.DisplayStyle);
                } else if(circles.ContainsKey(item.Index)) {
                    surface.Add(GetCircle(item.Index), item.DisplayStyle);
                } else if(lineSegments.TryGetValue(item.Index, out LineSegmentInfo lineSegmentInfo)) {
                    var line = GetLine(lineSegmentInfo.Line);
                    var from = GetPoint(lineSegmentInfo.From);
                    var to = GetPoint(lineSegmentInfo.To);
                    surface.Add(ConstructorHelper.LineSegment(line, from, to), item.DisplayStyle);
                } else {
                    throw new InvalidOperationException();
                }
            }
        }
        public List<FreePointInfo> FreePoints { get; set; } = new();
        public List<LineLinePointInfo> LineLinePoints { get; set; } = new();
        public List<LineCirclePointInfo> LineCirclePoints { get; set; } = new();
        public List<CircleCirclePointInfo> CircleCirclePoints { get; set; } = new();
        public List<LineInfo> Lines { get; set; } = new();
        public List<LineSegmentInfo> LineSegments { get; set; } = new();
        public List<CircleInfo> Circles { get; set; } = new();
        public List<ViewInfo> Views { get; set; } = new();
    }
    public class Vector2JsonConverter : JsonConverter<Vector2> {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var str = reader.GetString()!.Split(',');
            return new Vector2(float.Parse(str[0], CultureInfo.InvariantCulture), float.Parse(str[1], CultureInfo.InvariantCulture));
        }

        public override void Write(Utf8JsonWriter writer, Vector2 vector, JsonSerializerOptions options) {
            writer.WriteStringValue(vector.X.ToString("e", CultureInfo.InvariantCulture) + "," + vector.Y.ToString("e", CultureInfo.InvariantCulture));
        }
    }
    public class FreePointInfo {
        public int Index { get; set; }
        [JsonConverter(typeof(Vector2JsonConverter))]
        public Vector2 Location { get; set; }
    }
    public class LineLinePointInfo {
        public int Index { get; set; }
        public int Line1 { get; set; }
        public int Line2 { get; set; }
    }
    public class LineCirclePointInfo {
        public int Index { get; set; }
        public int Line { get; set; }
        public int Circle { get; set; }
        public CircleIntersectionKind Intersection { get; set; }
    }
    public class CircleCirclePointInfo {
        public int Index { get; set; }
        public int Circle1 { get; set; }
        public int Circle2 { get; set; }
        public CircleIntersectionKind Intersection { get; set; }
    }
    public class LineInfo {
        public int From { get; set; }
        public int To { get; set; }
        public int Index { get; set; }
    }
    public class LineSegmentInfo {
        public int Line { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int Index { get; set; }
    }
    public class CircleInfo {
        public int Center { get; set; }
        public int Radius1 { get; set; }
        public int Radius2 { get; set; }
        public int Index { get; set; }
    }
    public class ViewInfo {
        public int Index { get; set; }
        public DisplayStyle DisplayStyle { get; set; }
    }
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(SurfaceInfo))]
    internal partial class SourceGenerationContext : JsonSerializerContext {
    }
}
