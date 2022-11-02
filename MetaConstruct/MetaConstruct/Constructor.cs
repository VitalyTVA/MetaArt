namespace MetaConstruct {
    public abstract class Point { }
    public sealed class LineLinePoint : Point {
        public LineLinePoint(Line line1, Line line2) {
            Line1 = line1;
            Line2 = line2;
        }
        public Line Line1 { get; }
        public Line Line2 { get; }
    }
    public enum CircleIntersectionKind { First, Second, Secondary }
    public sealed class LineCirclePoint : Point {
        public LineCirclePoint(Line line, Circle circle, CircleIntersectionKind intersection) {
            Line = line;
            Circle = circle;
            Intersection = intersection;
        }
        public Line Line { get; } 
        public Circle Circle { get; }
        public CircleIntersectionKind Intersection { get; }
    }
    public sealed class CircleCirclePoint : Point {
        public CircleCirclePoint(Circle circle1, Circle circle2, CircleIntersectionKind intersection) {
            Circle1 = circle1;
            Circle2 = circle2;
            Intersection = intersection;
        }
        public Circle Circle1 { get; }
        public Circle Circle2 { get; }
        public CircleIntersectionKind Intersection { get; }
    }

    public record class CircleIntersection(Point Point1, Point Point2);

    public abstract record Entity {
        protected Entity() => Validate();
        protected virtual void Validate() { }
    }
    public abstract record Primitive : Entity;
    public sealed record Line : Primitive {
        public Line(Point from, Point to) {
            From = from;
            To = to;
        }
        public Point From { get; init; }
        public Point To { get; init; }
    }
    public sealed record Circle(Point Center, Point Radius1, Point Radius2) : Primitive {
        public bool IsSimpleCircle => Radius1 == Center || Radius2 == Center;
        public Point GetPointOnCircle() {
            VerifySimple();
            return GetPointOnCircleCore();
        }

        Point GetPointOnCircleCore() => Radius1 == Center ? Radius2 : Radius1;

        public Point? TryGetPointOnCircle() {
            if(!IsSimpleCircle)
                return null;
            return GetPointOnCircleCore();
        }
        public bool VerifySimple() {
            if(!IsSimpleCircle)
                throw new InvalidOperationException();
            return true;
        }
    }

    public sealed class FreePoint : Point {
        public string Id { get; }

        public FreePoint(string id) {
            Id = id;
        }
    }
    public class Constructor {
        int idCount = 0;
        public FreePoint Point() => new FreePoint("P" + idCount++);

        readonly Dictionary<(Circle, Circle), CircleIntersection> circleIntersections = new();
        readonly Dictionary<(Line, Circle), CircleIntersection> lineCircleIntersections = new();
        readonly Dictionary<(Line, Line), Point> lineIntersections = new();

        public CircleIntersection Intersect(Circle c1, Circle c2) {
            if(c1 == c2)
                throw new InvalidOperationException();
            if(circleIntersections.TryGetValue((c1, c2), out var result))
                return result;
            return circleIntersections[(c1, c2)] = c1.IsSimpleCircle && c1.TryGetPointOnCircle() == c2.TryGetPointOnCircle()
                ? new CircleIntersection(
                    c1.GetPointOnCircle(),
                    new CircleCirclePoint(c1, c2, CircleIntersectionKind.Secondary)
                )
                : new CircleIntersection(
                    new CircleCirclePoint(c1, c2, CircleIntersectionKind.First),
                    new CircleCirclePoint(c1, c2, CircleIntersectionKind.Second)
                );
        }

        public CircleIntersection Intersect(Line l, Circle c) {
            if(lineCircleIntersections.TryGetValue((l, c), out var result))
                return result;
            return lineCircleIntersections[(l, c)] = l.From == c.TryGetPointOnCircle() || l.To == c.TryGetPointOnCircle()
                ? new CircleIntersection(c.GetPointOnCircle(), new LineCirclePoint(l, c, CircleIntersectionKind.Secondary))
                : new CircleIntersection(new LineCirclePoint(l, c, CircleIntersectionKind.First), new LineCirclePoint(l, c, CircleIntersectionKind.Second));
        }

        public Point Intersect(Line l1, Line l2) {
            if(l1 == l2)
                throw new InvalidOperationException();
            if(l1.From == l2.From || l1.From == l2.To)
                return l1.From;
            if(l1.To == l2.From || l1.To == l2.To)
                return l1.To;
            if(lineIntersections.TryGetValue((l1, l2), out var result))
                return result;
            return lineIntersections[(l1, l2)] = new LineLinePoint(l1, l2);
        }

        public Line Line(Point p1, Point p2) => new Line(p1, p2);
        public Circle Circle(Point center, Point point) => Circle(center, center, point);
        public Circle Circle(Point center, Point radius1, Point radius2) => new Circle(center, radius1, radius2);
    }
}
