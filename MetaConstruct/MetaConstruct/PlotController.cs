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
                            if(intersectionPoint != null && !Surface.Contains(intersectionPoint)) {
                                action = undoManager.Execute(
                                       intersectionPoint,
                                       redo: statePoint => {
                                           Surface.Add(statePoint, DisplayStyle.Visible);
                                           return (statePoint, true);
                                       },
                                       undo: statePoint => {
                                           Surface.Remove(statePoint, keepLocation: true);
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
                                        Surface.Add(state.point, DisplayStyle.Visible);
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

        enum StateKind { ExistingPoint, ExistingNewPoint, NewFreePoint }
        static StateKind GetStateKind<T>(Either<(Point point, bool isNew), T> state) =>
            state.Match(left => left.isNew ? StateKind.ExistingNewPoint : StateKind.ExistingPoint, right => StateKind.NewFreePoint);

        InputState CreateTwoPointsTool(Vector2 startPoint, Func<Constructor, Point, Point, Entity?> createEntity, DisplayStyle enitityStyle) {
            var point = Surface.HitTest(startPoint).FirstOrDefault();
            var intersectionPoint = point == null ? Surface.HitTestIntersection(startPoint) : null;

            Either<(Point point, bool isNew), (FreePoint point, Vector2 location)> from = point != null || intersectionPoint != null
                ? ((point ?? intersectionPoint)!, intersectionPoint != null).AsLeft()
                : (Surface.Constructor.Point(), startPoint).AsRight();

            Either<(Point point, bool isNew), (FreePoint point, Vector2 location)> 
                ClearToState((Either<(Point point, bool isNew), FreePoint> to, Entity entity) to) {
                if(to.entity != null)
                    Surface.Remove(to.entity);
                return ClearPoint(to.to);
            }
            Either<(Point point, bool isNew), (FreePoint point, Vector2 location)> 
                ClearPoint(Either<(Point point, bool isNew), FreePoint> pointInfo_) {
                Either<(Point point, bool isNew), (FreePoint point, Vector2 location)> pointInfo;
                if(pointInfo_.IsRight()) {
                    var toLocation = Surface.GetPointLocation(pointInfo_.ToRight());
                    Surface.Remove(pointInfo_.ToRight(), keepLocation: true);
                    pointInfo = (pointInfo_.ToRight(), toLocation).AsRight();
                } else {
                    var (point, isNew) = pointInfo_.ToLeft();
                    if(isNew)
                        Surface.Remove(point, keepLocation: true);
                    pointInfo = (point, isNew).AsLeft();
                }
                return pointInfo;
            }
            Either<(Point point, bool isNew), FreePoint> 
                ApplyToState(Either<(Point point, bool isNew), (FreePoint point, Vector2 location)> pointInfo) {
                if(pointInfo.IsRight()) {
                    Surface.Add(pointInfo.ToRight().point, DisplayStyle.Visible);
                    Surface.SetPointLocation(pointInfo.ToRight().point, pointInfo.ToRight().location);
                } else {
                    var (point, isNew) = pointInfo.ToLeft();
                    if(isNew)
                        Surface.Add(point, DisplayStyle.Visible);
                }
                return pointInfo.Match(
                    left: x => Either<(Point point, bool isNew), FreePoint>.Left(x),
                    right: x => Either<(Point point, bool isNew), FreePoint>.Right(x.point));
            }
            bool CanUndo<T>(
                Either<(Point point, bool isNew), FreePoint> from,
                T? to
            ) => to != null || from.IsRight() || from.ToLeft().isNew;

            var transaction = undoManager.Execute(
                (
                    from: from,
                    to: default((Either<(Point point, bool isNew), (FreePoint point, Vector2 location)> pointInfo, Entity? entity)?)
                ),
                redo: state => {
                    ApplyToState(state.from);
                    var toInfo = state.to;
                    Either<(Point point, bool isNew), FreePoint>? to = default; 
                    if(toInfo != null) {
                        to = ApplyToState(toInfo.Value.pointInfo);
                    }
                    if(toInfo!= null && toInfo.Value.entity != null)
                        Surface.Add(toInfo.Value.entity, enitityStyle);

                    var from = state.from.Match(
                        left: x => Either<(Point point, bool isNew), FreePoint>.Left(x),
                        right: x => Either<(Point point, bool isNew), FreePoint>.Right(x.point));
                    return ((
                        from: from,
                        to: toInfo != null ? (to!.Value, toInfo.Value.entity) : null
                    ), CanUndo(from, toInfo));
                },
                undo: state => {
                    var from = ClearPoint(state.from);

                    if(state.to == null) {
                        return (from, null);
                    }

                    var pointInfo = ClearToState(state.to!.Value);
                    return (
                        from,
                        (pointInfo, state.to!.Value.entity)
                    );
                },
                update: ((Either<(Point point, bool isNew), FreePoint> from, (Either<(Point point, bool isNew), FreePoint> to, Entity entity)? to) state, Vector2 offset) => {
                    var toInfo = state.to;
                    if(offset.LengthSquared() < Surface.PointHitTestDistance * Surface.PointHitTestDistance) {
                        if(toInfo != null) {
                            ClearToState(toInfo.Value);
                            toInfo = null;
                        }
                    } else {
                        var fromPoint = state.from.Match(x => x.point, x => x);
                        var newToPoint = toInfo != null ? toInfo.Value.to.Match(x => x.isNew ? x.point : null, x => (FreePoint?)x) : null;
                        var point = Surface
                            .HitTest(startPoint + offset)
                            .Where(x => x != newToPoint && x != fromPoint)
                            .FirstOrDefault();
                        var intersectionPoint = point == null ? Surface.HitTestIntersection(startPoint + offset, except: state.to?.entity.YieldIfNotNull()) : null; //TODO check condition tested
                        if(intersectionPoint == fromPoint)
                            intersectionPoint = null;

                        Either<(Point point, bool isNew), (FreePoint point, Vector2 location)> toPointInfo = point != null || intersectionPoint != null
                            ? ((point ?? intersectionPoint)!, intersectionPoint != null).AsLeft()
                            : (Surface.Constructor.Point(), startPoint + offset).AsRight();

                        var toPoint1 = toPointInfo.Match(left => left.point, right => right.point);
                        var toPoint2 = toInfo != null ? toInfo.Value.to.Match(x => x.point, x => x) : null;

                        if(state.to == null ||
                            (state.to.Value.to != null && (GetStateKind(state.to.Value.to) != GetStateKind(toPointInfo)) || (GetStateKind(toPointInfo) == StateKind.ExistingPoint && toPoint1 != toPoint2))) {
                            if(state.to != null) {
                                ClearToState(state.to.Value);
                                toInfo = null;
                            }
                            var entity = createEntity(Surface.Constructor, fromPoint, toPoint1);
                            if(entity != null && !Surface.Contains(entity)) {
                                var toState = ApplyToState(toPointInfo);
                                Surface.Add(entity, enitityStyle);
                                toInfo = (toState, entity!);
                            }
                        } else {
                            if(toInfo != null && toInfo.Value.to.IsRight())
                                Surface.SetPointLocation(toInfo.Value.to.ToRight(), startPoint + offset);
                        }
                    }
                    return ((state.from, toInfo), CanUndo(state.from, toInfo));
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
