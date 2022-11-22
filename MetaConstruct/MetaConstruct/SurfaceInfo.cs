using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaConstruct.Serialization {
    public class SurfaceInfo {
        public static string Serialize(Surface surface) {
            var surfaceInfo = new SurfaceInfo();
            var index = 0;
            foreach(var (entity, style) in surface.GetEntities()) {
                var point = (FreePoint)((PointView)entity).point;
                surfaceInfo.FreePoints.Add(new FreePointInfo {
                    Index = index,
                    Location = surface.GetPointLocation(point)
                });
                surfaceInfo.Views.Add(new ViewInfo {
                    Index = index,
                    DisplayStyle = style
                });
                index++;
            }
            var jsonString = JsonSerializer.Serialize(surfaceInfo, SourceGenerationContext.Default.SurfaceInfo);
            return jsonString;
        }
        public static void Deserialize(Surface surface, string jsonString) {
            var info = JsonSerializer.Deserialize(jsonString, SourceGenerationContext.Default.SurfaceInfo)!;
            var points = info.FreePoints.ToDictionary(x => x.Index);
            foreach(var item in info.Views) {
                var point = surface.Constructor.Point();
                surface.Add(point.AsView(), item.DisplayStyle);
                surface.SetPointLocation(point, points[item.Index].Location);
            }
        }
        public List<FreePointInfo> FreePoints { get; set; } = new();
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
    public class ViewInfo {
        public int Index { get; set; }
        public DisplayStyle DisplayStyle { get; set; }
    }
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(SurfaceInfo))]
    internal partial class SourceGenerationContext : JsonSerializerContext {
    }
}
