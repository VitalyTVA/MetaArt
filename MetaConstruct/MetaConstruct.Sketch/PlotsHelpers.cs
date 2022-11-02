using System.Numerics;
using System.Runtime.CompilerServices;
using static MetaConstruct.ConstructorHelper;

namespace MetaConstruct {
    static class PlotsHelpers {
        public static readonly (Func<Constructor, Surface, PlotInfo> action, string name)[] Plots = new[] {
            RegisterPlot(Test1),
            RegisterPlot(Test2),
            RegisterPlot(Test3),
            RegisterPlot(Bisection),
            RegisterPlot(DivideX16),
            RegisterPlot(LineSegments),
            RegisterPlot(Square),
            RegisterPlot(Pentagon),
            RegisterPlot(Pentaspiral),
            RegisterPlot(SixCircles),
            RegisterPlot(Art1),
        };
        static (Func<Constructor, Surface, PlotInfo>, string) RegisterPlot(Func<Constructor, Surface, PlotInfo> action, [CallerArgumentExpression("action")] string name = "") {
            return (action, name);
        }

        static PlotInfo Test1(Constructor c, Surface s) {
            var center = c.Point();
            var top = c.Point();
            var centerCircle = c.Circle(center, top).Add(s);
            var c1 = c.Circle(top, center).Add(s);

            var c2 = c.Circle(c.Intersect(centerCircle, c1).Point1, center).Add(s);
            var c3 = c.Circle(c.Intersect(centerCircle, c2).Point1, center).Add(s);
            var c4 = c.Circle(c.Intersect(centerCircle, c3).Point1, center).Add(s);
            var c5 = c.Circle(c.Intersect(centerCircle, c4).Point1, center).Add(s);
            var c6 = c.Circle(c.Intersect(centerCircle, c5).Point1, center).Add(s);

            var l1 = c.Line(c2.Center, c4.Center).Add(s);
            var l2 = c.Line(c1.Center, c3.Center).Add(s);

            var p1 = c.Intersect(l1, l2);
            var p2 = c.Intersect(c.Line(p1, c6.Center), c6);
            var l3 = c.Line(p1, p2.Point1).Add(s);

            var s1 = CircleSegment(
                c1,
                c.Intersect(c1, centerCircle).Point2,
                c.Intersect(c1, c2).Point2
            ).Add(s);
            var s2 = CircleSegment(
                c2,
                c.Intersect(c2, c1).Point2,
                c.Intersect(c2, centerCircle).Point1
            ).Add(s);
            var s3 = CircleSegment(
                centerCircle,
                c.Intersect(centerCircle, c1).Point1,
                c.Intersect(centerCircle, c2).Point2
            ).Add(s);
            new Contour(new Segment[] {
            s1, s2, s3
        }).Add(s);
            l1.AsLineSegment().Add(s);
            l2.AsLineSegment().Add(s);
            l3.AsLineSegment().Add(s);

            return new PlotInfo(
                new[] {
                (center, new Vector2(400, 400)),
                (top, new Vector2(400, 300)),
                }
            );
        }

        static PlotInfo Test2(Constructor c, Surface s) {
            var p1 = c.Point();
            var p2 = c.Point();
            var p3 = c.Point();

            var c1 = c.Circle(p1, p3).Add(s);
            var c2 = c.Circle(p2, p3).Add(s);
            c.CircleSegment(c1, c2).Add(s);
            c.CircleSegment(c2, c1, invert: true).Add(s);

            return new PlotInfo(
                new[] {
                (p1, new Vector2(300, 400)),
                (p2, new Vector2(500, 400)),
                (p3, new Vector2(400, 300)),
                }
            );
        }

        static PlotInfo Test3(Constructor c, Surface s) {
            var p1 = c.Point();
            var p2 = c.Point();
            var p3 = c.Point();

            var l = c.Line(p3, p1).Add(s);
            var c_ = c.Circle(p2, p3).Add(s);

            c.CircleSegment(c_, l).Add(s);
            c.LineSegment(l, c_).Add(s);


            return new PlotInfo(
                new[] {
                (p1, new Vector2(300, 400)),
                (p2, new Vector2(400, 400)),
                (p3, new Vector2(400, 300)),
                }
            );
        }

        static PlotInfo Bisection(Constructor c, Surface s) {
            var p1 = c.Point();
            var p2 = c.Point();

            c.LineSegment(p1, p2).Add(s, DisplayStyle.Visible);
            var (bisection, _, (c1, c2)) = c.Bisection(p1, p2);
            bisection.Add(s);
            c1.Add(s);
            c2.Add(s);

            return new PlotInfo(
                new[] {
                (p1, new Vector2(300, 400)),
                (p2, new Vector2(500, 400)),
                }
            );
        }

