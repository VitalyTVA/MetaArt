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
    public enum CircleIntersectionKind { First, Second }
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

    public record LineSegment(Line Line, Point From, Point To) : Entity {
        protected override void Validate() {
            base.Validate();
            VerifyPoint(From);
            VerifyPoint(To);
        }

        void VerifyPoint(Point p) {
            if(p == Line.From || p == Line.To)
                return;
            switch(p) {
                case LineLinePoint lineLinePoint:
                    if(lineLinePoint.Line1 != Line && lineLinePoint.Line2 != Line)
                        throw new InvalidOperationException();
                    break;
                case LineCirclePoint lineCirclePoint:
                    if(lineCirclePoint.Line != Line)
                        throw new InvalidOperationException();
                    break;
                default:
                    throw new InvalidOperationException();
            };
        }
    }

    public record CircleSegment(Circle Circle, Point From, Point To) : Entity {
        protected override void Validate() {
            base.Validate();
            VerifyPoint(From);
            VerifyPoint(To);
        }

        void VerifyPoint(Point p) {
            if(Circle.Point == p)
                return;
            switch(p) {
                case LineCirclePoint lineCirclePoint:
                    if(lineCirclePoint.Circle != Circle)
                        throw new InvalidOperationException();
                    break;
                case CircleCirclePoint circleCirclePoint:
                    if(circleCirclePoint.Circle1 != Circle && circleCirclePoint.Circle2 != Circle)
                        throw new InvalidOperationException();
                    break;
                default:
                    throw new InvalidOperationException();
            };
        }
    }

    public static class Constructor {
        public static FreePoint Point() => new FreePoint();
        public static LineSegment LineSegment(Line line) => LineSegment(line, line.From, line.To);
        public static LineSegment LineSegment(Line line, Point from, Point to) => new LineSegment(line, from, to);
        public static CircleSegment CircleSegment(Circle circle, Point from, Point to) => new CircleSegment(circle, from, to);
        public static Line Line(Point p1, Point p2) => new Line(p1, p2);
        public static Circle Circle(Point center, Point point) => new Circle(center, point);

        public static CircleIntersection Intersect(Circle c1, Circle c2) {
            if(c1 == c2)
                throw new InvalidOperationException();
            return new CircleIntersection(
                c1.Point == c2.Point ? c1.Point : new CircleCirclePoint(c1, c2, CircleIntersectionKind.First), 
                new CircleCirclePoint(c1, c2, CircleIntersectionKind.Second)
            );
        }

        public static CircleIntersection Intersect(Line l, Circle c) {
            var point1 = l.From == c.Point || l.To == c.Point ? c.Point : new LineCirclePoint(l, c, CircleIntersectionKind.First);
            return new CircleIntersection(point1, new LineCirclePoint(l, c, CircleIntersectionKind.Second));
        }

        public static Point Intersect(Line l1, Line l2) {
            if(l1 == l2)
                throw new InvalidOperationException();
            if(l1.From == l2.From || l1.From == l2.To)
                return l1.From;
            if(l1.To == l2.From || l1.To == l2.To)
                return l1.To;
            return new LineLinePoint(l1, l2);
        }
    }
}
