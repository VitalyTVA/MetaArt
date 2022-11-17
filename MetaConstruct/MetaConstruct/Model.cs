namespace MetaConstruct {

    public record PointView(Point point) : Entity;

    public abstract record Segment : Entity;

    public record LineSegment(Line Line, Point From, Point To) : Segment {
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

    public record CircleSegment(Circle Circle, Point From, Point To) : Segment {
        protected override void Validate() {
            base.Validate();
            VerifyPoint(From);
            VerifyPoint(To);
        }

        void VerifyPoint(Point p) {
            if(Circle.TryGetPointOnCircle() == p)
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

    public record Contour(Segment[] Segments) : Segment { 
    }
    public enum DisplayStyle { Background, Visible }

    public static class ConstructorHelper {
        public static PointView AsView(this Point p) => new PointView(p);

        public static CircleSegment CircleSegment(this Constructor constructor, Circle circle, Circle other, bool invert = false) {
            var (p1, p2) = constructor.Intersect(circle, other);
            return CircleSegment(circle, invert, p1, p2);
        }

        public static CircleSegment CircleSegment(this Constructor constructor, Circle circle, Line line, bool invert = false) {
            var (p1, p2) = constructor.Intersect(line, circle);
            return CircleSegment(circle, invert, p1, p2);
        }

        static CircleSegment CircleSegment(Circle circle, bool invert, Point p1, Point p2) {
            return new CircleSegment(circle, invert ? p2 : p1, invert ? p1 : p2);
        }

        public static CircleSegment? CircleSegment(Point p1, Point p2) {
            switch((p1, p2)) {
                case (FreePoint from, CircleCirclePoint to):
                    return CircleSegment(to.Circle2, false, p1, p2);
                case (CircleCirclePoint from, FreePoint to) when from.Circle2.TryGetPointOnCircle() == to:
                    return CircleSegment(from.Circle2, false, p1, p2);
                case (CircleCirclePoint from, CircleCirclePoint to) when from.Circle2 == to.Circle2:
                    return CircleSegment(from.Circle2, false, p1, p2);
                default:
                    return null;
            }
        }

        public static LineSegment LineSegment(Line line, Point from, Point to) => new LineSegment(line, from, to);
        public static CircleSegment CircleSegment(Circle circle, Point from, Point to) => new CircleSegment(circle, from, to);

        public static LineSegment LineSegment(this Constructor constructor, Point from, Point to) => constructor.Line(from, to).AsLineSegment();
        public static LineSegment AsLineSegment(this Line line) => LineSegment(line, line.From, line.To);
        public static LineSegment LineSegment(this Constructor constructor, Line line, Circle circle) {
            var intersection = constructor.Intersect(line, circle);
            return LineSegment(line, intersection.Point1, intersection.Point2);
        }

        public static Line AsLine(this Constructor constructor, CircleIntersection intersection) => constructor.Line(intersection.Point1, intersection.Point2);
    }
}
