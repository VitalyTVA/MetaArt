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
    }
}
