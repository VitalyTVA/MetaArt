namespace MetaConstruct {
    public static class PlotPrimitives {
        public static (Line bisection, Point middle, (Circle c1, Circle c2) primitives) Bisection(this Constructor c, Point p1, Point p2) {
            var c1 = c.Circle(p1, p2);
            var c2 = c.Circle(p2, p1);
            var bisection = c.AsLine(c.Intersect(c1, c2));
            var middle = c.Intersect(bisection, c.Line(p1, p2));
            return (bisection, middle, (c1, c2));
        }

        public static (Point[] result, Segment[] segments) DivideNTimes(this Constructor c, Point p1, Point p2, int n) {
            var points = new List<Point> { p1, p2 };
            var segments = new List<Segment>();
            for(int i = 0; i < n; i++) {
                for(int j = 0; j < points.Count - 1; j++) {
                    var (bisection, middle, (c1, c2)) = c.Bisection(points[j], points[j + 1]);
                    points.Insert(j + 1, middle);
                    j++;
                    segments.Add(c.CircleSegment(c1, c2));
                    segments.Add(c.CircleSegment(c2, c1));
                    segments.Add(c.AsLine(c.Intersect(c2, c1)).AsLineSegment());
                }
            }
            return (points.ToArray(), segments.ToArray());
        }

        public static (Point[] result, Circle[] circles) MakeLineSegments(this Constructor c, Point p1, Point p2, LineSegment start, int n) {
            var point = start.From;
            var points = new List<Point>() { point };
            var circles = new List<Circle>();
            for(int i = 0; i < n; i++) {
                var circle = c.Circle(point, p1, p2);
                point = c.Intersect(start.Line, circle).Point1;
                points.Add(point);
                circles.Add(circle);
            }
            return (points.ToArray(), circles.ToArray());
        }

        public static
            (
                (Point vertex1, Point vertex2, Point vertex3, Point vertex4) result,
                (Circle circle, Line diameter, Line bisection1, Line bisection2, Circle circle1, Circle circle2, Circle circle3) primitives
            )
            Pentagon(this Constructor c, Point center, Point vertex0) {
            var circle = c.Circle(center, vertex0);
            var diameter = c.Line(center, vertex0);

            var bisection1 = c.Bisection(vertex0, c.Intersect(diameter, circle).Point2).bisection;

            var bisection2 = c.Bisection(center, c.Intersect(bisection1, circle).Point2).bisection;
            c.LineSegment(bisection2, circle);
            var circle1 = c.Circle(c.Intersect(bisection1, bisection2), vertex0);
            c.CircleSegment(circle1, bisection1);

            var intersection = c.Intersect(bisection1, circle1);

            var circle2 = c.Circle(vertex0, intersection.Point1);
            c.CircleSegment(circle2, circle);
            var (vertex4, vertex1) = c.Intersect(circle, circle2);

            var circle3 = c.Circle(vertex0, intersection.Point2);
            c.CircleSegment(circle3, circle);
            var (vertex3, vertex2) = c.Intersect(circle, circle3);

            return ((vertex1, vertex2, vertex3, vertex4), (circle, diameter, bisection1, bisection2, circle1, circle2, circle3));
        }

        public static
            (
                (Point vertex1, Point vertex2, Point vertex3) result,
                (Circle circle, Line diameter1, Line diameter2) primitives
            )
            Square(this Constructor c, Point center, Point vertex0) {
            var circle = c.Circle(center, vertex0);
            var diameter1 = c.Line(center, vertex0);
            var diameter2 = c.Bisection(vertex0, c.Intersect(diameter1, circle).Point2).bisection;
            var vertex2 = c.Intersect(diameter1, circle).Point2;
            var (vertex1, vertex3) = c.Intersect(diameter2, circle);
            return ((vertex1, vertex2, vertex3), (circle, diameter1, diameter2));
        }
    }
}