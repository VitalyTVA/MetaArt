using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;
using static MetaConstruct.ConstructorHelper;

namespace MetaContruct.Tests {
    public abstract class ConstructorTestsBase {
        protected Constructor c = null!;
        [SetUp]
        public void SetUp() {
            c = new Constructor();
        }

        protected FreePoint Point() => c.Point();
        protected Line Line(Point from, Point to) => c.Line(from, to);
        protected Circle Circle(Point center, Point point) => c.Circle(center, point);
        protected Circle Circle(Point center, Point radius1, Point radius2) => c.Circle(center, radius1, radius2);
        protected CircleIntersection Intersect(Circle c1, Circle c2) => c.Intersect(c1, c2);
        protected CircleIntersection Intersect(Line l, Circle c_) => c.Intersect(l, c_);
        protected Point Intersect(Line l1, Line l2) => c.Intersect(l1, l2);

        protected Surface CreateTestSurface() {
            return new Surface(c, 5);
        }
        protected Calculator CreateCalculator((FreePoint, Vector2)[] points) {
            var surface = CreateTestSurface();
            surface.SetPoints(points);
            return surface.CreateCalculator();
        }
    }

    [TestFixture]
    public class ConstructorTests : ConstructorTestsBase {
        [Test]
        public void FreePointTest() {
            var p1 = Point();
            var p2 = Point();
            Assert.AreEqual("P0", p1.Id);
            Assert.AreEqual("P1", p2.Id);
            Assert.AreNotEqual(p1.GetHashCode(), p2.GetHashCode());
        }

        [Test]
        public void LineFromCirclesIntersection() {
            var c1 = Circle(Point(), Point());
            var c2 = Circle(Point(), Point());
            var intersection = Intersect(c1, c2);
            var line = c.AsLine(intersection);
            Assert.AreEqual(intersection.Point1, line.From);
            Assert.AreEqual(intersection.Point2, line.To);
        }
        [Test]
        public void LinesIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();

            Point i1 = Intersect(Line(p1, p2), Line(p3, p4));
            Point i2 = Intersect(Line(p1, p2), Line(p3, p4));
            Assert.AreSame(i1, i2);
            Assert.AreNotEqual(Intersect(Line(p1, p2), Line(p3, p4)), Intersect(Line(p1, p2), Line(p3, Point())));

            var i = Intersect(Line(p1, p2), Line(p3, p4));
            Assert.AreNotEqual(p1, i);
            Assert.AreNotEqual(p2, i);
            Assert.AreNotEqual(p3, i);
            Assert.AreNotEqual(p4, i);

            Assert.AreEqual(p1, Intersect(Line(p1, p2), Line(p1, p3)));
            Assert.AreEqual(p2, Intersect(Line(p1, p2), Line(p2, p3)));
            Assert.AreEqual(p3, Intersect(Line(p1, p3), Line(p2, p3)));
            Assert.AreEqual(p2, Intersect(Line(p2, p1), Line(p3, p2)));

            Assert.Throws<InvalidOperationException>(() => Intersect(Line(p1, p2), Line(p1, p2)));
        }

        [Test]
        public void CirclesIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();

            var i1 = Intersect(Circle(p1, p2), Circle(p3, p4));
            var i2 = Intersect(Circle(p1, p2), Circle(p3, p4));
            Assert.AreSame(i1.Point1, i2.Point1);
            Assert.AreSame(i1.Point2, i2.Point2);
            Assert.AreSame(i1, i2);
            Assert.AreNotEqual(Intersect(Circle(p1, p2), Circle(p3, p4)), Intersect(Circle(p1, p2), Circle(p3, Point())));

            var i = Intersect(Circle(p1, p2), Circle(p1, p4));
            Assert.AreNotEqual(p1, i.Point1);
            Assert.AreNotEqual(p2, i.Point1);
            Assert.AreNotEqual(p4, i.Point1);
            Assert.AreNotEqual(p1, i.Point2);
            Assert.AreNotEqual(p2, i.Point2);
            Assert.AreNotEqual(p4, i.Point2);

            Assert.AreEqual(p3, Intersect(Circle(p1, p3), Circle(p2, p3)).Point1);
            Assert.AreNotEqual(p3, Intersect(Circle(p1, p3), Circle(p2, p3)).Point2);

