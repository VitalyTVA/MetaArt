using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;
using System.Text;
using static MetaConstruct.Constructor;

namespace MetaContruct.Tests {
    [TestFixture]
    public class PlotTests {
        [Test]
        public void LineTest() {
            var p1 = Point();
            var p2 = Point();

            AssertPlot(
@"line (1.0000 2.0000) (5.0000 6.0000)
",
                 points: new[] {
                    (p1, new Vector2(1, 2)),
                    (p2, new Vector2(5, 6))
                },
                Line(p1, p2)
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

            AssertPlot(
@"circle (1.0000 2.0000) 5.6569
",
                 points: new[] {
                    (p1, new Vector2(1, 2)),
                    (p2, new Vector2(5, 6))
                },
                Circle(p1, p2)
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
                CircleSegment(c1, c2).Invert(),
                CircleSegment(c2, c1).Invert()
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
        public void LineCircleIntersection_CommonPointTest3() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var l = Line(p3, p1);
            var c = Circle(p2, p3);

            AssertPlot(
@"circleSegment (4.0000 4.0000) 1.0000 -90.0000 180.0000
lineSegment (4.0000 3.0000) (3.0000 4.0000)
",
                points: new[] {
                    (p1, new Vector2(3, 4)),
                    (p2, new Vector2(4, 4)),
                    (p3, new Vector2(4, 3)),
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

        static void AssertPlot(string expected, (FreePoint, Vector2)[] points, params Entity[] primitives) {
            var sb = new StringBuilder();
            Plotter.Draw(
                points: points,
                new Painter(
                    DrawLine: l => sb.AppendLine($"line {l.from.VectorToString()} {l.to.VectorToString()}"),
                    DrawLineSegment: s => sb.AppendLine($"lineSegment {s.from.VectorToString()} {s.to.VectorToString()}"),
                    DrawCircle: c => sb.AppendLine($"circle {c.center.VectorToString()} {c.radius.FloatToString()}"),
                    DrawCircleSegment: s => sb.AppendLine($"circleSegment {s.circle.center.VectorToString()} {s.circle.radius.FloatToString()} {s.from.RadToDeg().FloatToString()} {s.to.RadToDeg().FloatToString()}")
                ),
                primitives: primitives
            );
            Assert.AreEqual(expected, sb.ToString());
        }
    }

    static class TestExtensions {
        public static string VectorToString(this Vector2 v) => $"({v.X:n4} {v.Y:n4})";
        public static string FloatToString(this float v) => v.ToString("n4");
        public static float RadToDeg(this float v) => v * 180 / MathF.PI;
    }
}
