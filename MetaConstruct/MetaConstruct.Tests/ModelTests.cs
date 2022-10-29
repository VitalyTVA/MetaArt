using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;
using System.Text;
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
        public void LineSegmentTest1() {
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
        public void LineSegmentTest2() {
            var p1 = Point();
            var p2 = Point();
            var c1 = Circle(p1, p2);
            var c2 = Circle(p2, p1);
            var line = Line(p1, p2);
            var intersection = Intersect(c1, c2);
            Assert.Throws<InvalidOperationException>(() => LineSegment(line, p1, intersection.Point1));
        }

        [Test]
        public void LineSegmentTest3() {
            var p1 = Point();
            var p2 = Point();
            var line = Line(p1, p2);
            var c = Circle(Point(), Point());
            var p3 = Intersect(line, c).Point1;
            var segment = LineSegment(line, p1, p3);
            Assert.AreEqual(line, segment.Line);
            Assert.AreEqual(p1, segment.From);
            Assert.AreEqual(p3, segment.To);
            Assert.Throws<InvalidOperationException>(() => LineSegment(line, p1, Intersect(Line(Point(), Point()), c).Point1));
        }

        [Test]
        public void LineSegmentTest4() {
            var p1 = Point();
            var p2 = Point();
            var line = Line(p1, p2);
            var l = Line(Point(), Point());
            var p3 = Intersect(line, l);
            var segment = LineSegment(line, p1, p3);
            Assert.AreEqual(line, segment.Line);
            Assert.AreEqual(p1, segment.From);
            Assert.AreEqual(p3, segment.To);
            Assert.Throws<InvalidOperationException>(() => LineSegment(line, p1, Intersect(Line(Point(), Point()), l)));
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

        [Test]
        public void CircleSegmentFromCirclesIntersection() {
            var c1 = Circle(Point(), Point());
            var c2 = Circle(Point(), Point());
            var intersection = Intersect(c1, c2);
            var segment = CircleSegment(c1, c2);
            Assert.AreEqual(c1, segment.Circle);
            Assert.AreEqual(intersection.Point1, segment.From);
            Assert.AreEqual(intersection.Point2, segment.To);

            segment = segment.Invert();
            Assert.AreEqual(c1, segment.Circle);
            Assert.AreEqual(intersection.Point1, segment.To);
            Assert.AreEqual(intersection.Point2, segment.From);
        }

        [Test]
        public void CircleSegmentFromLineCircleIntersection() {
            var l = Line(Point(), Point());
            var c = Circle(Point(), Point());
            var intersection = Intersect(l, c);
            var segment = CircleSegment(c, l);
            Assert.AreEqual(c, segment.Circle);
            Assert.AreEqual(intersection.Point1, segment.From);
            Assert.AreEqual(intersection.Point2, segment.To);
        }

        [Test]
        public void LineSegmentFromLineCircleIntersection() {
            var l = Line(Point(), Point());
            var c = Circle(Point(), Point());
            var intersection = Intersect(l, c);
            var segment = LineSegment(l, c);
            Assert.AreEqual(l, segment.Line);
            Assert.AreEqual(intersection.Point1, segment.From);
            Assert.AreEqual(intersection.Point2, segment.To);
        }

        [Test]
        public void LineSegmentFromLineCircleIntersectionWithCommonPoint() {
            var p = Point();
            var l = Line(Point(), p);
            var c = Circle(Point(), p);
            var intersection = Intersect(l, c);
            var segment = LineSegment(l, c);
            Assert.AreEqual(l, segment.Line);
            Assert.AreEqual(intersection.Point1, segment.From);
            Assert.AreEqual(intersection.Point2, segment.To);

            var p2 = Point();
            var intersection2 = Intersect(Line(Point(), p2), Circle(Point(), p2));
            Assert.Throws<InvalidOperationException>(() => LineSegment(l, p, intersection2.Point2));
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

            var s1 = CircleSegment(c1, c2);
            Assert.AreEqual(p3, s1.From);
            Assert.AreNotEqual(p3, s1.To);

            var s2 = CircleSegment(c2, c1);
            Assert.AreEqual(p3, s2.From);
            Assert.AreNotEqual(p3, s2.To);

            var s3 = CircleSegment(c1, Intersect(c1, Circle(Point(), Point())).Point1, Intersect(c1, Circle(Point(), p3)).Point2);
            var s4 = CircleSegment(c1, Intersect(c1, Circle(Point(), Point())).Point1, Intersect(Circle(Point(), p3), c1).Point2);

            Assert.Throws<InvalidOperationException>(() => CircleSegment(c1, p3, Intersect(c2, Circle(Point(), p3)).Point2));
            Assert.Throws<InvalidOperationException>(() => CircleSegment(c1, Intersect(c2, Circle(Point(), p3)).Point2, p3));
            Assert.Throws<InvalidOperationException>(() => CircleSegment(c1, p3, Intersect(Circle(Point(), p3), c2).Point2));
            Assert.Throws<InvalidOperationException>(() => CircleSegment(c1, Intersect(Circle(Point(), p3), c2).Point2, p3));
            Assert.Throws<InvalidOperationException>(() => CircleSegment(c1, p3, Intersect(Circle(Point(), p3), Circle(Point(), p3)).Point2));
        }

        [Test]
        public void LineCircleIntersection_CommonPointTest() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();

            var l = Line(p1, p3);
            var c = Circle(p2, p3);

            var s1 = CircleSegment(c, l);
            Assert.AreEqual(p3, s1.From);
            Assert.AreNotEqual(p3, s1.To);

            var s3 = CircleSegment(c, Intersect(Line(Point(), Point()), c).Point1, Intersect(Line(Point(), p3), c).Point2);
            var s4 = CircleSegment(c, Intersect(Line(Point(), Point()), c).Point1, Intersect(Line(Point(), p3), c).Point2);

            var intersection = Intersect(l, c);
            Assert.Throws<InvalidOperationException>(() => CircleSegment(c, intersection.Point1, Intersect(l, Circle(Point(), Point())).Point1));
        }
    }
}
