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
                var point = (FreePoint)pair.Key;
                surfaceInfo.FreePoints.Add(new FreePointInfo {
                    Index = pair.Value,
                    Location = surface.GetPointLocation(point)
                });
            }
            foreach(var pair in primitives) {
                var line = (Line)pair.Key;
                surfaceInfo.Lines.Add(new LineInfo {
                    Index = pair.Value,
                    From = points[line.From],
                    To = points[line.To],
                });
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
            switch(primitive) {
                case Line line:
                    CollectPoints(line.From, points, primitives);
                    CollectPoints(line.To, points, primitives);
                    primitives.Add(line, points.Count + primitives.Count);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        static void CollectPoints(Point point, Dictionary<Point, int> points, Dictionary<Primitive, int> primitives) {
            switch(point) {
                case FreePoint freePoint:
                    points.Add(point, points.Count + primitives.Count);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        public static void Deserialize(Surface surface, string jsonString) {
            var info = JsonSerializer.Deserialize(jsonString, SourceGenerationContext.Default.SurfaceInfo)!;
            var points = info.FreePoints.ToDictionary(x => x.Index);
            var lines = info.Lines.ToDictionary(x => x.Index);
            var createdPoints = new Dictionary<int, Point>();
            var createdPrimitives = new Dictionary<int, Primitive>();
            Point GetPoint(int index) { 
                if(createdPoints.TryGetValue(index, out Point point))
                    return point;
                var newPoint = surface.Constructor.Point();
                surface.SetPointLocation(newPoint, points[index].Location);
                return createdPoints[index] = newPoint;
            }
            foreach(var item in info.Views) {
                if(points.ContainsKey(item.Index)) {
                    var point = (FreePoint)GetPoint(item.Index);
                    surface.Add(point.AsView(), item.DisplayStyle);
                    createdPoints[item.Index] = point;
                } else if(lines.ContainsKey(item.Index)) {
                    var lineInfo = lines[item.Index];
                    var from = GetPoint(lineInfo.From);
                    var to = GetPoint(lineInfo.To);
                    var line = surface.Constructor.Line(from, to);
                    surface.Add(line, item.DisplayStyle);
                } else {
                    throw new InvalidOperationException();
                }
            }
        }
        public List<FreePointInfo> FreePoints { get; set; } = new();
        public List<LineInfo> Lines { get; set; } = new();
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
    public class LineInfo {
        public int From { get; set; }
        public int To { get; set; }
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
