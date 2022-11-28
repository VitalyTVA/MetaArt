using System.Globalization; 
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaConstruct.Serialization {
    public class SurfaceInfo {
        public static string Serialize(Surface surface) {
            var surfaceInfo = new SurfaceInfo();
            var segments = new Dictionary<Segment, int>();

            var (freePoints, getPointId, getPrimitiveId) = ConstructionInfo.CollectConstruction(surface.GetEntities().Select(x => x.Entity), surfaceInfo.Construction);


            int StoreSegment(Segment segment) {
                if(segments.TryGetValue(segment, out var id))
                    return id;
                   
                id = segments.Count;
                segments.Add(segment, id);
                if(segment is LineSegment lineSegment) {
                    surfaceInfo.Segments.Add(new SegmentInfo {
                        Id = id,
                        SegmentKind = SimpleSegmentKind.Line,
                        Primitive = getPrimitiveId(lineSegment.Line),
                        From = getPointId(lineSegment.From),
                        To = getPointId(lineSegment.To),
                    });
                } else if(segment is CircleSegment circleSegment) {
                    surfaceInfo.Segments.Add(new SegmentInfo {
                        Id = id,
                        SegmentKind = SimpleSegmentKind.Circle,
                        Primitive = getPrimitiveId(circleSegment.Circle),
                        From = getPointId(circleSegment.From),
                        To = getPointId(circleSegment.To),
                    });
                } else if(segment is Contour contour) {
                    surfaceInfo.Contours.Add(new ContourInfo {
                        Segments = contour.Segments
                            .Select(subSegment => StoreSegment(subSegment))
                            .ToArray(),
                        Id = id,
                    });
                } else
                    throw new InvalidOperationException();
                return id;
            }

            foreach(var (entity, style) in surface.GetEntities()) {
                if(entity is Segment segment) {
                    StoreSegment(segment);
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
                    PrimitiveView p => getPrimitiveId(p.primitive),
                    Segment s => segments[s],
                };
                var kind = entity switch {
                    PointView => ViewKind.Point,
                    PrimitiveView p => p.primitive switch {
                        Circle => ViewKind.Circle,
                        Line => ViewKind.Line,
                    },
                    LineSegment => ViewKind.LineSegment,
                    CircleSegment => ViewKind.CircleSegment,
                    Contour => ViewKind.Contour,
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
            var segments = info.Segments.ToDictionary(x => x.Id);
            var contours = info.Contours.ToDictionary(x => x.Id);
            var (getPoint, getLine, getCircle) = ConstructionInfo.Construct(surface.Constructor, info.Construction);
            Segment CreateSegment(int id) {
                var segmentInfo = segments[id];
                var from = getPoint(segmentInfo.From);
                var to = getPoint(segmentInfo.To);
                return segmentInfo.SegmentKind switch { 
                    SimpleSegmentKind.Circle => ConstructorHelper.CircleSegment(getCircle(segmentInfo.Primitive), from, to),
                    SimpleSegmentKind.Line => ConstructorHelper.LineSegment(getLine(segmentInfo.Primitive), from, to),
                };
            }
            foreach(var item in info.Views) {
                if(item.ViewKind == ViewKind.Point) {
                    var point = getPoint(item.Id);
                    surface.Add(point, item.DisplayStyle);
                } else if(item.ViewKind == ViewKind.Line) {
                    surface.Add(getLine(item.Id), item.DisplayStyle);
                } else if(item.ViewKind == ViewKind.Circle) {
                    surface.Add(getCircle(item.Id), item.DisplayStyle);
                } else if(item.ViewKind is ViewKind.CircleSegment or ViewKind.LineSegment) {
                    surface.Add(CreateSegment(item.Id), item.DisplayStyle);
                } else if(item.ViewKind == ViewKind.Contour) {
                    var segmentInfo = contours[item.Id];
                    var subSegments = segmentInfo.Segments
                        .Select(segmentId => CreateSegment(segmentId))
                        .ToArray();
                    surface.Add(new Contour(subSegments), item.DisplayStyle);
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
        public List<SegmentInfo> Segments { get; set; } = new();
        public List<ContourInfo> Contours { get; set; } = new();
        public List<ViewInfo> Views { get; set; } = new();
        public ConstructionInfo Construction { get; set; } = new ();

        public class FreePointLocationInfo {
            public int Point { get; set; }
            [JsonConverter(typeof(Vector2JsonConverter))]
            public Vector2 Location { get; set; }
        }
        public enum SimpleSegmentKind { Line, Circle }
        public class SegmentInfo {
            public int Id { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public SimpleSegmentKind SegmentKind { get; set; }
            public int Primitive { get; set; }
            public int From { get; set; }
            public int To { get; set; }
        }
        public class ContourInfo {
            public int Id { get; set; }
            public int[] Segments { get; set; } = null!;
        }
        public enum ViewKind { Point, Line, Circle, LineSegment, CircleSegment, Contour }
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
