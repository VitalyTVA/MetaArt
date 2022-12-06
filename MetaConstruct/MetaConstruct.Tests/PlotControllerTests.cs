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
            surface.Add(p, DisplayStyle.Background);

            controller.scene.Press(new Vector2(48, 48));
            Assert.False(controller.undoManager.CanUndo);
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
            surface.Add(p, DisplayStyle.Background);

            controller.scene.Press(new Vector2(48, 48));
            Assert.False(controller.undoManager.CanUndo);
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
        public void PointTool_ClickAndCreateNewFreePoint() {
            var (controller, surface) = CreateTestController(Tool.Point);
            CollectionAssert.IsEmpty(surface.GetEntities());

            controller.scene.Press(new Vector2(48, 48));
            Assert.True(controller.undoManager.CanUndo);
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
            Assert.True(controller.undoManager.CanUndo);
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

        [Test]
        public void PointTool_ClickAndCreateNewLinesIntersectionPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var (controller, surface) = CreateTestController(Tool.Point);
            surface.SetPointLocation(p1, new Vector2(1, 2));
            surface.SetPointLocation(p2, new Vector2(7, 4));
            surface.SetPointLocation(p3, new Vector2(5, 0));
            surface.SetPointLocation(p4, new Vector2(3, 6));
            var l1 = Line(p1, p2).Add(surface);
            var l2 = Line(p3, p4).Add(surface);

            Assert.AreEqual(2, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(3.8f, 3.1f));
            Assert.True(controller.undoManager.CanUndo);
            var p = (LineLinePoint)((PointView)surface.GetEntities().Skip(2).Single().Entity).point;
            Assert.AreSame(l1, p.Line1);
            Assert.AreSame(l2, p.Line2);

            controller.scene.Release(new Vector2(4.1f, 2.9f));
            Assert.AreSame(p, (LineLinePoint)((PointView)surface.GetEntities().Skip(2).Single().Entity).point);
            Assert.AreSame(l1, p.Line1);
            Assert.AreSame(l2, p.Line2);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(2, surface.GetEntities().Count());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            p = (LineLinePoint)((PointView)surface.GetEntities().Skip(2).Single().Entity).point;
            Assert.AreSame(l1, p.Line1);
            Assert.AreSame(l2, p.Line2);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void PointTool_ClickAndCreateExistingLinesIntersectionPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var (controller, surface) = CreateTestController(Tool.Point);
            surface.SetPointLocation(p1, new Vector2(10, 0));
            surface.SetPointLocation(p2, new Vector2(0, 10));
            surface.SetPointLocation(p3, new Vector2(0, 0));
            var l1 = Line(p1, p3).Add(surface);
            var l2 = Line(p2, p3).Add(surface);

            Assert.AreEqual(2, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 0));
            Assert.True(controller.undoManager.CanUndo);
            var p = (FreePoint)((PointView)surface.GetEntities().Skip(2).Single().Entity).point;
            Assert.AreSame(p3, p);
            Assert.AreEqual(new Vector2(0, 0), surface.GetPointLocation(p3));

            controller.scene.Release(new Vector2(4.1f, 2.9f));
            Assert.AreSame(p, (FreePoint)((PointView)surface.GetEntities().Skip(2).Single().Entity).point);
            Assert.AreSame(p3, p);
            Assert.AreEqual(new Vector2(0, 0), surface.GetPointLocation(p3));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(2, surface.GetEntities().Count());
            Assert.AreEqual(new Vector2(0, 0), surface.GetPointLocation(p3));

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            p = (FreePoint)((PointView)surface.GetEntities().Skip(2).Single().Entity).point;
            Assert.AreSame(p3, p);
            Assert.AreEqual(new Vector2(0, 0), surface.GetPointLocation(p3));

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
            Assert.True(controller.undoManager.CanUndo);
            var p1 = (FreePoint)((PointView)surface.GetEntities().Single().Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            var p2 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p2));

            var (l, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l.ToLine(), Line(p1, p2));
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

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            CollectionAssert.IsEmpty(surface.GetEntities());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(l, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(DisplayStyle.Background, surface.GetEntities().Skip(2).Single().Style);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_ExistingFreeFromPoint_NewFreeToPoint() {
            var p1 = Point();
            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(p1, DisplayStyle.Visible);
            surface.SetPointLocation(p1, new Vector2(50, 50));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Press(new Vector2(48, 48));
            Assert.False(controller.undoManager.CanUndo);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));

            var p2 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p2));

            var (l, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l.ToLine(), Line(p1, p2));
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

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().Single().Entity);

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(l, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_NewFreeFromPoint_ExistingFreeToPoint_ThenNewFreeToPoint() {
            var p2 = Point();

            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(p2, DisplayStyle.Visible);
            surface.SetPointLocation(p2, new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Press(new Vector2(48, 48));
            Assert.True(controller.undoManager.CanUndo);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            var p1 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            var (l1, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l1.ToLine(), Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(59, 69));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(l1, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(l1.ToLine(), Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(70, 80));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().ElementAt(1).Entity);
            var p3 = (FreePoint)((PointView)surface.GetEntities().ElementAt(2).Entity).point;
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(70, 80), surface.GetPointLocation(p3));
            var l2 = surface.GetEntities().Skip(3).Single().Entity.ToLine();
            Assert.AreEqual(p1, l2.From);
            Assert.AreEqual(p3, l2.To);

            controller.scene.Release(new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(p3.AsView(), surface.GetEntities().ElementAt(2).Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(70, 80), surface.GetPointLocation(p3));
            Assert.AreEqual(l2, surface.GetEntities().Skip(3).Single().Entity.ToLine());

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);

            controller.undoManager.Undo();
            surface.SetPointLocation(p2, new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().Single().Entity);
            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);

            controller.undoManager.Redo();
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(p3.AsView(), surface.GetEntities().ElementAt(2).Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(70, 80), surface.GetPointLocation(p3));
            Assert.AreEqual(l2, surface.GetEntities().Skip(3).Single().Entity.ToLine());
            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_NewFreeFromPoint_NewFreeToPoint_ThenExistingFreeToPoint_ThenNewFreeToPoint() {
            var p2 = Point();

            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(p2, DisplayStyle.Visible);
            surface.SetPointLocation(p2, new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Press(new Vector2(48, 48));
            Assert.True(controller.undoManager.CanUndo);
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
            Assert.AreEqual(l1.ToLine(), Line(p1, p3));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            var l2 = surface.GetEntities().Skip(2).Single().Entity.ToLine();
            Assert.AreEqual(p1, l2.From);
            Assert.AreEqual(p2, l2.To);

            controller.scene.Drag(new Vector2(100, 100));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            p3 = (FreePoint)((PointView)surface.GetEntities().ElementAt(2).Entity).point;
            Assert.AreEqual(new Vector2(100, 100), surface.GetPointLocation(p3));
            l1 = surface.GetEntities().Skip(3).Single().Entity;
            Assert.AreEqual(l1.ToLine(), Line(p1, p3));
            Assert.AreEqual(DisplayStyle.Background, style);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);

            controller.undoManager.Undo();
            surface.SetPointLocation(p2, new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().Single().Entity);
            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);

            controller.undoManager.Redo();
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            p3 = (FreePoint)((PointView)surface.GetEntities().ElementAt(2).Entity).point;
            Assert.AreEqual(new Vector2(100, 100), surface.GetPointLocation(p3));
            l1 = surface.GetEntities().Skip(3).Single().Entity;
            Assert.AreEqual(l1.ToLine(), Line(p1, p3));
            Assert.AreEqual(DisplayStyle.Background, style);
            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_ExistingIntersectionFromPoint_NewFreeToPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var i = Intersect(Line(p1, p2), Line(p3, p4));
            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(i, DisplayStyle.Visible);
            surface.SetPointLocation(p1, new Vector2(-10, 0));
            surface.SetPointLocation(p2, new Vector2(10, 0));
            surface.SetPointLocation(p3, new Vector2(0, -10));
            surface.SetPointLocation(p4, new Vector2(0, 10));
            Assert.AreEqual(i.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Press(new Vector2(0, 0));
            Assert.False(controller.undoManager.CanUndo);
            Assert.AreEqual(i.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(i.AsView(), surface.GetEntities().First().Entity);
            var toPoint = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(toPoint));
            var (l, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l.ToLine(), Line(i, toPoint));
            Assert.AreEqual(DisplayStyle.Background, style);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(i.AsView(), surface.GetEntities().Single().Entity);

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(i.AsView(), surface.GetEntities().First().Entity);
            toPoint = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(toPoint));
            (l, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l.ToLine(), Line(i, toPoint));
            Assert.AreEqual(DisplayStyle.Background, style);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_NewFreeFromPoint_NewFreeToPoint_DragOverFromPoint() {
            var (controller, surface) = CreateTestController(Tool.Line);
            CollectionAssert.IsEmpty(surface.GetEntities());

            controller.scene.Press(new Vector2(48, 48));
            Assert.True(controller.undoManager.CanUndo);
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
            Assert.AreEqual(l.ToLine(), Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);
        }

        [Test]
        public void LineTool_NewFreeFromPoint_ExistingFreeToPoint() {
            var p2 = Point();

            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(p2, DisplayStyle.Visible);
            surface.SetPointLocation(p2, new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().Single().Entity);

            controller.scene.Press(new Vector2(48, 48));
            Assert.True(controller.undoManager.CanUndo);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            var p1 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            var (l1, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(l1.ToLine(), Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(59, 69));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(l1, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(l1.ToLine(), Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Release(new Vector2(59, 69));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(l1, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(l1.ToLine(), Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);

            controller.undoManager.Undo();
            surface.SetPointLocation(p2, new Vector2(60, 70));
            Assert.AreEqual(p2.AsView(), surface.GetEntities().Single().Entity);
            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);

            controller.undoManager.Redo();
            Assert.AreEqual(p2.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(l1, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(l1.ToLine(), Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);
            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_NewFreeFromPoint_NoToPoint() {
            var (controller, surface) = CreateTestController(Tool.Line);
            CollectionAssert.IsEmpty(surface.GetEntities());

            controller.scene.Press(new Vector2(48, 48));
            Assert.True(controller.undoManager.CanUndo);
            var p1 = (FreePoint)((PointView)surface.GetEntities().Single().Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(49, 49));
            Assert.AreEqual(p1, ((PointView)surface.GetEntities().Single().Entity).point);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Release(new Vector2(49, 49));
            Assert.AreEqual(p1, ((PointView)surface.GetEntities().Single().Entity).point);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            CollectionAssert.IsEmpty(surface.GetEntities());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(p1, ((PointView)surface.GetEntities().Single().Entity).point);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_ExistingFreeFromPoint_ExistingFreeToPoint() {
            var p1 = Point();
            var p2 = Point();
            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(p1, DisplayStyle.Visible);
            surface.Add(p2, DisplayStyle.Visible);
            var c = Circle(p1, p2).Add(surface);
            surface.SetPointLocation(p1, new Vector2(50, 50));
            surface.SetPointLocation(p2, new Vector2(100, 100));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().Skip(2).Single().Entity.ToCircle());

            controller.scene.Press(new Vector2(48, 48));
            Assert.False(controller.undoManager.CanUndo);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(99, 99));
            Assert.True(controller.undoManager.CanUndo);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().ElementAt(2).Entity.ToCircle());
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(100, 100), surface.GetPointLocation(p2));
            var (l, style) = surface.GetEntities().Skip(3).Single();
            Assert.AreEqual(l.ToLine(), Line(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Release(new Vector2(99, 99));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().ElementAt(2).Entity.ToCircle());
            Assert.AreEqual(l, surface.GetEntities().Skip(3).Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(100, 100), surface.GetPointLocation(p2));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            surface.SetPointLocation(p1, new Vector2(50, 50));
            surface.SetPointLocation(p2, new Vector2(100, 100));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().Skip(2).Single().Entity.ToCircle());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().ElementAt(2).Entity.ToCircle());
            Assert.AreEqual(l, surface.GetEntities().Skip(3).Single().Entity);
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(100, 100), surface.GetPointLocation(p2));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_ExistingFreeFromPoint_ExistingFreeToPoint_EntityOnSurface() {
            var p1 = Point();
            var p2 = Point();
            var (controller, surface) = CreateTestController(Tool.Line);
            surface.Add(p1, DisplayStyle.Visible);
            surface.Add(p2, DisplayStyle.Visible);
            Line(p1, p2).Add(surface);
            surface.SetPointLocation(p1, new Vector2(50, 50));
            surface.SetPointLocation(p2, new Vector2(100, 100));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(Line(p1, p2), surface.GetEntities().Skip(2).Single().Entity.ToLine());

            controller.scene.Press(new Vector2(48, 48));
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(99, 99));
            Assert.False(controller.undoManager.CanUndo);
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(Line(p1, p2), surface.GetEntities().Skip(2).Single().Entity.ToLine());
            Assert.AreEqual(new Vector2(50, 50), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(100, 100), surface.GetPointLocation(p2));
        }

        [Test]
        public void LineTool_NewIntersectionFromPoint_NewFreeToPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var i = Intersect(Line(p1, p2), Line(p3, p4));
            var (controller, surface) = CreateTestController(Tool.Line);
            Line(p1, p2).Add(surface);
            Line(p3, p4).Add(surface);
            surface.SetPointLocation(p1, new Vector2(-10, 0));
            surface.SetPointLocation(p2, new Vector2(10, 0));
            surface.SetPointLocation(p3, new Vector2(0, -10));
            surface.SetPointLocation(p4, new Vector2(0, 10));
            CollectionAssert.AreEqual(new[] { Line(p1, p2), Line(p3, p4) }, surface.GetEntities().Select(x => x.Entity.ToLine()));

            controller.scene.Press(new Vector2(0, 0));
            Assert.True(controller.undoManager.CanUndo);
            CollectionAssert.AreEqual(new[] { Line(p1, p2), Line(p3, p4) }, surface.GetEntities().Take(2).Select(x => x.Entity.ToLine()));
            Assert.AreEqual(i.AsView(), surface.GetEntities().Skip(2).Single().Entity);

            controller.scene.Drag(new Vector2(58, 68));
            CollectionAssert.AreEqual(new[] { Line(p1, p2), Line(p3, p4) }, surface.GetEntities().Take(2).Select(x => x.Entity.ToLine()));
            Assert.AreEqual(i.AsView(), surface.GetEntities().Skip(2).First().Entity);
            var toPoint = (FreePoint)((PointView)surface.GetEntities().ElementAt(3).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(toPoint));
            var (l, style) = surface.GetEntities().Skip(4).Single();
            Assert.AreEqual(l.ToLine(), Line(i, toPoint));
            Assert.AreEqual(DisplayStyle.Background, style);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            CollectionAssert.AreEqual(new[] { Line(p1, p2), Line(p3, p4) }, surface.GetEntities().Select(x => x.Entity.ToLine()));

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            CollectionAssert.AreEqual(new[] { Line(p1, p2), Line(p3, p4) }, surface.GetEntities().Take(2).Select(x => x.Entity.ToLine()));
            Assert.AreEqual(i.AsView(), surface.GetEntities().Skip(2).First().Entity);
            toPoint = (FreePoint)((PointView)surface.GetEntities().ElementAt(3).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(toPoint));
            l = surface.GetEntities().Skip(4).Single().Entity;
            Assert.AreEqual(l.ToLine(), Line(i, toPoint));
            Assert.AreEqual(DisplayStyle.Background, style);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_NewIntersectionFreeFromPoint_NewFreeToPoint() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var i = Intersect(Line(p1, p3), Line(p2, p3));
            Assert.AreSame(i, p3);
            var (controller, surface) = CreateTestController(Tool.Line);
            Line(p1, p3).Add(surface);
            Line(p2, p3).Add(surface);
            surface.SetPointLocation(p1, new Vector2(-10, 0));
            surface.SetPointLocation(p2, new Vector2(0, 10));
            surface.SetPointLocation(p3, new Vector2(0, 0));
            CollectionAssert.AreEqual(new[] { Line(p1, p3), Line(p2, p3) }, surface.GetEntities().Select(x => x.Entity.ToLine()));

            controller.scene.Press(new Vector2(0, 0));
            Assert.True(controller.undoManager.CanUndo);
            CollectionAssert.AreEqual(new[] { Line(p1, p3), Line(p2, p3) }, surface.GetEntities().Take(2).Select(x => x.Entity.ToLine()));
            Assert.AreEqual(i.AsView(), surface.GetEntities().Skip(2).Single().Entity);

            controller.scene.Drag(new Vector2(58, 68));
            CollectionAssert.AreEqual(new[] { Line(p1, p3), Line(p2, p3) }, surface.GetEntities().Take(2).Select(x => x.Entity.ToLine()));
            Assert.AreEqual(i.AsView(), surface.GetEntities().Skip(2).First().Entity);
            var toPoint = (FreePoint)((PointView)surface.GetEntities().ElementAt(3).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(toPoint));
            var (l, style) = surface.GetEntities().Skip(4).Single();
            Assert.AreEqual(l.ToLine(), Line(i, toPoint));
            Assert.AreEqual(DisplayStyle.Background, style);
            Assert.AreEqual(new Vector2(0, 0), surface.GetPointLocation(p3));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            CollectionAssert.AreEqual(new[] { Line(p1, p3), Line(p2, p3) }, surface.GetEntities().Select(x => x.Entity.ToLine()));
            Assert.AreEqual(new Vector2(0, 0), surface.GetPointLocation(p3));

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            CollectionAssert.AreEqual(new[] { Line(p1, p3), Line(p2, p3) }, surface.GetEntities().Take(2).Select(x => x.Entity.ToLine()));
            Assert.AreEqual(i.AsView(), surface.GetEntities().Skip(2).First().Entity);
            toPoint = (FreePoint)((PointView)surface.GetEntities().ElementAt(3).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(toPoint));
            l = surface.GetEntities().Skip(4).Single().Entity;
            Assert.AreEqual(l.ToLine(), Line(i, toPoint));
            Assert.AreEqual(DisplayStyle.Background, style);
            Assert.AreEqual(new Vector2(0, 0), surface.GetPointLocation(p3));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_ExistingFreePoint_ExistingCircleIntersectionPoint() {
            var (controller, surface) = CreateTestController(Tool.Line);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-100, 0)),
                (p2, new Vector2(100, 0)),
                (p3, new Vector2(0, 50)),
            });
            Assert.AreSame(p3, i1);
            c1.Add(surface);
            c2.Add(surface);
            foreach(var item in new[] { p3, i2 }) {
                surface.Add(item, DisplayStyle.Background);
            }

            Assert.AreEqual(4, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 49));
            Assert.AreEqual(4, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(-1, -49));
            var line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(i2, line.To);

            controller.scene.Release(new Vector2(-1, -49));
            Assert.AreSame(line, surface.GetEntities().Skip(4).Single().Entity.ToLine());
            Assert.AreSame(i1, line.From);
            Assert.AreSame(i2, line.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(4, surface.GetEntities().Count());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreSame(line, surface.GetEntities().Skip(4).Single().Entity.ToLine());
            Assert.AreSame(i1, line.From);
            Assert.AreSame(i2, line.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_ExistingFreePoint_NewCircleIntersectionPoint() {
            var (controller, surface) = CreateTestController(Tool.Line);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-100, 0)),
                (p2, new Vector2(100, 0)),
                (p3, new Vector2(0, 50)),
            });
            Assert.AreSame(p3, i1);
            c1.Add(surface);
            c2.Add(surface);
            foreach(var item in new[] { p3 }) {
                surface.Add(item, DisplayStyle.Background);
            }

            Assert.AreEqual(3, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 49));
            Assert.AreEqual(3, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(-1, -49));
            var (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreSame(i2, point.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, style);
            var line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(i2, line.To);

            controller.scene.Release(new Vector2(-1, -49));
            (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreSame(i2, point.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, style);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(i2, line.To);
            Assert.AreEqual(new Vector2(0, 50), surface.GetPointLocation(p3));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(3, surface.GetEntities().Count());
            Assert.AreEqual(new Vector2(0, 50), surface.GetPointLocation(p3));

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreSame(i2, point.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, style);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(i2, line.To);
            Assert.AreEqual(new Vector2(0, 50), surface.GetPointLocation(p3));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_ExistingIntersectionPoint_NewCircleIntersectionFreePoint() {
            var (controller, surface) = CreateTestController(Tool.Line);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-100, 0)),
                (p2, new Vector2(100, 0)),
                (p3, new Vector2(0, -50)),
            });
            Assert.AreSame(p3, i1);
            c1.Add(surface);
            c2.Add(surface);
            foreach(var item in new[] { i2 }) {
                surface.Add(item, DisplayStyle.Background);
            }

            Assert.AreEqual(3, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 49));
            Assert.AreEqual(3, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(-1, -49));
            var (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreSame(i1, point.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, style);
            var line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i2, line.From);
            Assert.AreSame(i1, line.To);
            Assert.AreEqual(new Vector2(0, -50), surface.GetPointLocation((FreePoint)point.ToPoint()));

            controller.scene.Release(new Vector2(-1, -49));
            (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreSame(i1, point.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, style);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i2, line.From);
            Assert.AreSame(i1, line.To);
            Assert.AreEqual(new Vector2(0, -50), surface.GetPointLocation((FreePoint)point.ToPoint()));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(3, surface.GetEntities().Count());
            Assert.AreEqual(new Vector2(0, -50), surface.GetPointLocation((FreePoint)point.ToPoint()));

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreSame(i1, point.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, style);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i2, line.From);
            Assert.AreSame(i1, line.To);
            Assert.AreEqual(new Vector2(0, -50), surface.GetPointLocation((FreePoint)point.ToPoint()));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_NewCircleIntersectionFreePoint_ThenNewFreePoint_ThenNewCircleIntersectionPoint() {
            var (controller, surface) = CreateTestController(Tool.Line);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-100, 0)),
                (p2, new Vector2(100, 0)),
                (p3, new Vector2(0, 50)),
            });
            Assert.AreSame(p3, i1);
            c1.Add(surface);
            c2.Add(surface);

            Assert.AreEqual(2, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 46));
            Assert.AreEqual(3, surface.GetEntities().Count());
            var (pointFrom, styleFrom) = surface.GetEntities().Skip(2).Single();
            Assert.AreSame(p3, pointFrom.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, styleFrom);
            Assert.True(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(0, 48));
            Assert.AreEqual(3, surface.GetEntities().Count());
            (pointFrom, styleFrom) = surface.GetEntities().Skip(2).Single();
            Assert.AreSame(p3, pointFrom.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, styleFrom);
            Assert.True(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(0, 54));
            Assert.AreEqual(5, surface.GetEntities().Count());
            (pointFrom, styleFrom) = surface.GetEntities().Skip(2).First();
            Assert.AreSame(p3, pointFrom.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, styleFrom);
            var (pointTo, styleTo) = surface.GetEntities().Skip(3).First();
            Assert.IsInstanceOf<FreePoint>(pointTo.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, styleTo);
            Assert.True(controller.undoManager.CanUndo);
            var line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(pointTo.ToPoint(), line.To);
            Assert.AreEqual(new Vector2(0, 54), surface.GetPointLocation((FreePoint)pointTo.ToPoint()));

            controller.scene.Drag(new Vector2(0, 55));
            Assert.AreEqual(5, surface.GetEntities().Count());
            (pointFrom, styleFrom) = surface.GetEntities().Skip(2).First();
            Assert.AreSame(p3, pointFrom.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, styleFrom);
            Assert.AreSame(pointTo.ToPoint(), surface.GetEntities().Skip(3).First().Entity.ToPoint());
            Assert.True(controller.undoManager.CanUndo);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(pointTo.ToPoint(), line.To);
            Assert.AreEqual(new Vector2(0, 55), surface.GetPointLocation((FreePoint)pointTo.ToPoint()));

            controller.scene.Drag(new Vector2(0, 60));
            Assert.AreEqual(5, surface.GetEntities().Count());
            (pointFrom, styleFrom) = surface.GetEntities().Skip(2).First();
            Assert.AreSame(p3, pointFrom.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, styleFrom);
            Assert.AreSame(pointTo.ToPoint(), surface.GetEntities().Skip(3).First().Entity.ToPoint());
            Assert.True(controller.undoManager.CanUndo);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(pointTo.ToPoint(), line.To);
            Assert.AreEqual(new Vector2(0, 60), surface.GetPointLocation((FreePoint)pointTo.ToPoint()));

            controller.scene.Drag(new Vector2(-1, -49));
            (pointTo, styleTo) = surface.GetEntities().Skip(3).First();
            Assert.AreSame(i2, pointTo.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, styleTo);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(i2, line.To);

            controller.scene.Release(new Vector2(-1, -49));
            (pointTo, styleTo) = surface.GetEntities().Skip(3).First();
            Assert.AreSame(i2, pointTo.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, styleTo);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(i2, line.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(2, surface.GetEntities().Count());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            (pointTo, styleTo) = surface.GetEntities().Skip(3).First();
            Assert.AreSame(i2, pointTo.ToPoint());
            Assert.AreEqual(DisplayStyle.Visible, styleTo);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i1, line.From);
            Assert.AreSame(i2, line.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void LineTool_ExistingIntersectionPoint_DragBackToStartPoint() {
            var (controller, surface) = CreateTestController(Tool.Line);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-100, 0)),
                (p2, new Vector2(100, 0)),
                (p3, new Vector2(0, -50)),
            });
            Assert.AreSame(p3, i1);
            c1.Add(surface);
            c2.Add(surface);
            foreach(var item in new[] { i2 }) {
                surface.Add(item, DisplayStyle.Background);
            }

            Assert.AreEqual(3, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 49));
            Assert.AreEqual(3, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(0, 60));
            var (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreEqual(DisplayStyle.Visible, style);
            var line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i2, line.From);
            Assert.AreSame(point.ToPoint(), line.To);
            Assert.AreEqual(new Vector2(0, 60), surface.GetPointLocation((FreePoint)point.ToPoint()));

            controller.scene.Drag(new Vector2(1, 51));
            Assert.AreEqual(3, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(-1, 52));
            Assert.AreEqual(3, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(-1, 51));
            Assert.AreEqual(3, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);
        }

        [Test]
        public void LineTool_ExistingIntersectionPoint_DragCloseToCircle() {
            var (controller, surface) = CreateTestController(Tool.Line);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-100, 0)),
                (p2, new Vector2(100, 0)),
                (p3, new Vector2(0, -50)),
            });
            Assert.AreSame(p3, i1);
            c1.Add(surface);
            c2.Add(surface);
            foreach(var item in new[] { i2 }) {
                surface.Add(item, DisplayStyle.Background);
            }

            Assert.AreEqual(3, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 49));
            Assert.AreEqual(3, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(0, 60));
            var (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreEqual(DisplayStyle.Visible, style);
            var line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i2, line.From);
            Assert.AreSame(point.ToPoint(), line.To);
            Assert.AreEqual(new Vector2(0, 60), surface.GetPointLocation((FreePoint)point.ToPoint()));

            controller.scene.Drag(new Vector2(4, 57));
            (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreEqual(DisplayStyle.Visible, style);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i2, line.From);
            Assert.AreSame(point.ToPoint(), line.To);
            Assert.AreEqual(new Vector2(4, 57), surface.GetPointLocation((FreePoint)point.ToPoint()));

            controller.scene.Drag(new Vector2(4, 56));
            (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreEqual(DisplayStyle.Visible, style);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i2, line.From);
            Assert.AreSame(point.ToPoint(), line.To);
            Assert.AreEqual(new Vector2(4, 56), surface.GetPointLocation((FreePoint)point.ToPoint()));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(3, surface.GetEntities().Count());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            (point, style) = surface.GetEntities().Skip(3).First();
            Assert.AreEqual(DisplayStyle.Visible, style);
            line = surface.GetEntities().Skip(4).Single().Entity.ToLine();
            Assert.AreSame(i2, line.From);
            Assert.AreSame(point.ToPoint(), line.To);
            Assert.AreEqual(new Vector2(4, 56), surface.GetPointLocation((FreePoint)point.ToPoint()));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);

        }

        [Test]
        public void LineTool_NewFreeFromPoint_ExistingIntersectionPointInsideAngle() {
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var (controller, surface) = CreateTestController(Tool.Line);
            var l1 = Line(p1, p3).Add(surface);
            var l2 = Line(p2, p3).Add(surface);
            var i = Intersect(l1, l2).Add(surface);
            surface.SetPoints(new[] {
                (p1, new Vector2(-20, 100)),
                (p2, new Vector2(20, 100)),
                (p3, new Vector2(0, 0)),
            });
            Assert.AreSame(p3, i);

            Assert.AreEqual(3, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 100));
            Assert.True(controller.undoManager.CanUndo);
            var pFrom = (FreePoint)((PointView)surface.GetEntities().Skip(3).Single().Entity).point;
            Assert.AreEqual(new Vector2(0, 100), surface.GetPointLocation(pFrom));

            controller.scene.Drag(new Vector2(0, 10));
            Assert.AreSame(pFrom, (FreePoint)((PointView)surface.GetEntities().Skip(3).First().Entity).point);
            Assert.AreEqual(new Vector2(0, 100), surface.GetPointLocation(pFrom));
            var pTo = (FreePoint)((PointView)surface.GetEntities().Skip(4).First().Entity).point;
            Assert.AreEqual(new Vector2(0, 10), surface.GetPointLocation(pTo));
            var (l, style) = surface.GetEntities().Skip(5).Single();
            Assert.AreEqual(l.ToLine(), Line(pFrom, pTo));
        }
        #endregion

        #region circle
        [Test]
        public void CircleTool_NewFreeFromPoint_NewFreeToPoint() {
            var (controller, surface) = CreateTestController(Tool.Circle);
            CollectionAssert.IsEmpty(surface.GetEntities());

            controller.scene.Press(new Vector2(48, 48));
            Assert.True(controller.undoManager.CanUndo);
            var p1 = (FreePoint)((PointView)surface.GetEntities().Single().Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            var p2 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p2));

            var (c, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(c.ToCircle(), Circle(p1, p2));
            Assert.AreEqual(DisplayStyle.Background, style);

            controller.scene.Drag(new Vector2(60, 70));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            controller.scene.Release(new Vector2(60, 70));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            CollectionAssert.IsEmpty(surface.GetEntities());
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(DisplayStyle.Background, surface.GetEntities().Skip(2).Single().Style);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }
        #endregion

        #region line segment
        [Test]
        public void LineSegmentTool_NewFreeFromPoint_NewFreeToPoint() {
            var (controller, surface) = CreateTestController(Tool.LineSegment);
            CollectionAssert.IsEmpty(surface.GetEntities());

            controller.scene.Press(new Vector2(48, 48));
            Assert.True(controller.undoManager.CanUndo);
            var p1 = (FreePoint)((PointView)surface.GetEntities().Single().Entity).point;
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            controller.scene.Drag(new Vector2(58, 68));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));

            var p2 = (FreePoint)((PointView)surface.GetEntities().ElementAt(1).Entity).point;
            Assert.AreEqual(new Vector2(58, 68), surface.GetPointLocation(p2));

            var (c, style) = surface.GetEntities().Skip(2).Single();
            Assert.AreEqual(c, this.c.LineSegment(p1, p2));
            Assert.AreEqual(DisplayStyle.Visible, style);

            controller.scene.Drag(new Vector2(60, 70));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            controller.scene.Release(new Vector2(60, 70));
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            CollectionAssert.IsEmpty(surface.GetEntities());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(p1.AsView(), surface.GetEntities().First().Entity);
            Assert.AreEqual(p2.AsView(), surface.GetEntities().ElementAt(1).Entity);
            Assert.AreEqual(c, surface.GetEntities().Skip(2).Single().Entity);
            Assert.AreEqual(DisplayStyle.Visible, surface.GetEntities().Skip(2).Single().Style);
            Assert.AreEqual(new Vector2(48, 48), surface.GetPointLocation(p1));
            Assert.AreEqual(new Vector2(60, 70), surface.GetPointLocation(p2));

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }
        #endregion

        #region circle segment
        [Test]
        public void CircleSegmentTool_ExistingFreePoint_ExistingCircleIntersectionPoint() {
            var (controller, surface) = CreateTestController(Tool.CircleSegment);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-100, 0)),
                (p2, new Vector2(100, 0)),
                (p3, new Vector2(0, 50)),
            });
            Assert.AreSame(p3, i1);
            foreach(var item in new[] { p1, p2, p3, i2 }) {
                surface.Add(item, DisplayStyle.Background);
            }

            Assert.AreEqual(4, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 49));
            Assert.AreEqual(4, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(-1, -49));
            var arc = (CircleSegment)surface.GetEntities().Skip(4).Single().Entity;
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i1, arc.From);
            Assert.AreSame(i2, arc.To);

            controller.scene.Release(new Vector2(-1, -49));
            Assert.AreSame(arc, (CircleSegment)surface.GetEntities().Skip(4).Single().Entity);
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i1, arc.From);
            Assert.AreSame(i2, arc.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(4, surface.GetEntities().Count());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreSame(arc, (CircleSegment)surface.GetEntities().Skip(4).Single().Entity);
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i1, arc.From);
            Assert.AreSame(i2, arc.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void CircleSegmentTool_ExistingCircleIntersectionPoint_ExistingFreePoint() {
            var (controller, surface) = CreateTestController(Tool.CircleSegment);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var c1 = Circle(p1, p3);
            var c2 = Circle(p2, p3);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-100, 0)),
                (p2, new Vector2(100, 0)),
                (p3, new Vector2(0, -50)),
            });
            Assert.AreSame(p3, i1);
            foreach(var item in new[] { p1, p2, p3, i2 }) {
                surface.Add(item, DisplayStyle.Background);
            }

            Assert.AreEqual(4, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 49));
            Assert.AreEqual(4, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(-1, -49));
            Assert.True(controller.undoManager.CanUndo);
            var arc = (CircleSegment)surface.GetEntities().Skip(4).Single().Entity;
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i2, arc.From);
            Assert.AreSame(i1, arc.To);

            controller.scene.Drag(new Vector2(500, 500));
            Assert.False(controller.undoManager.CanUndo);
            Assert.AreEqual(4, surface.GetEntities().Count());

            controller.scene.Drag(new Vector2(-1, -49));
            Assert.True(controller.undoManager.CanUndo);
            arc = (CircleSegment)surface.GetEntities().Skip(4).Single().Entity;
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i2, arc.From);
            Assert.AreSame(i1, arc.To);

            controller.scene.Release(new Vector2(-1, -49));
            Assert.AreSame(arc, (CircleSegment)surface.GetEntities().Skip(4).Single().Entity);
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i2, arc.From);
            Assert.AreSame(i1, arc.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(4, surface.GetEntities().Count());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreSame(arc, (CircleSegment)surface.GetEntities().Skip(4).Single().Entity);
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i2, arc.From);
            Assert.AreSame(i1, arc.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void CircleSegmentTool_ExistingCircleIntersectionPoint_ExistingCircleIntersectionPoint() {
            var (controller, surface) = CreateTestController(Tool.CircleSegment);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var p4 = Point();
            var p5 = Point();
            var c1 = Circle(p1, p2);
            var c2 = Circle(p3, p4);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-40, 0)),
                (p2, new Vector2(10, 0)),
                (p3, new Vector2(40, 0)),
                (p4, new Vector2(-10, 0)),
                (p5, new Vector2(500, 500)),
            });
            foreach(var item in new[] { p1, p2, p3, p4, p5, i1, i2 }) {
                surface.Add(item, DisplayStyle.Background);
            }

            Assert.AreEqual(7, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 30));
            Assert.AreEqual(7, surface.GetEntities().Count());
            Assert.False(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(-1, -29));
            Assert.True(controller.undoManager.CanUndo);
            var arc = (CircleSegment)surface.GetEntities().Skip(7).Single().Entity;
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i2, arc.From);
            Assert.AreSame(i1, arc.To);

            controller.scene.Drag(new Vector2(500, 500));
            Assert.False(controller.undoManager.CanUndo);
            Assert.AreEqual(7, surface.GetEntities().Count());

            controller.scene.Drag(new Vector2(-1, -29));
            Assert.True(controller.undoManager.CanUndo);
            arc = (CircleSegment)surface.GetEntities().Skip(7).Single().Entity;
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i2, arc.From);
            Assert.AreSame(i1, arc.To);

            controller.scene.Release(new Vector2(-1, -49));
            Assert.AreSame(arc, (CircleSegment)surface.GetEntities().Skip(7).Single().Entity);
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i2, arc.From);
            Assert.AreSame(i1, arc.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(7, surface.GetEntities().Count());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreSame(arc, (CircleSegment)surface.GetEntities().Skip(7).Single().Entity);
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i2, arc.From);
            Assert.AreSame(i1, arc.To);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }

        [Test]
        public void CircleSegmentTool_NewFreePoint_ExistingCircleIntersectionPoint_ThanToNoPoint() {
            var (controller, surface) = CreateTestController(Tool.CircleSegment);
            var p1 = Point();
            var p2 = Point();
            var p3 = Point();
            var c1 = Circle(p1, p3).Add(surface);
            var c2 = Circle(p2, p3).Add(surface);
            var (i1, i2) = Intersect(c1, c2);
            surface.SetPoints(new[] {
                (p1, new Vector2(-100, 0)),
                (p2, new Vector2(100, 0)),
                (p3, new Vector2(0, 50)),
            });
            Assert.AreSame(p3, i1);
            foreach(var item in new[] { p1, p2, i2 }) {
                surface.Add(item, DisplayStyle.Background);
            }

            Assert.AreEqual(5, surface.GetEntities().Count());

            controller.scene.Press(new Vector2(0, 49));
            Assert.AreEqual(6, surface.GetEntities().Count());
            Assert.AreSame(p3, surface.GetEntities().Last().Entity.ToPoint());
            Assert.True(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(-1, -49));
            var arc = (CircleSegment)surface.GetEntities().Skip(6).Single().Entity;
            Assert.AreSame(c2, arc.Circle);
            Assert.AreSame(i1, arc.From);
            Assert.AreSame(i2, arc.To);
            Assert.True(controller.undoManager.CanUndo);

            controller.scene.Drag(new Vector2(500, 500));
            Assert.AreEqual(6, surface.GetEntities().Count());
            Assert.AreSame(p3, surface.GetEntities().Last().Entity.ToPoint());
            Assert.True(controller.undoManager.CanUndo);

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
            controller.undoManager.Undo();
            Assert.AreEqual(5, surface.GetEntities().Count());

            Assert.False(controller.undoManager.CanUndo);
            Assert.True(controller.undoManager.CanRedo);
            controller.undoManager.Redo();
            Assert.AreEqual(6, surface.GetEntities().Count());
            Assert.AreSame(p3, surface.GetEntities().Last().Entity.ToPoint());

            Assert.True(controller.undoManager.CanUndo);
            Assert.False(controller.undoManager.CanRedo);
        }
        #endregion
    }
}
