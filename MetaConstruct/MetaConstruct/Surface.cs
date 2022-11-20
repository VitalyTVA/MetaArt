using MetaArt.Core;
using MetaCore;

namespace MetaConstruct {
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
                .Select(x => (((PointView)x.Entity).point, location: calculator.CalcPoint(((PointView)x.Entity).point)))
                .OrderBy(x => Vector2.DistanceSquared(x.location, point))
                .Where(x => Vector2.DistanceSquared(x.location, point) < PointHitTestDistance * PointHitTestDistance)
                .Select(x => x.point);
        }

        public Point? HitTestIntersection(Vector2 point) {
            var calculator = CreateCalculator();
            var closeEntities = entities
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
                .Select(x => x.Entity switch {
                    LineSegment s => s.Line,
                    CircleSegment s => s.Circle,
                    Entity e => e
                })
                .Distinct()
                .ToArray();
            var intersections = closeEntities
                .SelectMany((x, i) => closeEntities.Skip(i + 1).Select(y => (x, y)))
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
}
