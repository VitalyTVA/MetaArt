using MetaArt.Core;
using MetaCore;
using System.Numerics;

namespace MetaConstruct {
    public enum Tool { Hand, Point, Line, Circle, LineSegment, CircleSegment }
    public class PlotController {
        Engine engine;
        public Surface Surface { get; private set; } = null!;
        public Scene scene => engine.scene;
        Tool tool;
        public readonly UndoManager undoManager = new();
        public PlotController(int width, int height) {
            engine = new Engine(width, height);
            scene.AddElement(new PlotElement() { 
                Rect = new Rect(0, 0, width, height),
                GetPressState = startPoint => {
                    if(tool == Tool.Point) {
                        var point = Surface.HitTest(startPoint).OfType<FreePoint>().FirstOrDefault();

                        IUpdatableAction<Vector2> action = null!;

                        if(point == null) {
                            var intersectionPoint = Surface.HitTestIntersection(startPoint);
                            if(intersectionPoint != null && !Surface.Contains(intersectionPoint.AsView())) {
                                action = undoManager.Execute(
                                       intersectionPoint,
                                       redo: statePoint => {
                                           Surface.Add(statePoint.AsView(), DisplayStyle.Visible);
                                           return (statePoint, true);
                                       },
                                       undo: statePoint => {
                                           Surface.Remove(statePoint);
                                           return statePoint;
                                       },
                                       update: (Point statePoint, Vector2 offset) => {
                                           return (statePoint, true);
                                       }
                                   );
                            }
                        }
                        if(action == null) {
                            if(point == null) {
                                action = undoManager.Execute(
                                    (point: Surface.Constructor.Point(), location: startPoint),
                                    redo: state => {
                                        Surface.Add(state.point.AsView(), DisplayStyle.Visible);
                                        Surface.SetPointLocation(state.point, state.location);
                                        return (state.point, true);
                                    },
                                    undo: statePoint => {
                                        var location = Surface.GetPointLocation(statePoint);
                                        Surface.Remove(statePoint);
                                        return (statePoint, location);
                                    },
                                    update: (FreePoint statePoint, Vector2 offset) => {
                                        Surface.SetPointLocation(statePoint, startPoint + offset);
                                        return (statePoint, true);
                                    }
                                );
                            } else {
                                var startPointLocation = Surface.GetPointLocation(point);
                                action = undoManager.Execute(
                                    (point: point!, location: Surface.GetPointLocation(point!)),
                                    redo: state => {
                                        var location = Surface.GetPointLocation(state.point);
                                        bool move = !MathF.VectorsEqual(state.location, location);
                                        if(move)
                                            Surface.SetPointLocation(state.point, state.location);
                                        return ((state.point, location), move);
                                    },
                                    undo: state => {
                                        var location = Surface.GetPointLocation(state.point);
                                        Surface.SetPointLocation(state.point, state.location);
                                        return (state.point, location);
                                    },
                                    update: ((FreePoint point, Vector2 location) state, Vector2 offset) => {
                                        Surface.SetPointLocation(state.point, startPointLocation + offset);
                                        return (state, true);
                                    }
                                );
                            };
                        }
                        return DragInputState.GetDragState(
                            startPoint,
                            onDrag: offset => {
                                if(offset.LengthSquared() > 0)
                                    action.Update(offset);
                                return true;
                            }
                        );
                    }

                    if(tool == Tool.Line) {
                        return CreateTwoPointsTool(startPoint, (c, p1, p2) => c.Line(p1, p2), DisplayStyle.Background);
                    }
                    if(tool == Tool.Circle) {
                        return CreateTwoPointsTool(startPoint, (c, p1, p2) => c.Circle(p1, p2), DisplayStyle.Background);
                    }
                    if(tool == Tool.LineSegment) {
                        return CreateTwoPointsTool(startPoint, (c, p1, p2) => c.LineSegment(p1, p2), DisplayStyle.Visible);
                    }
                    if(tool == Tool.CircleSegment) {
                        return CreateTwoPointsTool(startPoint, (c, p1, p2) => ConstructorHelper.CircleSegment(p1, p2), DisplayStyle.Visible);
                    }

                    throw new NotImplementedException();
                }
            });
        }

