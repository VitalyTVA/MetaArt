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
        public void CirclesIntersetion() {
            var (p1, p2) = ConstructHelper.GetCircleIntersections(new Vector2(1, 2), new Vector2(5, 6), 3, 4)!.Value;
            Assert.AreEqual(new Vector2(3.9972801f, 2.1277199f), p1);
            Assert.AreEqual(new Vector2(1.1277198f, 4.99728f), p2);

            (p1, p2) = ConstructHelper.GetCircleIntersections(new Vector2(5, 6), new Vector2(1, 2), 4, 3)!.Value;
            Assert.AreEqual(new Vector2(3.9972801f, 2.1277199f), p2);
            Assert.AreEqual(new Vector2(1.1277198f, 4.99728f), p1);

            (p1, p2) = ConstructHelper.GetCircleIntersections(new Vector2(-3, 0), new Vector2(2, 0), 2, 3)!.Value;
            Assert.AreEqual(new Vector2(-1, 0), p1);
            Assert.AreEqual(new Vector2(-1, 0), p2);

            Assert.Null(ConstructHelper.GetCircleIntersections(new Vector2(-3, 0), new Vector2(2.001f, 0), 2, 3));

            (p1, p2) = ConstructHelper.GetCircleIntersections(new Vector2(0, 0), new Vector2(2, 0), 3, 1)!.Value;
            Assert.AreEqual(new Vector2(3, 0), p1);
            Assert.AreEqual(new Vector2(3, 0), p2);

            Assert.Null(ConstructHelper.GetCircleIntersections(new Vector2(0, 0), new Vector2(2, 0), 3.001f, 1));

        }
    }
}
