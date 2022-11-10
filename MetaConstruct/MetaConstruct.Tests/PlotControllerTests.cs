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

            Assert.False(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanUndo);

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

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p));

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
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

            Assert.False(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
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

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            CollectionAssert.IsEmpty(surface.GetEntities());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
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

            controller.scene.Press(new Vector2(58, 68));
            controller.scene.Drag(new Vector2(100, 200));
            controller.scene.Release(new Vector2(100, 200));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);

            controller.undoManager.Undo();
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p));
            Assert.True(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);

            controller.undoManager.Undo();
            CollectionAssert.IsEmpty(surface.GetEntities());
            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);

            controller.undoManager.Redo();
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p));
            Assert.True(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);

            controller.undoManager.Redo();
            Assert.AreEqual(p.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(100, 200), surface.GetPointLocation(p));
            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
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

        [Test]
        public void LineTool_NewFreeFromPoint_ExistingFreeToPoint_ThenNewFreeToPoint() {
            var p2 = Point();

            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(p2.AsView(), DisplayStyle.Visible);
            surface.SetPointLocation(p2, new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Press(new Vector2(48, 48));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            var p1 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            var (l1, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l1, Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(59, 69));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(l1, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(l1, Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(70, 80));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().ElementAt(1).Entity);
            var p3 = (FreePoint)((PointView)surface.GetEntities().ElementAt(2).Entity).point;
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(70, 80), surface.GetPointLocation(p3));
            var l2 = (Line)surface.GetEntities().Skip(3).Single().Entity;
            Assert.AreEqual(p1, l2.From);
            Assert.AreEqual(p3, l2.To);

            controller.scene.Release(new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(p3.AsView(), surface.GetEntities().ElementAt(2).Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(70, 80), surface.GetPointLocation(p3));
            Assert.AreEqual(l2, (Line)surface.GetEntities().Skip(3).Single().Entity);
        }

        [Test]
        public void LineTool_NewFreeFromPoint_NewFreeToPoint_ThenExistingFreeToPoint_ThenNewFreeToPoint() {
            var p2 = Point();

            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(p2.AsView(), DisplayStyle.Visible);
            surface.SetPointLocation(p2, new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Press(new Vector2(48, 48));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            var p1 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(100, 100));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            var p3 = (FreePoint)((PointView)surface.GetEntities().ElementAt(2).Entity).point;
            Assert.AreEqual(new Vector2(100, 100), surface.GetPointLocation(p3));
            var (l1, style) = surface.GetEntities().Skip(3).Single();
            Assert.AreEqual(l1, Line(p1, p3));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            var l2 = (Line)surface.GetEntities().Skip(2).Single().Entity;
            Assert.AreEqual(p1, l2.From);
            Assert.AreEqual(p2, l2.To);

            controller.scene.Drag(new Vector2(100, 100));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            p3 = (FreePoint)((PointView)surface.GetEntities().ElementAt(2).Entity).point;
            Assert.AreEqual(new Vector2(100, 100), surface.GetPointLocation(p3));
            l1 = surface.GetEntities().Skip(3).Single().Entity;
            Assert.AreEqual(l1, Line(p1, p3));
            Assert.AreEqual(DisplayStyle.Background, style);
        }

        [Test]
        public void LineTool_ExistingIntersectionFromPoint_NewFreeToPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var i = Intersect(Line(p1, p2), Line(p3, p4));
            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(i.AsView(), DisplayStyle.Visible);
            surface.SetPointLocation(p1, new Vector2(-10, 0));
            surface.SetPointLocation(p2, new Vector2(10, 0));
            surface.SetPointLocation(p3, new Vector2(0, -10));
            surface.SetPointLocation(p4, new Vector2(0, 10));
            Assert.AreEqual(i.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Press(new Vector2(0, 0));
            Assert.AreEqual(i.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(i.AsView(), surface.GetEntities().First().Entity);
            var toPoint = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(toPoint));
            var (l, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l, Line(i, toPoint));
            Assert.AreEqual(DisplayStyle.Background, style);
        }

        [Test]
        public void LineTool_NewFreeFromPoint_NewFreeToPoint_DragOverFromPoint() {
            var (controller, surface) = CreateTestController(Tool.Line);
            CollectionAssert.IsEmpty(surface.GetEntities());

            controller.scene.Press(new Vector2(48, 48));
            var p1 = (FreePoint)((PointView)surface.GetEntities().Single().Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(48, 49));
            Assert.AreEqual(p1, (FreePoint)((PointView)surface.GetEntities().Single().Entity).point);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            var p2 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p2));
            var (l, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l, Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);
        }
        #endregion
    }
}
