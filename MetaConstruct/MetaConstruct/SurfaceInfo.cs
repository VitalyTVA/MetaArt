using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaConstruct.Serialization {
    public class SurfaceInfo {
        public static string Serialize(Surface surface) {
            var surfaceInfo = new SurfaceInfo();
            var points = new Dictionary<Point, int>();
            var primitives = new Dictionary<Primitive, int>();
            foreach(var (entity, style) in surface.GetEntities()) {
                if(entity is PointView pointView)
                    CollectPoints(pointView.point, points, primitives);
                else if(entity is Primitive primitive)
                    CollectPrimitives(primitive, points, primitives);
                else
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

                } else { 
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
            foreach(var (entity, style) in surface.GetEntities()) {
                var index = entity switch {
                    PointView p => points[p.point],
                    Primitive p => primitives[p],
                };
                surfaceInfo.Views.Add(new ViewInfo {
                    Index = index,
                    DisplayStyle = style
                });
            }
            var jsonString = JsonSerializer.Serialize(surfaceInfo, SourceGenerationContext.Default.SurfaceInfo);
            return jsonString;
        }
        static void CollectPrimitives(Primitive primitive, Dictionary<Point, int> points, Dictionary<Primitive, int> primitives) {
            if(primitives.ContainsKey(primitive))
                return;
            switch(primitive) {
                case Line line:
                    CollectPoints(line.From, points, primitives);
                    CollectPoints(line.To, points, primitives);
                    primitives.Add(line, points.Count + primitives.Count);
                    break;
                case Circle circle:
                    CollectPoints(circle.Center, points, primitives);
                    CollectPoints(circle.Radius1, points, primitives);
                    CollectPoints(circle.Radius2, points, primitives);
                    primitives.Add(circle, points.Count + primitives.Count);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        static void CollectPoints(Point point, Dictionary<Point, int> points, Dictionary<Primitive, int> primitives) {
            if(points.ContainsKey(point))
                return;

            switch(point) {
                case FreePoint:
                    break;
                case LineLinePoint p:
                    CollectPrimitives(p.Line1, points, primitives);
                    CollectPrimitives(p.Line2, points, primitives);
                    break;
                default:
                    throw new NotImplementedException();
            }
            points.Add(point, points.Count + primitives.Count);
        }
        public static void Deserialize(Surface surface, string jsonString) {
            var info = JsonSerializer.Deserialize(jsonString, SourceGenerationContext.Default.SurfaceInfo)!;
            var freePoints = info.FreePoints.ToDictionary(x => x.Index);
            var lineLinePoints = info.LineLinePoints.ToDictionary(x => x.Index);
            var lines = info.Lines.ToDictionary(x => x.Index);
            var circles = info.Circles.ToDictionary(x => x.Index);
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
                if(freePoints.ContainsKey(item.Index) || lineLinePoints.ContainsKey(item.Index)) {
                    var point = GetPoint(item.Index);
                    surface.Add(point.AsView(), item.DisplayStyle);
                } else if(lines.ContainsKey(item.Index)) {
                    surface.Add(GetLine(item.Index), item.DisplayStyle);
                } else if(circles.ContainsKey(item.Index)) {
                    surface.Add(GetCircle(item.Index), item.DisplayStyle);
                } else {
                    throw new InvalidOperationException();
                }
            }
        }
        public List<FreePointInfo> FreePoints { get; set; } = new();
        public List<LineLinePointInfo> LineLinePoints { get; set; } = new();
        public List<LineInfo> Lines { get; set; } = new();
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
    public class LineInfo {
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
