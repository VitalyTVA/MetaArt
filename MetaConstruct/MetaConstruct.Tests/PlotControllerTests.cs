using MetaConstruct;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;
using static MetaConstruct.ConstructorHelper;

namespace MetaContruct.Tests {
    [TestFixture]
    public class PlotControllerTests : ConstructorTestsBase {
        [Test]
        public void PointTool_MoveExistingPoint() {
            var (controller, surface) = CreateTestController();
            controller.SetTool(Tool.Point);
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
            var (controller, surface) = CreateTestController();
            controller.SetTool(Tool.Point);
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
            var (controller, surface) = CreateTestController();
            controller.SetTool(Tool.Point);
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
            var (controller, surface) = CreateTestController();
            controller.SetTool(Tool.Point);
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
    }
}