        private InputState CreateTwoPointsTool(Vector2 startPoint, Func<Constructor, Point, Point, Entity?> createEntity, DisplayStyle enitityStyle) {
            var point = Surface.HitTest(startPoint).FirstOrDefault();

            Either<Point, (FreePoint point, Vector2 location)> from = point != null
                ? point.AsLeft()
                : (Surface.Constructor.Point(), startPoint).AsRight();

            var transaction = undoManager.Execute(
                (
                    from: from,
                    to: default((Either<Point, (FreePoint point, Vector2 location)> pointInfo, Entity? entity)?)
                ),
                redo: state => {
                    switch(state.from) {
                        case (Point existingPoint, null):
                            break;
                        case (null, (FreePoint newPoint, Vector2 location)):
                            Surface.Add(newPoint.AsView(), DisplayStyle.Visible);
                            Surface.SetPointLocation(newPoint, location);
                            break;
                    }

                    var toInfo = state.to;
                    if(toInfo != null) {
                        if(toInfo.Value.pointInfo.IsRight()) {
                            Surface.Add(toInfo.Value.pointInfo.ToRight().point.AsView(), DisplayStyle.Visible);
                            Surface.SetPointLocation(toInfo.Value.pointInfo.ToRight().point, toInfo.Value.pointInfo.ToRight().location);
                        }
                        if(toInfo.Value.entity != null)
                            Surface.Add(toInfo.Value.entity, enitityStyle);
                    }

                    var from = state.from.Match(
                        left: x => Either<Point, FreePoint>.Left(x),
                        right: x => Either<Point, FreePoint>.Right(x.point));
                    var to = toInfo == null ? default(Either<Point, FreePoint>?) : toInfo.Value.pointInfo.Match(
                        left: x => Either<Point, FreePoint>.Left(x),
                        right: x => Either<Point, FreePoint>.Right(x.point));
                    return ((
                        from: from,
                        to: toInfo != null ? (to!.Value, toInfo.Value.entity) : null
                    ), toInfo != null || from.IsRight());
                },
                undo: state => {
                    Either<Point, (FreePoint point, Vector2 location)> from;
                    if(state.from.IsRight()) {
                        var point = state.from.ToRight();
                        from = (point, Surface.GetPointLocation(point)).AsRight();
                        Surface.Remove(point);
                    } else {
                        from = state.from.ToLeft().AsLeft();
                    }

                    if(state.to == null) {
                        return (from, null);
                    }

                    if(state.to!.Value.entity != null)
                        Surface.Remove(state.to!.Value.entity);
                    Either<Point, (FreePoint point, Vector2 location)> pointInfo;
                    if(state.to!.Value.to.IsRight()) {
                        var toLocation = Surface.GetPointLocation(state.to!.Value.to.ToRight());
                        Surface.Remove(state.to!.Value.to.ToRight());
                        pointInfo = (state.to!.Value.to.ToRight(), toLocation).AsRight();
                    } else {
                        pointInfo = state.to!.Value.to.ToLeft().AsLeft();
                    }
                    return (
                        from,
                        (pointInfo, state.to!.Value.entity)
                    );
                },
                update: ((Either<Point, FreePoint> from, (Either<Point, FreePoint> to, Entity entity)? to) state, Vector2 offset) => {
                    var toInfo = state.to;
                    if(/*toInfo == null && */offset.LengthSquared() < Surface.PointHitTestDistance * Surface.PointHitTestDistance)
                        return (state, true);

                    var fromPoint = state.from.Match(x => x, x => x);
                    var newToPoint = toInfo != null ? toInfo.Value.to.Match(x => null, x => (FreePoint?)x) : null;
                    var point = Surface
                        .HitTest(startPoint + offset)
                        .Where(x => x != newToPoint && x != fromPoint)
                        .FirstOrDefault();
                    if(point != null) {
                        if(newToPoint != null && toInfo != null && toInfo.Value.to.IsRight()) {
                            Surface.Remove(newToPoint!);
                            Surface.Remove(toInfo.Value.entity);
                            toInfo = null;
                        }
                        if(toInfo != null && toInfo.Value.to.IsLeft() && toInfo.Value.to.ToLeft() != point) {
                            Surface.Remove(toInfo.Value.entity);
                            toInfo = null;
                        }
                        if(toInfo == null) {
                            var entity = createEntity(Surface.Constructor, fromPoint, point);
                            if(entity != null) {
                                Surface.Add(entity!, enitityStyle);
                                toInfo = (point.AsLeft(), entity);
                            }
                        }
                    } else {
                        if(newToPoint == null && toInfo != null && toInfo.Value.to.IsLeft()) {
                            Surface.Remove(toInfo.Value.entity);
                            toInfo = null;
                        }
                        if(toInfo == null) {
                            var pointTo = Surface.Constructor.Point();
                            var entity = createEntity(Surface.Constructor, fromPoint, pointTo);
                            if(entity != null) {
                                Surface.Add(pointTo.AsView(), DisplayStyle.Visible);
                                Surface.Add(entity!, enitityStyle);
                                toInfo = (pointTo.AsRight(), entity);
                            }
                        }
                    }

                    if(toInfo != null && toInfo.Value.to.IsRight())
                        Surface.SetPointLocation(toInfo.Value.to.ToRight(), startPoint + offset);
                    return ((state.from, toInfo), toInfo != null);
                }
            );
            return DragInputState.GetDragState(
                startPoint,
                onDrag: offset => {
                    transaction.Update(offset);
                    return true;
                }
            );
        }

