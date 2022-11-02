using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using static MetaConstruct.ConstructorHelper;

namespace MetaContruct.Tests {
    [TestFixture]
    public class PlotTests : ModelTestsBase {
        [Test]
        public void LineTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            AssertPlot(
@"line (1.0000 2.0000) (5.0000 6.0000)
line:Background (1.0000 2.0000) (9.0000 13.0000)
",
                 points: new[] {
                    (p1, new Vector2(1, 2)),
                    (p2, new Vector2(5, 6)),
                    (p3, new Vector2(9, 13)),
                },
                (Line(p1, p2), DisplayStyle.Visible),
                (Line(p1, p3), DisplayStyle.Background)
            );
        }

        [Test]
        public void LineSegmentTest() {
            var p1 = Point();
            var p2 = Point();

            AssertPlot(
@"lineSegment (1.0000 2.0000) (5.0000 6.0000)
",
                 points: new[] {
                    (p1, new Vector2(1, 2)),
                    (p2, new Vector2(5, 6))
                },
                LineSegment(Line(p1, p2))
            );
        }

        [Test]
        public void CircleTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            AssertPlot(
@"circle (1.0000 2.0000) 5.6569
circle:Background (1.0000 2.0000) 13.6015
",
                 points: new[] {
                    (p1, new Vector2(1, 2)),
                    (p2, new Vector2(5, 6)),
                    (p3, new Vector2(9, 13)),
                },
                (Circle(p1, p2), DisplayStyle.Visible),
                (Circle(p1, p3), DisplayStyle.Background)
            );
        }

        [Test]
        public void LineCircleIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();

            var c = Circle(p1, p2);
            var l = Line(p3, p4);

            AssertPlot(
@"lineSegment (6.7720 -1.7720) (-1.7720 6.7720)
circleSegment (1.0000 1.0000) 6.4031 -25.6526 115.6526
",
                 points: new[] {
                    (p1, new Vector2(1, 1)),
                    (p2, new Vector2(5, 6)),
                    (p3, new Vector2(1, 4)),
                    (p4, new Vector2(4, 1)),
                },
                LineSegment(l, c),
                CircleSegment(c, l)
            );
        }

        [Test]
        public void CirclesIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();

            var c1 = Circle(p1, p2);
            var c2 = Circle(p3, p4);

            AssertPlot(
@"circleSegment (1.0000 1.0000) 6.4031 56.4006 123.5994
circleSegment (1.0000 4.0000) 4.2426 -213.3651 33.3651
",
                 points: new[] {
                    (p1, new Vector2(1, 1)),
                    (p2, new Vector2(5, 6)),
                    (p3, new Vector2(1, 4)),
                    (p4, new Vector2(4, 1)),
                },
                CircleSegment(c1, c2),
                CircleSegment(c2, c1)
            );
        }

        [Test]
        public void CirclesIntersection_CommonPointTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);

            AssertPlot(
@"circleSegment (-4.0000 0.0000) 5.0000 36.8699 323.1301
circleSegment (4.0000 0.0000) 5.0000 143.1301 216.8699
",
                points: new[] {
                    (p1, new Vector2(-4, 0)),
                    (p2, new Vector2(4, 0)),
                    (p3, new Vector2(0, 3)),
                },
                CircleSegment(c1, c2),
                CircleSegment(c2, c1)
            );

            AssertPlot(
@"circleSegment (-4.0000 0.0000) 5.0000 -36.8699 36.8699
circleSegment (4.0000 0.0000) 5.0000 -143.1301 143.1301
",
                points: new[] {
                    (p1, new Vector2(-4, 0)),
                    (p2, new Vector2(4, 0)),
                    (p3, new Vector2(0, 3)),
                },
                CircleSegment(c1, c2, invert: true),
                CircleSegment(c2, c1, invert: true)
            );
        }

        [Test]
        public void CirclesIntersection_CommonPointTest_NonSimpleCircle() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();

            var c1 = Circle(p1, p4, p3);
            var c2 = Circle(p2, p3);

            var s1 = CircleSegment(c1, c2);
            var s2 = CircleSegment(c2, c1);
            AssertPlot(
@"circleSegment (-4.0000 0.0000) 5.0080 -36.8965 36.8965
circleSegment (4.0000 0.0000) 5.0000 143.0346 216.9654
",
                points: new[] {
                    (p1, new Vector2(-4, 0)),
                    (p2, new Vector2(4, 0)),
                    (p3, new Vector2(0, 3)),
                    (p4, new Vector2(-4.01f, 0)),
                },
                s1,
                s2
            );
        }

        [Test]
        public void LineCircleIntersection_CommonPointTest1() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var l = Line(p1, p3);
            var c = Circle(p2, p3);

            AssertPlot(
@"circleSegment (4.0000 0.0000) 5.0000 -216.8699 110.6097
lineSegment (0.0000 3.0000) (2.2400 4.6800)
",
                points: new[] {
                    (p1, new Vector2(-4, 0)),
                    (p2, new Vector2(4, 0)),
                    (p3, new Vector2(0, 3)),
                },
                CircleSegment(c, l),
                LineSegment(l, c)
            );
        }

        [Test]
        public void LineCircleIntersection_CommonPointTest2() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var l = Line(p3, p1);
            var c = Circle(p2, p3);

            AssertPlot(
@"circleSegment (4.0000 0.0000) 5.0000 -216.8699 110.6097
lineSegment:Background (0.0000 3.0000) (2.2400 4.6800)
",
                points: new[] {
                    (p1, new Vector2(-4, 0)),
                    (p2, new Vector2(4, 0)),
                    (p3, new Vector2(0, 3)),
                },
                (CircleSegment(c, l), DisplayStyle.Visible),
                (LineSegment(l, c), DisplayStyle.Background)
            );
        }
        [Test]
        public void LineCircleIntersection_CommonPointTest3() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var l = Line(p3, p1);
            var c = Circle(p2, p3);

            AssertPlot(
@"circleSegment:Background (4.0000 4.0000) 1.0000 -90.0000 180.0000
lineSegment (4.0000 3.0000) (3.0000 4.0000)
",
                points: new[] {
                    (p1, new Vector2(3, 4)),
                    (p2, new Vector2(4, 4)),
                    (p3, new Vector2(4, 3)),
                },
                (CircleSegment(c, l), DisplayStyle.Background),
                (LineSegment(l, c), DisplayStyle.Visible)
            );
        }

        [Test]
        public void LineCircleIntersection_CommonPointTest_NonSimpleCircle() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();

            var l = Line(p1, p3);
            var c = Circle(p2, p4, p3);

            AssertPlot(
@"circleSegment (4.0000 0.0000) 5.0080 110.2986 143.4412
lineSegment (2.2627 4.6970) (-0.0227 2.9830)
",
                points: new[] {
                    (p1, new Vector2(-4, 0)),
                    (p2, new Vector2(4, 0)),
                    (p3, new Vector2(0, 3)),
                    (p4, new Vector2(4.01f, 0)),
                },
                CircleSegment(c, l),
                LineSegment(l, c)
            );
        }

        [Test]
        public void LinesIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();

            var i1 = Intersect(Line(p1, p2), Line(p3, p4));
            var i2 = Intersect(Line(p1, p3), Line(p2, p4));

            AssertPlot(
@"line (2.3333 2.6667) (1.0000 -14.0000)
",
                 points: new[] {
                    (p1, new Vector2(1, 1)),
                    (p2, new Vector2(5, 6)),
                    (p3, new Vector2(1, 4)),
                    (p4, new Vector2(4, 1)),
                },
                Line(i1, i2)
            );
        }

        [Test]
        public void ContourTest() {
            var center = Point();
            var top = Point();
            var centerCircle = Circle(center, top);
            var c1 = Circle(top, center);
            var c2 = Circle(Intersect(centerCircle, c1).Point1, center);
            var s1 = CircleSegment(
                c1,
                Intersect(c1, centerCircle).Point2,
                Intersect(c1, c2).Point2
            );
            var s2 = CircleSegment(
                c2,
                Intersect(c2, c1).Point2,
                Intersect(c2, centerCircle).Point1
            );
            var s3 = CircleSegment(
                centerCircle,
                Intersect(centerCircle, c1).Point1,
                Intersect(centerCircle, c2).Point2
            );
            var contour = new Contour(new Segment[] {
                s1, s2, s3
            });
            AssertPlot(
@"contour {
    circleSegment (0.0000 1.0000) 1.0000 -30.0000 30.0000
    circleSegment (0.8660 0.5000) 1.0000 90.0000 150.0000
    circleSegment (0.0000 0.0000) 1.0000 30.0000 90.0000
}
",
                 points: new[] {
                    (center, new Vector2(0, 0)),
                    (top, new Vector2(0, 1)),
                },
                contour
            );
            AssertPlot(
@"contour {
    circleSegment:Background (0.0000 1.0000) 1.0000 -30.0000 30.0000
    circleSegment:Background (0.8660 0.5000) 1.0000 90.0000 150.0000
    circleSegment:Background (0.0000 0.0000) 1.0000 30.0000 90.0000
}
",
                 points: new[] {
                    (center, new Vector2(0, 0)),
                    (top, new Vector2(0, 1)),
                },
                (contour, DisplayStyle.Background)
            );
        }
        static void AssertPlot(
            string expected, (FreePoint, Vector2)[] points, params Entity[] primitives) {
            AssertPlot(expected, points, primitives.Select(x => (x, DisplayStyle.Visible)).ToArray());
        }
        static void AssertPlot(string expected, (FreePoint, Vector2)[] points, params (Entity, DisplayStyle) [] primitives) {
            var sb = new StringBuilder();
            StringBuilder AppendCircleSegment(CircleSegmentF s, DisplayStyle style)
                => sb.AppendLine($"circleSegment{GetStyleString(style)} {s.circle.center.VectorToString()} {s.circle.radius.FloatToString()} {s.from.RadToDeg().FloatToString()} {s.to.RadToDeg().FloatToString()}");
            static string GetStyleString(DisplayStyle style) => style == DisplayStyle.Visible ? string.Empty : ":" + style.ToString();
            Plotter.Draw(
                points: points,
                new Painter(
                    DrawLine: (l, style) => sb.AppendLine($"line{GetStyleString(style)} {l.LineFToString()}"),
                    DrawLineSegment: (s, style) => sb.AppendLine($"lineSegment{GetStyleString(style)} {s.LineFToString()}"),
                    DrawCircle: (c, style) => sb.AppendLine($"circle{GetStyleString(style)} {c.CircleFToString()}"),
                    DrawCircleSegment: (s, style) => AppendCircleSegment(s, style),
                    FillContour: (contour, style) => {
                        sb.AppendLine("contour {");
                        foreach(var item in contour) {
                            sb.Append("    ");
                            AppendCircleSegment(item, style);
                        }
                        sb.AppendLine("}");
                    }
                ),
                primitives: primitives
            );
            Assert.AreEqual(expected, sb.ToString());
        }
    }

    static class TestExtensions {
        public static string LineFToString(this LineF l) => $"{l.from.VectorToString()} {l.to.VectorToString()}";
        public static string CircleFToString(this CircleF c) => $"{c.center.VectorToString()} {c.radius.FloatToString()}";

        public static string VectorToString(this Vector2 v) => $"({v.X:n4} {v.Y:n4})";
        public static string FloatToString(this float v) => v.ToString("n4");
        public static float RadToDeg(this float v) => v * 180 / MathF.PI;
    }
}
