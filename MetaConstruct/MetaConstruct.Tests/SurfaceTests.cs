using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;
using static MetaConstruct.ConstructorHelper;

namespace MetaContruct.Tests {
    [TestFixture]
    public class SurfaceTests : ConstructorTestsBase {
        [Test]
        public void MovePointTest() {
            var p1 = Point();
            var p2 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(1, 2)),
                (p2, new Vector2(3, 4)),
            });

            Assert.AreEqual(new Vector2(1, 2), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(3, 4), surface.GetPointLocation(p2));

            surface.SetPointLocation(p1, new Vector2(9, 13));
            Assert.AreEqual(new Vector2(9, 13), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(3, 4), surface.GetPointLocation(p2));

            surface.SetPointLocation(p2, new Vector2(117, 253));
            Assert.AreEqual(new Vector2(9, 13), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(117, 253), surface.GetPointLocation(p2));
        }

        [Test]
        public void HitTestFreePointTest() {
            var p1 = Point();
            var p2 = Point();
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(1, 2)),
                (p2, new Vector2(3, 4)),
            });
            CollectionAssert.IsEmpty(surface.HitTest(new Vector2(2, 3)));

            surface.Add(p1.AsView(), DisplayStyle.Background);
            surface.Add(p2.AsView(), DisplayStyle.Visible);

            CollectionAssert.AreEqual(new[] { p1, p2 }, surface.HitTest(new Vector2(2, 3)));
            CollectionAssert.AreEqual(new[] { p1, p2 }, surface.HitTest(new Vector2(2 - 0.001f, 3 - 0.001f)));
            CollectionAssert.AreEqual(new[] { p2, p1 }, surface.HitTest(new Vector2(2 + 0.001f, 3 + 0.001f)));

            CollectionAssert.AreEqual(new[] { p1 }, surface.HitTest(new Vector2(1 - 5 + 0.001f, 2)));
            CollectionAssert.IsEmpty(surface.HitTest(new Vector2(1 - 5, 2)));
        }

        [Test]
        public void HitTestIntersectionPointTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var l1 = Line(p1, p2);
            var l2 = Line(p3, p4);
            var i = Intersect(l1, l2);
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(10, 0)),
                (p2, new Vector2(-10, 0)),
                (p3, new Vector2(0, 10)),
                (p4, new Vector2(0, -10)),
            });
            surface.Add(l1, DisplayStyle.Background);
            surface.Add(l2, DisplayStyle.Background);

            CollectionAssert.IsEmpty(surface.HitTest(new Vector2(0, 0)));

            surface.Add(p1.AsView(), DisplayStyle.Background);
            surface.Add(p2.AsView(), DisplayStyle.Visible);
            surface.Add(i.AsView(), DisplayStyle.Visible);
            CollectionAssert.AreEqual(new[] { i }, surface.HitTest(new Vector2(0, 0)));
        }

        [Test]
        public void HitTestLinesIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var p5 = Point();
            var p6 = Point();
            var l1 = Line(p1, p2);
            var l2 = Line(p3, p4);
            var l3 = Line(p5, p6);
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(10, 0)),
                (p2, new Vector2(-10, 0)),
                (p3, new Vector2(0, 10)),
                (p4, new Vector2(0, -10)),
                (p5, new Vector2(3, 10)),
                (p6, new Vector2(3.1f, -10)),
            });
            surface.Add(l1, DisplayStyle.Background);
            surface.Add(l2, DisplayStyle.Background);
            surface.Add(l3, DisplayStyle.Background);

            Assert.AreSame(Intersect(l1, l2), surface.HitTestIntersection(new Vector2(1, -1)));
            Assert.AreSame(Intersect(l1, l3), surface.HitTestIntersection(new Vector2(3, -1)));
            Assert.Null(surface.HitTestIntersection(new Vector2(100, 100)));
        }

        [Test]
        public void HitTestLineSegmentsIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var p5 = Point();
            var p6 = Point();
            var surface = CreateTestSurface();
            var l1 = c.LineSegment(p1, p2).Add(surface);
            var l2 = c.LineSegment(p3, p4).Add(surface);
            var l3 = c.LineSegment(p5, p6).Add(surface);
            surface.SetPoints(new[] {
                (p1, new Vector2(10, 0)),
                (p2, new Vector2(-10, 0)),
                (p3, new Vector2(0, 10)),
                (p4, new Vector2(0, -10)),
                (p5, new Vector2(20, 10)),
                (p6, new Vector2(21, -10)),
            });

            Assert.AreSame(Intersect(l1.Line, l2.Line), surface.HitTestIntersection(new Vector2(1, -1)));
            Assert.Null(surface.HitTestIntersection(new Vector2(10, -1)));
            Assert.Null(surface.HitTestIntersection(new Vector2(20, 0)));
            Assert.Null(surface.HitTestIntersection(new Vector2(100, 100)));
        }

        [Test]
        public void HitTestLineCircleIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            var l = Line(p2, p3).Add(surface);
            var c = Circle(p1, p2).Add(surface);
            surface.SetPoints(new[] {
                (p1, new Vector2(0, 0)),
                (p2, new Vector2(3, 4)),
                (p3, new Vector2(3, -10)),
            });

            Assert.AreSame(p2, surface.HitTestIntersection(new Vector2(3, 4)));
            Assert.AreSame(Intersect(l, c).Point2, surface.HitTestIntersection(new Vector2(3, -4)));
            Assert.Null(surface.HitTestIntersection(new Vector2(100, 100)));

            surface.Remove(l);
            surface.Remove(c);
            c.Add(surface);
            l.Add(surface);

            Assert.AreSame(p2, surface.HitTestIntersection(new Vector2(3, 4)));
            Assert.AreSame(Intersect(l, c).Point2, surface.HitTestIntersection(new Vector2(3, -4)));
            Assert.Null(surface.HitTestIntersection(new Vector2(100, 100)));
        }

        [Test]
        public void HitTestCirclesIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            var c1 = Circle(p1, p3).Add(surface);
            var c2 = Circle(p2, p3).Add(surface);
            surface.SetPoints(new[] {
                (p1, new Vector2(-3, 0)),
                (p2, new Vector2(3, 0)),
                (p3, new Vector2(0, 4)),
            });

            Assert.AreSame(p3, surface.HitTestIntersection(new Vector2(0, 4)));
            Assert.AreSame(Intersect(c1, c2).Point2, surface.HitTestIntersection(new Vector2(0, -4)));
            Assert.Null(surface.HitTestIntersection(new Vector2(100, 100)));
        }

        [Test]
        public void HitTestCircleSegmentsIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);
            surface.SetPoints(new[] {
                (p1, new Vector2(-3, 0)),
                (p2, new Vector2(3, 0)),
                (p3, new Vector2(0, 4)),
            });
            CircleSegment(c1, p3, Intersect(c1, c2).Point2).Add(surface);
            CircleSegment(c2, p3, Intersect(c1, c2).Point2).Add(surface);

            Assert.AreSame(p3, surface.HitTestIntersection(new Vector2(0, 4)));
            Assert.AreSame(Intersect(c1, c2).Point2, surface.HitTestIntersection(new Vector2(0, -4)));
            Assert.Null(surface.HitTestIntersection(new Vector2(100, 100)));
        }

        [Test]
        public void HitTestLinesIntersectionTest_LineAndLineSegment() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var l1 = Line(p1, p2);
            var l2 = Line(p3, p4);
            var surface = CreateTestSurface();
            surface.SetPoints(new[] {
                (p1, new Vector2(10, 0)),
                (p2, new Vector2(-10, 0)),
                (p3, new Vector2(0, 10)),
                (p4, new Vector2(0, -10)),
            });
            surface.Add(l1, DisplayStyle.Background);
            surface.Add(l2, DisplayStyle.Background);
            l1.AsLineSegment().Add(surface);

            Assert.AreSame(Intersect(l1, l2), surface.HitTestIntersection(new Vector2(1, -1)));
            Assert.Null(surface.HitTestIntersection(new Vector2(100, 100)));
        }

        [Test]
        public void HitTestCirclesIntersectionTest_CirleAndCircleSegment() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var surface = CreateTestSurface();
            var c1 = Circle(p1, p3).Add(surface);
            var c2 = Circle(p2, p3).Add(surface);
            c.CircleSegment(c1, c2).Add(surface);
            surface.SetPoints(new[] {
                (p1, new Vector2(-3, 0)),
                (p2, new Vector2(3, 0)),
                (p3, new Vector2(0, 4)),
            });

            Assert.AreSame(p3, surface.HitTestIntersection(new Vector2(0, 4)));
            Assert.AreSame(Intersect(c1, c2).Point2, surface.HitTestIntersection(new Vector2(0, -4)));
            Assert.Null(surface.HitTestIntersection(new Vector2(100, 100)));
        }

    }
}
