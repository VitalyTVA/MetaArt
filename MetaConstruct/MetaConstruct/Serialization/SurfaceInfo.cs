using System.Globalization; 
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaConstruct.Serialization {
    public class SurfaceInfo {
        public static string Serialize(Surface surface) {
            var surfaceInfo = new SurfaceInfo();
            var segments = new Dictionary<Segment, int>();

            var (freePoints, getPointId, getPrimitiveId) = ConstructionInfo.CollectConstruction(surface.GetEntities().Select(x => x.Entity), surfaceInfo.Construction);

            foreach(var (entity, style) in surface.GetEntities()) {
                if(entity is Segment segment) {
                    var id = segments.Count;
                    segments.Add(segment, segments.Count);
                    if(segment is LineSegment lineSegment) {
                        surfaceInfo.LineSegments.Add(new LineSegmentInfo {
                            Id = id,
                            Line = getPrimitiveId(lineSegment.Line),
                            From = getPointId(lineSegment.From),
                            To = getPointId(lineSegment.To),
                        });
                    } else if(segment is CircleSegment circleSegment) {
                        surfaceInfo.CircleSegments.Add(new CircleSegmentInfo {
                            Id = id,
                            Circle = getPrimitiveId(circleSegment.Circle),
                            From = getPointId(circleSegment.From),
                            To = getPointId(circleSegment.To),
                        });
                    } else {
                        throw new InvalidOperationException();
                    }
                }
            }
            foreach(var freePoint in freePoints) {
                surfaceInfo.PointLocations.Add(new FreePointLocationInfo {
                    Point = getPointId(freePoint),
                    Location = surface.GetPointLocation(freePoint)
                });
            }
            foreach(var (entity, style) in surface.GetEntities()) {
                var index = entity switch {
                    PointView p => getPointId(p.point),
                    Primitive p => getPrimitiveId(p),
                    Segment s => segments[s],
                };
                var kind = entity switch {
                    PointView => ViewKind.Point,
                    Circle => ViewKind.Circle,
                    Line => ViewKind.Line,
                    LineSegment => ViewKind.LineSegment,
                    CircleSegment => ViewKind.CircleSegment,
                };
                surfaceInfo.Views.Add(new ViewInfo {
                    Id = index,
                    ViewKind = kind,
                    DisplayStyle = style
                });
            }
            var jsonString = JsonSerializer.Serialize(surfaceInfo, SourceGenerationContext.Default.SurfaceInfo);
            return jsonString;
        }



        public static void Deserialize(Surface surface, string jsonString) {
            var info = JsonSerializer.Deserialize(jsonString, SourceGenerationContext.Default.SurfaceInfo)!;
            var lineSegments = info.LineSegments.ToDictionary(x => x.Id);
            var circleSegments = info.CircleSegments.ToDictionary(x => x.Id);
            var (getPoint, getLine, getCircle) = ConstructionInfo.Construct(surface.Constructor, info.Construction);
            foreach(var item in info.Views) {
                if(item.ViewKind == ViewKind.Point) {
                    var point = getPoint(item.Id);
                    surface.Add(point.AsView(), item.DisplayStyle);
                } else if(item.ViewKind == ViewKind.Line) {
                    surface.Add(getLine(item.Id), item.DisplayStyle);
                } else if(item.ViewKind == ViewKind.Circle) {
                    surface.Add(getCircle(item.Id), item.DisplayStyle);
                } else if(item.ViewKind == ViewKind.LineSegment) {
                    var segmentInfo = lineSegments[item.Id];
                    var line = getLine(segmentInfo.Line);
                    var from = getPoint(segmentInfo.From);
                    var to = getPoint(segmentInfo.To);
                    surface.Add(ConstructorHelper.LineSegment(line, from, to), item.DisplayStyle);
                } else if(item.ViewKind == ViewKind.CircleSegment) {
                    var segmentInfo = circleSegments[item.Id];
                    var circle = getCircle(segmentInfo.Circle);
                    var from = getPoint(segmentInfo.From);
                    var to = getPoint(segmentInfo.To);
                    surface.Add(ConstructorHelper.CircleSegment(circle, from, to), item.DisplayStyle);
                } else {
                    throw new InvalidOperationException();
                }
            }
            foreach(var item in info.PointLocations) {
                var point = (FreePoint)getPoint(item.Point);
                surface.SetPointLocation(point, item.Location);
            }
        }

        public List<FreePointLocationInfo> PointLocations { get; set; } = new();
        public List<LineSegmentInfo> LineSegments { get; set; } = new();
        public List<CircleSegmentInfo> CircleSegments { get; set; } = new();
        public List<ViewInfo> Views { get; set; } = new();
        public ConstructionInfo Construction { get; set; } = new ();

        public class FreePointLocationInfo {
            public int Point { get; set; }
            [JsonConverter(typeof(Vector2JsonConverter))]
            public Vector2 Location { get; set; }
        }

        public class LineSegmentInfo {
            public int Id { get; set; }
            public int Line { get; set; }
            public int From { get; set; }
            public int To { get; set; }
        }
        public class CircleSegmentInfo {
            public int Id { get; set; }
            public int Circle { get; set; }
            public int From { get; set; }
            public int To { get; set; }
        }
        public enum ViewKind { Point, Line, Circle, LineSegment, CircleSegment }
        public class ViewInfo {
            public int Id { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public ViewKind ViewKind { get; set; }
            public DisplayStyle DisplayStyle { get; set; }
        }
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


    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(SurfaceInfo))]
    internal partial class SourceGenerationContext : JsonSerializerContext {
    }
}