            Assert.Throws<InvalidOperationException>(() => Intersect(Circle(p1, p2), Circle(p1, p2)));
        }

        [Test]
        public void CirclesIntersection_WithCommonPointTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var i1 = Intersect(Circle(p1, p2), Circle(p3, p2));
            var i2 = Intersect(Circle(p1, p2), Circle(p3, p2));
            Assert.AreSame(i1.Point1, i2.Point1);
            Assert.AreSame(i1.Point2, i2.Point2);
            Assert.AreSame(i1, i2);
            Assert.AreNotEqual(Intersect(Circle(p1, p2), Circle(p3, p2)), Intersect(Circle(p1, p2), Circle(p3, Point())));
        }

        [Test]
        public void LineCircleIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();

            var i1 = Intersect(Line(p1, p2), Circle(p3, p4));
            var i2 = Intersect(Line(p1, p2), Circle(p3, p4));
            Assert.AreSame(i1.Point1, i2.Point1);
            Assert.AreSame(i1.Point2, i2.Point2);
            Assert.AreSame(i1, i2);
            Assert.AreNotEqual(Intersect(Line(p1, p2), Circle(p3, p4)), Intersect(Line(p1, p2), Circle(p3, Point())));

            var i = Intersect(Line(p1, p2), Circle(p3, p4));
            Assert.AreNotEqual(p1, i.Point1);
            Assert.AreNotEqual(p2, i.Point1);
            Assert.AreNotEqual(p3, i.Point1);
            Assert.AreNotEqual(p4, i.Point1);
            Assert.AreNotEqual(p1, i.Point2);
            Assert.AreNotEqual(p2, i.Point2);
            Assert.AreNotEqual(p3, i.Point2);
            Assert.AreNotEqual(p4, i.Point2);

            Assert.AreEqual(p2, Intersect(Line(p1, p2), Circle(p1, p2)).Point1);
            Assert.AreNotEqual(p2, Intersect(Line(p1, p2), Circle(p1, p2)).Point2);
            Assert.AreEqual(p2, Intersect(Line(p2, p1), Circle(p1, p2)).Point1);
            Assert.AreNotEqual(p2, Intersect(Line(p2, p1), Circle(p1, p2)).Point2);
        }

        [Test]
        public void LineCircleIntersection_WithCommonPointTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var i1 = Intersect(Line(p1, p2), Circle(p3, p2));
            var i2 = Intersect(Line(p1, p2), Circle(p3, p2));
            Assert.AreSame(i1.Point1, i2.Point1);
            Assert.AreSame(i1.Point2, i2.Point2);
            Assert.AreSame(i1, i2);
        }

        [Test]
        public void LineTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            Assert.AreEqual(Line(p1, p2), Line(p1, p2));
            Assert.AreNotEqual(Line(p1, p2), Line(p1, p3));
        }

        [Test]
        public void CirclesIntersection_CommonPointTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);

            var i1 = Intersect(c1, c2);
            Assert.AreEqual(p3, i1.Point1);
            Assert.AreNotEqual(p3, i1.Point2);

            var i2 = Intersect(c2, c1);
            Assert.AreEqual(p3, i2.Point1);
            Assert.AreNotEqual(p3, i2.Point2);
        }

        [Test]
        public void CirclesIntersection_CommonPointTest_NonSipleCircle1() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var c1 = Circle(p1, Point(), p3);
            var c2 = Circle(p2, p3);

            var i1 = Intersect(c1, c2);
            Assert.AreNotEqual(p3, i1.Point1);
            Assert.AreNotEqual(p3, i1.Point2);

            var i2 = Intersect(c2, c1);
            Assert.AreNotEqual(p3, i2.Point1);
            Assert.AreNotEqual(p3, i2.Point2);
        }

        [Test]
        public void CirclesIntersection_CommonPointTest_NonSipleCircle2() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var c1 = Circle(p1, p3, Point());
            var c2 = Circle(p2, p3);

            var i1 = Intersect(c1, c2);
            Assert.AreNotEqual(p3, i1.Point1);
            Assert.AreNotEqual(p3, i1.Point2);

            var i2 = Intersect(c2, c1);
            Assert.AreNotEqual(p3, i2.Point1);
            Assert.AreNotEqual(p3, i2.Point2);
        }

        [Test]
        public void CirclesIntersection_CommonPointTest_NonSipleCircle3() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var c1 = Circle(p1, Point(), p3);
            var c2 = Circle(p2, Point(), p3);

            var i1 = Intersect(c1, c2);
            Assert.AreNotEqual(p3, i1.Point1);
            Assert.AreNotEqual(p3, i1.Point2);

            var i2 = Intersect(c2, c1);
            Assert.AreNotEqual(p3, i2.Point1);
            Assert.AreNotEqual(p3, i2.Point2);
        }

        [Test]
        public void LineCircleIntersection_CommonPointTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var l = Line(p1, p3);
            var c = Circle(p2, p3);

            var s1 = Intersect(l, c);
            Assert.AreEqual(p3, s1.Point1);
            Assert.AreNotEqual(p3, s1.Point2);
        }

        [Test]
        public void LineCircleIntersection_CommonPointTest_NonSimpleCircle1() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var l = Line(p1, p3);
            var c = Circle(p2, Point(), p3);

            var s1 = Intersect(l, c);
            Assert.AreNotEqual(p3, s1.Point1);
            Assert.AreNotEqual(p3, s1.Point2);
        }

        [Test]
        public void LineCircleIntersection_CommonPointTest_NonSimpleCircle2() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var l = Line(p1, p3);
            var c = Circle(p2, p3, Point());

            var s1 = Intersect(l, c);
            Assert.AreNotEqual(p3, s1.Point1);
            Assert.AreNotEqual(p3, s1.Point2);
        }

        [Test]
        public void FreePointReferenceEqualityTest() {
            Assert.AreNotEqual(new FreePoint("P0"), new FreePoint("P0"));
        }
        [Test]
        public void CirclesPointReferenceEqualityTest() {
            var c1 = Circle(Point(), Point());
            var c2 = Circle(Point(), Point());
            Assert.AreNotEqual(new CircleCirclePoint(c1, c2, CircleIntersectionKind.First), new CircleCirclePoint(c1, c2, CircleIntersectionKind.First));
        }
        [Test]
        public void LineCirclePointReferenceEqualityTest() {
            var c = Circle(Point(), Point());
            var l = Line(Point(), Point());
            Assert.AreNotEqual(new LineCirclePoint(l, c, CircleIntersectionKind.First), new LineCirclePoint(l, c, CircleIntersectionKind.First));
        }
        [Test]
        public void LinesPointReferenceEqualityTest() {
            var l1 = Line(Point(), Point());
            var l2 = Line(Point(), Point());
            Assert.AreNotEqual(new LineLinePoint(l1, l2), new LineLinePoint(l1, l2));
        }

        [Test]
        public void GetPointOnCircleTest1() {
            var c = Point();
            var p = Point();
            var c1 = Circle(c, p);
            Assert.True(c1.IsSimpleCircle);
            Assert.True(c1.VerifySimple());
            Assert.AreSame(p, c1.Radius2);
            Assert.AreSame(p, c1.GetPointOnCircle());
            Assert.AreSame(p, c1.TryGetPointOnCircle());

            var c2 = Circle(c, Point(), p);
            Assert.False(c2.IsSimpleCircle);
            Assert.AreSame(p, c2.Radius2);
            Assert.Throws<InvalidOperationException>(() => c2.GetPointOnCircle());
            Assert.Throws<InvalidOperationException>(() => c2.VerifySimple());
            Assert.AreSame(null, c2.TryGetPointOnCircle());
        }

        [Test]
        public void GetPointOnCircleTest2() {
            var c = Point();
            var p = Point();
            var c1 = Circle(c, p, c);
            Assert.True(c1.IsSimpleCircle);
            Assert.True(c1.VerifySimple());
            Assert.AreSame(p, c1.Radius1);
            Assert.AreSame(p, c1.GetPointOnCircle());
            Assert.AreSame(p, c1.TryGetPointOnCircle());

            var c2 = Circle(c, p, Point());
            Assert.False(c2.IsSimpleCircle);
            Assert.AreSame(p, c2.Radius1);
            Assert.Throws<InvalidOperationException>(() => c2.GetPointOnCircle());
            Assert.Throws<InvalidOperationException>(() => c2.VerifySimple());
            Assert.AreSame(null, c2.TryGetPointOnCircle());
        }
    }
}
