using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;

namespace MetaContruct.Tests {
    [TestFixture]
    public class ModelTests {
        [Test]
        public void FreePointTest() {
            var p1 = new FreePoint();
            var p2 = new FreePoint();
            Assert.AreNotEqual(p1.GetHashCodeEx(), p2.GetHashCodeEx());
            Assert.Throws<InvalidOperationException>(() => p1.GetHashCode());
        }

        [Test]
        public void LineSegmentTest() {
            var p1 = new FreePoint();
            var p2 = new FreePoint();
            var line = new Line(p1, p2);
            new LineSegment(line, p1, p2);
            Assert.Throws<InvalidOperationException>(() => new LineSegment(line, p1, new FreePoint()));
            Assert.Throws<InvalidOperationException>(() => new LineSegment(line, new FreePoint(), p2));
        }
    }
}
