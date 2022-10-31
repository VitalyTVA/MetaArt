namespace MetaConstruct {
    public abstract record Point;
    public sealed record LineLinePoint(Line Line1, Line Line2) : Point {
        //public LineLinePoint(Line line1, Line line2) {
        //    Line1 = line1;
        //    Line2 = line2;
        //}
        //public Line Line1 { get; init; } 
        //public Line Line2 { get; init; } 
    }
    public enum CircleIntersectionKind { First, Second, Secondary }
    public sealed record LineCirclePoint(Line Line, Circle Circle, CircleIntersectionKind Intersection) : Point;
    public sealed record CircleCirclePoint(Circle Circle1, Circle Circle2, CircleIntersectionKind Intersection) : Point;

    public record struct CircleIntersection(Point Point1, Point Point2);

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
    public sealed record Circle(Point Center, Point Point) : Primitive;

    public sealed record FreePoint : Point {
        readonly object obj = new object();
        public int GetHashCodeEx() => obj.GetHashCode();
        public override int GetHashCode() {
            throw new InvalidOperationException($"Use {nameof(GetHashCodeEx)} instead.");
        }
    }
    public class Constructor {
        public FreePoint Point() => new FreePoint();
        public CircleIntersection Intersect(Circle c1, Circle c2) {
            if(c1 == c2)
                throw new InvalidOperationException();
            if(c1.Point == c2.Point) {
                return new CircleIntersection(
                    c1.Point,
                    new CircleCirclePoint(c1, c2, CircleIntersectionKind.Secondary)
                );
            }
            return new CircleIntersection(
                new CircleCirclePoint(c1, c2, CircleIntersectionKind.First),
                new CircleCirclePoint(c1, c2, CircleIntersectionKind.Second)
            );
        }

        public CircleIntersection Intersect(Line l, Circle c) {
            if(l.From == c.Point || l.To == c.Point) {
                return new CircleIntersection(c.Point, new LineCirclePoint(l, c, CircleIntersectionKind.Secondary));
            }
            return new CircleIntersection(new LineCirclePoint(l, c, CircleIntersectionKind.First), new LineCirclePoint(l, c, CircleIntersectionKind.Second));
        }

        public Point Intersect(Line l1, Line l2) {
            if(l1 == l2)
                throw new InvalidOperationException();
            if(l1.From == l2.From || l1.From == l2.To)
                return l1.From;
            if(l1.To == l2.From || l1.To == l2.To)
                return l1.To;
            return new LineLinePoint(l1, l2);
        }

        public Line Line(Point p1, Point p2) => new Line(p1, p2);
        public Circle Circle(Point center, Point point) => new Circle(center, point);
    }
}