        static PlotInfo DivideX16(Constructor c, Surface s) {
            var p1 = c.Point();
            var p2 = c.Point();

            c.LineSegment(p1, p2).Add(s);

            var (points, segments) = c.DivideNTimes(p1, p2, 4);

            foreach(var item in segments) {
                item.Add(s, DisplayStyle.Background);
            }

            return new PlotInfo(
                new[] {
                (p1, new Vector2(200, 400)),
                (p2, new Vector2(600, 400)),
                }
            );
        }

        static PlotInfo LineSegments(Constructor c, Surface s) {
            var p1 = c.Point();
            var p2 = c.Point();
            var p3 = c.Point();
            var p4 = c.Point();

            c.LineSegment(p1, p2).Add(s);
            var line = c.Line(p3, p4).Add(s);

            var (points, circles) = c.MakeLineSegments(p1, p2, line.AsLineSegment(), 7);

            foreach(var item in circles) {
                item.Add(s, DisplayStyle.Background);
            }

            c.LineSegment(points.First(), points.Last()).Add(s);

            foreach(var item in new[] { p1, p2, p4 }.Concat(points)) {
                item.AsView().Add(s);
            }

            return new PlotInfo(
                new[] {
                (p1, new Vector2(200, 300)),
                (p2, new Vector2(250, 300)),
                (p3, new Vector2(200, 450)),
                (p4, new Vector2(600, 450)),
                }
            );
        }

        static PlotInfo Square(Constructor c, Surface s) {
            var center = c.Point();
            var vertex0 = c.Point();

            var ((vertex1, vertex2, vertex3), (circle, diameter1, diameter2)) = c.Square(center, vertex0);

            circle.Add(s);
            c.LineSegment(diameter1, circle).Add(s, DisplayStyle.Background);
            c.LineSegment(diameter2, circle).Add(s, DisplayStyle.Background);

            c.LineSegment(vertex0, vertex1).Add(s);
            c.LineSegment(vertex1, vertex2).Add(s);
            c.LineSegment(vertex2, vertex3).Add(s);
            c.LineSegment(vertex3, vertex0).Add(s);

            return new PlotInfo(
                new[] {
                (center, new Vector2(400, 400)),
                (vertex0, new Vector2(300, 300)),
                }
            );
        }

        static PlotInfo Pentagon(Constructor c, Surface s) {
            var center = c.Point();
            var vertex0 = c.Point();

            var ((vertex1, vertex2, vertex3, vertex4), primitives) = c.Pentagon(center, vertex0);

            primitives.circle.Add(s);
            primitives.diameter.Add(s);
            primitives.bisection1.Add(s);

            c.LineSegment(primitives.bisection2, primitives.circle).Add(s, DisplayStyle.Background);
            c.CircleSegment(primitives.circle1, primitives.bisection1).Add(s, DisplayStyle.Background);
            c.CircleSegment(primitives.circle2, primitives.circle).Add(s, DisplayStyle.Background);
            c.CircleSegment(primitives.circle3, primitives.circle).Add(s, DisplayStyle.Background);

            c.LineSegment(vertex0, vertex1).Add(s);
            c.LineSegment(vertex1, vertex2).Add(s);
            c.LineSegment(vertex2, vertex3).Add(s);
            c.LineSegment(vertex3, vertex4).Add(s);
            c.LineSegment(vertex4, vertex0).Add(s);

            return new PlotInfo(
                new[] {
                (center, new Vector2(400, 400)),
                (vertex0, new Vector2(400, 300)),
                }
            );
        }

        static PlotInfo Pentaspiral(Constructor c, Surface s) {
            var center = c.Point();
            var vertex0 = c.Point();

            LineSegment[] MakePentagram(Point v0, Point v1, Point v2, Point v3, Point v4) => new[] {
            c.LineSegment(v0, v2).Add(s, DisplayStyle.Background),
            c.LineSegment(v2, v4).Add(s, DisplayStyle.Background),
            c.LineSegment(v4, v1).Add(s, DisplayStyle.Background),
            c.LineSegment(v1, v3).Add(s, DisplayStyle.Background),
            c.LineSegment(v3, v0).Add(s, DisplayStyle.Background),
        };

            var ((vertex1, vertex2, vertex3, vertex4), _) = c.Pentagon(center, vertex0);
            var segments = MakePentagram(vertex0, vertex1, vertex2, vertex3, vertex4);

            var vertex0_ = c.Intersect(c.Line(vertex0, vertex4), c.Line(vertex1, vertex2));
            var ((vertex1_, vertex2_, vertex3_, vertex4_), _) = c.Pentagon(center, vertex0_);
            var segments_ = MakePentagram(vertex0_, vertex1_, vertex2_, vertex3_, vertex4_);

            for(int i = 0; i < 5; i++) {
                var s1 = segments[(i + 1) % 5];
                var s2_ = segments_[(i + 2) % 5];

                var s4 = segments[(i + 4) % 5];
                var line = c.Line(s2_.To, center).AsLineSegment().Add(s, DisplayStyle.Background);

                var center_ = c.Intersect(s1.Line, s4.Line);
                var circle = c.Circle(center_, segments[i].From);
                var inersection = c.Intersect(line.Line, circle);
                CircleSegment(circle, circle.GetPointOnCircle(), inersection.Point2).Add(s);
            }
            for(int i = 0; i < 5; i++) {
                var circle = c.Circle(segments[(i + 3) % 5].From, segments_[i].From);
                var line = c.Line(segments_[i].From, center);
                var inersection = c.Intersect(line, circle);
                CircleSegment(circle, circle.GetPointOnCircle(), inersection.Point2).Add(s);
            }

            return new PlotInfo(
                new[] {
                (center, new Vector2(400, 400)),
                (vertex0, new Vector2(400, 300)),
                }
            );
        }