        public void Load(Surface surface) {
            this.Surface = surface;
        }
        public void SetTool(Tool tool) { 
            this.tool = tool;
        }
    }

    public class PlotElement : Element {
        public PlotElement() {
            HitTestVisible = true;
        }
    }

    public interface IUpdatableAction<TUpdate> {
        void Update(TUpdate update);
    }
    public class UndoManager {
        Stack<IUndoAction> undoStack = new();
        Stack<IRedoAction> redoStack = new();
        public bool CanUndo => undoStack.Count != 0;
        public bool CanRedo => redoStack.Count != 0;
        public void Undo() {
            var action = undoStack.Pop();
            redoStack.Push(action.Undo());
        }
        public void Redo() {
            var action = redoStack.Pop();
            undoStack.Push(action.Redo());
        }

        public IUpdatableAction<TUpdate> Execute<TDo, TUndo, TUpdate>(
            TDo data,
            Func<TDo, (TUndo state, bool canUndo)> redo,
            Func<TUndo, TDo> undo,
            Func<TUndo, TUpdate, (TUndo state, bool canUndo)> update
        ) {
            var action = new UndoActionUpdatable<TDo, TUndo, TUpdate>(this, redo(data), redo, undo, update);
            return action;
        }

        void Push(IUndoAction action) {
            if(!IsTopAction(action)) {
                undoStack.Push(action);
                redoStack.Clear();
            }
        }

        void Pop(IUndoAction action) {
            if(IsTopAction(action))
                undoStack.Pop();
        }

        bool IsTopAction(IUndoAction action) => undoStack.Any() && undoStack.Peek() == action;

        interface IUndoAction {
            IRedoAction Undo();
        }
        interface IRedoAction {
            IUndoAction Redo();
        }

        class UndoAction<TDo, TUndo> : IUndoAction {
            protected TUndo data;
            readonly Func<TDo, (TUndo state, bool canUndo)> redo;
            readonly Func<TUndo, TDo> undo;
            public UndoAction(TUndo data, Func<TDo, (TUndo state, bool canUndo)> redo, Func<TUndo, TDo> undo) {
                this.data = data;
                this.redo = redo;
                this.undo = undo;
            }
            IRedoAction IUndoAction.Undo() {
                var redoData = undo(data);
                return new RedoAction<TDo, TUndo>(redoData, redo, undo);
            }
        }

        class RedoAction<TDo, TUndo> : IRedoAction {
            readonly TDo data;
            readonly Func<TDo, (TUndo state, bool canUndo)> redo;
            readonly Func<TUndo, TDo> undo;
            public RedoAction(TDo data, Func<TDo, (TUndo state, bool canUndo)> redo, Func<TUndo, TDo> undo) {
                this.data = data;
                this.redo = redo;
                this.undo = undo;
            }
            IUndoAction IRedoAction.Redo() {
                var (undoData, canUndo) = redo(data);
                if(!canUndo)
                    throw new InvalidOperationException();
                return new UndoAction<TDo, TUndo>(undoData, redo, undo);
            }
        }

        class UndoActionUpdatable<TDo, TUndo, TUpdate> : UndoAction<TDo, TUndo>, IUpdatableAction<TUpdate> {
            readonly UndoManager manager;
            readonly Func<TUndo, TUpdate, (TUndo state, bool canUndo)> update;
            public UndoActionUpdatable(UndoManager manager, (TUndo state, bool canUndo) data, Func<TDo, (TUndo state, bool canUndo)> redo, Func<TUndo, TDo> undo, Func<TUndo, TUpdate, (TUndo state, bool canUndo)> update) 
                : base(data.state, redo, undo) {
                this.manager = manager;
                this.update = update;
                UpdtateStack(data.canUndo);
            }
            public void Update(TUpdate data) {
                (this.data, var canUndo) = update(this.data, data);
                UpdtateStack(canUndo);
            }
            void UpdtateStack(bool canUndo) {
                if(canUndo)
                    manager.Push(this);
                else
                    manager.Pop(this);
            }
        }
    }
}
