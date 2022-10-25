using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
using static MetaConstruct.Constructor;

namespace MetaContruct.Tests {
    [TestFixture]
    public class ModelTests {
        [Test]
        public void FreePointTest() {
            var p1 = Point();
            var p2 = Point();
            Assert.AreNotEqual(p1.GetHashCodeEx(), p2.GetHashCodeEx());
            Assert.Throws<InvalidOperationException>(() => p1.GetHashCode());
        }

        [Test]
        public void LineSegmentTest() {
            var p1 = Point();
            var p2 = Point();
            var line = Line(p1, p2);
            var segment = LineSegment(line, p1, p2);
            Assert.AreEqual(line, segment.Line);
            Assert.AreEqual(p1, segment.From);
            Assert.AreEqual(p2, segment.To);
            Assert.Throws<InvalidOperationException>(() => LineSegment(line, p1, new FreePoint()));
            Assert.Throws<InvalidOperationException>(() => LineSegment(line, new FreePoint(), p2));
        }

        [Test]
        public void CircleSegmentTest1() {
            var p1 = Point();
            var p2 = Point();
            var c1 = Circle(p1, p2);
            var c2 = Circle(p2, p1);
            var intersection = Intersect(c1, c2);
            var segment1 = CircleSegment(c1, intersection.Point1, intersection.Point2);
            Assert.AreEqual(c1, segment1.Circle);
            Assert.AreEqual(intersection.Point1, segment1.From);
            Assert.AreEqual(intersection.Point2, segment1.To);

            var segment2 = CircleSegment(c2, intersection.Point1, intersection.Point2);
            Assert.AreEqual(c2, segment2.Circle);
            Assert.AreEqual(intersection.Point1, segment2.From);
            Assert.AreEqual(intersection.Point2, segment2.To);

            Assert.Throws<InvalidOperationException>(() => CircleSegment(c1, intersection.Point1, p1));
            Assert.Throws<InvalidOperationException>(() => CircleSegment(c1, p1, intersection.Point1));

            Assert.Throws<InvalidOperationException>(() => CircleSegment(c1, intersection.Point1, Intersect(Circle(Point(), Point()), Circle(Point(), Point())).Point1));
            Assert.Throws<InvalidOperationException>(() => CircleSegment(c1, Intersect(c2, c1).Point1, Intersect(Circle(Point(), Point()), Circle(Point(), Point())).Point1));
        }

        [Test]
        public void CircleSegmentTest2() {
            var c = Circle(Point(), Point());
            var l = Line(Point(), Point());
            var intersection = Intersect(l, c);
            var segment = CircleSegment(c, intersection.Point1, intersection.Point2);
            Assert.AreEqual(c, segment.Circle);
            Assert.AreEqual(intersection.Point1, segment.From);
            Assert.AreEqual(intersection.Point2, segment.To);

            Assert.Throws<InvalidOperationException>(() => CircleSegment(c, intersection.Point1, Intersect(l, Circle(Point(), Point())).Point1));
            Assert.Throws<InvalidOperationException>(() => CircleSegment(c, intersection.Point1, Intersect(l, Line(Point(), Point()))));
        }

        [Test]
        public void CircleSegmentTest3() {
            var p1 = Point();
            var p2 = Point();
            var c1 = Circle(p1, p2);
            var c2 = Circle(p2, p1);
            var intersection = Intersect(c1, c2);
            var segment = CircleSegment(c1, p2, intersection.Point2);
            Assert.AreEqual(c1, segment.Circle);
            Assert.AreEqual(p2, segment.From);
            Assert.AreEqual(intersection.Point2, segment.To);
        }
    }
}