        static PlotInfo SixCircles(Constructor c, Surface s) {
            var center = c.Point();
            var top = c.Point();
            var centerCircle = c.Circle(center, top).Add(s);
            var c1 = c.Circle(top, center).Add(s);

            var c2 = c.Circle(c.Intersect(centerCircle, c1).Point1, center).Add(s);
            var c3 = c.Circle(c.Intersect(centerCircle, c2).Point1, center).Add(s);
            var c4 = c.Circle(c.Intersect(centerCircle, c3).Point1, center).Add(s);
            var c5 = c.Circle(c.Intersect(centerCircle, c4).Point1, center).Add(s);
            var c6 = c.Circle(c.Intersect(centerCircle, c5).Point1, center).Add(s);

            c.CircleSegment(c1, centerCircle).Add(s);
            c.CircleSegment(c2, centerCircle).Add(s);
            c.CircleSegment(c3, centerCircle).Add(s);
            c.CircleSegment(c4, centerCircle).Add(s);
            c.CircleSegment(c5, centerCircle).Add(s);
            c.CircleSegment(c6, centerCircle).Add(s);

            return new PlotInfo(
                new[] {
                (center, new Vector2(400, 400)),
                (top, new Vector2(400, 300)),
                }
            );
        }

        static PlotInfo Art1(Constructor c, Surface s) {
            var center = c.Point();
            var vertex0 = c.Point();
            var ((vertex1, vertex2, vertex3), _) = c.Square(center, vertex0);

            c.LineSegment(vertex0, vertex1).Add(s);
            c.LineSegment(vertex1, vertex2).Add(s);
            c.LineSegment(vertex2, vertex3).Add(s);
            c.LineSegment(vertex3, vertex0).Add(s);

            var n = 5;
            var countN = 2 << n - 1;
            var points1 = c.DivideNTimes(vertex0, vertex1, n).result;
            var points2 = c.DivideNTimes(vertex1, vertex2, n).result;
            var points3 = c.DivideNTimes(vertex2, vertex3, n).result;
            var points4 = c.DivideNTimes(vertex3, vertex0, n).result;

            for(int i = 0; i < countN; i++) {
                c.LineSegment(points1[i], points2[i]).Add(s);
                c.LineSegment(points2[i], points3[i]).Add(s);
                c.LineSegment(points3[i], points4[i]).Add(s);
                c.LineSegment(points4[i], points1[i]).Add(s);
            }

            var diameterPoints1 = c.DivideNTimes(vertex0, vertex2, 4).result;
            var diameterPoints2 = c.DivideNTimes(vertex1, vertex3, 4).result;
            var (p1, p2) = (diameterPoints1[6], diameterPoints1[10]);
            var (p3, p4) = (diameterPoints2[6], diameterPoints2[10]);
            c.LineSegment(p1, p2).Add(s);
            c.LineSegment(p3, p4).Add(s);

            var m = 4;
            var countM = 2 << m - 1;
            var innerPoints1 = c.DivideNTimes(p1, p2, m).result;
            var innerPoints2 = c.DivideNTimes(p3, p4, m).result;
            for(int i = 0; i < countM / 2; i++) {
                c.LineSegment(innerPoints1[i], innerPoints2[countM / 2 - 1 - i]).Add(s);
                c.LineSegment(innerPoints1[i], innerPoints2[countM / 2 + 1 + i]).Add(s);
                c.LineSegment(innerPoints1[countM - i], innerPoints2[countM / 2 - 1 - i]).Add(s);
                c.LineSegment(innerPoints1[countM - i], innerPoints2[countM / 2 + 1 + i]).Add(s);
            }
            for(int i = 0; i < countM / 2 + 1; i++) {
                c.LineSegment(innerPoints1[i], points4[i]).Add(s);
                c.LineSegment(innerPoints2[i], points1[i]).Add(s);
                c.LineSegment(innerPoints1[countM - i], points2[i]).Add(s);
                c.LineSegment(innerPoints2[countM - i], points3[i]).Add(s);
            }


            return new PlotInfo(
                new[] {
                (center, new Vector2(400, 400)),
                (vertex0, new Vector2(100, 100)),
                }
            );
        }

    }
}
