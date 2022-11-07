using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;
using static MetaConstruct.ConstructorHelper;

namespace MetaContruct.Tests {
    [TestFixture]
    public class PlotControllerTests : ConstructorTestsBase {
        #region point
        [Test]
        public void PointTool_MoveExistingPoint() {
            var (controller, surface) = CreateTestController(Tool.Point);
            var p = Point();
            surface.SetPoints(new[] {
                (p, new Vector2(50, 50)),
            });
            surface.Add(p.AsView(), DisplayStyle.Background);

            controller.scene.Press(new Vector2(48, 48));
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p));

            controller.scene.Release(new Vector2(58, 68));
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p));
        }

        [Test]
        public void PointTool_ClickExistingPoint() {
            var (controller, surface) = CreateTestController(Tool.Point);
            var p = Point();
            surface.SetPoints(new[] {
                (p, new Vector2(50, 50)),
            });
            surface.Add(p.AsView(), DisplayStyle.Background);

            controller.scene.Press(new Vector2(48, 48));
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p));

            controller.scene.Drag(new Vector2(48, 48));
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p));

            controller.scene.Release(new Vector2(48, 48));
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p));
        }

        [Test]
        public void PointTool_ClickAndCreateNewPoint() {
            var (controller, surface) = CreateTestController(Tool.Point);
            CollectionAssert.IsEmpty(surface.GetEntities());

            controller.scene.Press(new Vector2(48, 48));
            var p = (FreePoint)((PointView)surface.GetEntities().Single().Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p));

            controller.scene.Release(new Vector2(48, 48));
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p));
        }

        [Test]
        public void PointTool_ClickDragNewPoint() {
            var (controller, surface) = CreateTestController(Tool.Point);
            CollectionAssert.IsEmpty(surface.GetEntities());

            controller.scene.Press(new Vector2(48, 48));
            var p = (FreePoint)((PointView)surface.GetEntities().Single().Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p));

            controller.scene.Release(new Vector2(58, 68));
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p));
        }
        #endregion

        #region line
        [Test]
        public void LineTool_NewFreeFromPoint_NewFreeToPoint() {
            var (controller, surface) = CreateTestController(Tool.Line);
            CollectionAssert.IsEmpty(surface.GetEntities());

            controller.scene.Press(new Vector2(48, 48));
            var p1 = (FreePoint)((PointView)surface.GetEntities().Single().Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            var p2 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p2));

            var (l, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l, Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(60, 70));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(l, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            controller.scene.Release(new Vector2(60, 70));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(l, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
        }

        [Test]
        public void LineTool_ExistingFreeFromPoint_NewFreeToPoint() {
            var p1 = Point();
            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(p1.AsView(), DisplayStyle.Visible);
            surface.SetPointLocation(p1, new Vector2(50, 50));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Press(new Vector2(48, 48));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));

            var p2 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p2));

            var (l, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l, Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(60, 70));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(l, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            controller.scene.Release(new Vector2(60, 70));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(l, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
        }
        #endregion
    }
}
