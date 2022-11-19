using MetaConstruct;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MetaContruct.Tests {
    [TestFixture]
    public class ConstructHelperTests {
        [Test]
        public void CirclesIntersetions() {
            var (p1, p2) = ConstructHelper.GetCirclesIntersections(new Vector2(1, 2), new Vector2(5, 6), 3, 4)!.Value;
            Assert.AreEqual(new Vector2(3.9972801f, 2.1277199f), p1);
            Assert.AreEqual(new Vector2(1.1277198f, 4.99728f), p2);

            (p1, p2) = ConstructHelper.GetCirclesIntersections(new Vector2(5, 6), new Vector2(1, 2), 4, 3)!.Value;
            Assert.AreEqual(new Vector2(3.9972801f, 2.1277199f), p2);
            Assert.AreEqual(new Vector2(1.1277198f, 4.99728f), p1);

            (p1, p2) = ConstructHelper.GetCirclesIntersections(new Vector2(-3, 0), new Vector2(2, 0), 2, 3)!.Value;
            Assert.AreEqual(new Vector2(-1, 0), p1);
            Assert.AreEqual(new Vector2(-1, 0), p2);

            Assert.Null(ConstructHelper.GetCirclesIntersections(new Vector2(-3, 0), new Vector2(2.001f, 0), 2, 3));

            (p1, p2) = ConstructHelper.GetCirclesIntersections(new Vector2(0, 0), new Vector2(2, 0), 3, 1)!.Value;
            Assert.AreEqual(new Vector2(3, 0), p1);
            Assert.AreEqual(new Vector2(3, 0), p2);

            Assert.Null(ConstructHelper.GetCirclesIntersections(new Vector2(0, 0), new Vector2(2, 0), 3.001f, 1));
        }

        [Test]
        public void LinesIntersetion() {
            var p = ConstructHelper.GetLinesIntersection(new Vector2(1, 2), new Vector2(5, 6), new Vector2(-3, 2), new Vector2(4, -5));
            Assert.AreEqual(new Vector2(-1, 0), p);

            Assert.Null(ConstructHelper.GetLinesIntersection(new Vector2(1, 2), new Vector2(5, 6), new Vector2(2, 2), new Vector2(6, 6)));
        }

        [Test]
        public void LineCircleIntersetions() {
            var (p1, p2) = ConstructHelper.GetLineCircleIntersections(new Vector2(4, 5), 6, new Vector2(1, 2), new Vector2(2, 1))!.Value;
            Assert.AreEqual(new Vector2(4, -1), p1);
            Assert.AreEqual(new Vector2(-2, 5), p2);

            (p1, p2) = ConstructHelper.GetLineCircleIntersections(new Vector2(4, 5), 6, new Vector2(-2, 0), new Vector2(-2, 1))!.Value;
            Assert.AreEqual(new Vector2(-2, 5), p1);
            Assert.AreEqual(new Vector2(-2, 5), p2);

            Assert.Null(ConstructHelper.GetLineCircleIntersections(new Vector2(4, 5), 6, new Vector2(-2.001f, 0), new Vector2(-2.001f, 1)));

            Assert.Null(ConstructHelper.GetLineCircleIntersections(new Vector2(4, 5), 6, new Vector2(2, 1), new Vector2(2, 1)));
        }

        [Test]
        public void AngleBetween() {
            Assert.AreEqual(MathF.PI / 4, ConstructHelper.AngleTo(new Vector2(3, 3)));
            Assert.AreEqual(3 * MathF.PI / 4, ConstructHelper.AngleTo(new Vector2(-3, 3)));
            Assert.AreEqual(5 * MathF.PI / 4, ConstructHelper.AngleTo(new Vector2(-3, -3)));
            Assert.AreEqual(7 * MathF.PI / 4, ConstructHelper.AngleTo(new Vector2(3, -3)));
        }

        [Test]
        public void DistanceToLine() {
            Assert.AreEqual(3.1622777f, ConstructHelper.DistanceToLine(new Vector2(1, 2), new Vector2(3, 6), new Vector2(5, 0)));
            Assert.AreEqual(0, ConstructHelper.DistanceToLine(new Vector2(4, 3), new Vector2(3, 6), new Vector2(5, 0)));
        }

        [Test]
        public void DistanceToCircle() {
            Assert.AreEqual(2.47213602f, ConstructHelper.DistanceToCircle(new Vector2(8, 5), new Vector2(4, 3), 2));
            Assert.AreEqual(0, ConstructHelper.DistanceToCircle(new Vector2(4, 5), new Vector2(4, 3), 2));
            Assert.AreEqual(0.585786462f, ConstructHelper.DistanceToCircle(new Vector2(5, 4), new Vector2(4, 3), 2));
        }

    }
}
