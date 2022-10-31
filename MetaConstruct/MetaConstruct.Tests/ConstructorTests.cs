using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
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
        protected CircleIntersection Intersect(Circle c1, Circle c2) => c.Intersect(c1, c2);
        protected CircleIntersection Intersect(Line l, Circle c_) => c.Intersect(l, c_);
        protected Point Intersect(Line l1, Line l2) => c.Intersect(l1, l2);
    }

    [TestFixture]
    public class ConstructorTests : ConstructorTestsBase {
        [Test]
        public void FreePointTest() {
            var p1 = Point();
            var p2 = Point();
            Assert.AreNotEqual(p1.GetHashCodeEx(), p2.GetHashCodeEx());
            Assert.Throws<InvalidOperationException>(() => p1.GetHashCode());
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

            Assert.AreEqual(Intersect(Line(p1, p2), Line(p3, p4)), Intersect(Line(p1, p2), Line(p3, p4)));
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

            Assert.AreEqual(Intersect(Circle(p1, p2), Circle(p3, p4)), Intersect(Circle(p1, p2), Circle(p3, p4)));
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
        public void LineCircleIntersectionTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();

            Assert.AreEqual(Intersect(Line(p1, p2), Circle(p3, p4)), Intersect(Line(p1, p2), Circle(p3, p4)));
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
    }
}
