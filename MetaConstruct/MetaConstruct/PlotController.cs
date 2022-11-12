﻿using MetaArt.Core;
using MetaCore;
using System.Numerics;

namespace MetaConstruct {
    public enum Tool { Pointer, Point, Line, Circle, LineSegment, CircleSegment }
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
                        var startPointLocation = point != null ? Surface.GetPointLocation(point) : startPoint;
                        bool isNew = point == null;
                        var transaction = isNew
                            ? undoManager.CreateTransaction(
                                (point: Surface.Constructor.Point(), location: startPoint),
                                redo: state => {
                                    Surface.Add(state.point.AsView(), DisplayStyle.Visible);
                                    Surface.SetPointLocation(state.point, state.location);
                                    return state.point;
                                },
                                undo: statePoint => {
                                    var location = Surface.GetPointLocation(statePoint);
                                    Surface.Remove(statePoint);
                                    return (statePoint, location);
                                },
                                update: (FreePoint statePoint, Vector2 offset) => {
                                    Surface.SetPointLocation(statePoint, startPointLocation + offset);
                                    return statePoint;
                                }
                            )
                            : undoManager.CreateTransaction(
                                (point: point!, location: Surface.GetPointLocation(point!)),
                                redo: state => {
                                    var location = Surface.GetPointLocation(state.point);
                                    Surface.SetPointLocation(state.point, state.location);
                                    return (state.point, location);
                                },
                                undo: state => {
                                    var location = Surface.GetPointLocation(state.point);
                                    Surface.SetPointLocation(state.point, state.location);
                                    return (state.point, location);
                                },
                                update: ((FreePoint point, Vector2 location) state, Vector2 offset) => {
                                    Surface.SetPointLocation(state.point, startPointLocation + offset);
                                    return state;
                                }
                            );
                        if(isNew)
                            transaction.Commit();
                        return DragInputState.GetDragState(
                            startPoint,
                            onDrag: offset => {
                                if(offset.LengthSquared() > 0)
                                    transaction.Commit().Update(offset);
                                return true;
                            }
                        );
                    }

                    if(tool == Tool.Line) {
                        var point = Surface.HitTest(startPoint).FirstOrDefault();

                        Either<Point, (FreePoint point, Vector2 location)> from = point != null
                            ? point.AsLeft()
                            : (Surface.Constructor.Point(), startPoint).AsRight();

                        var transaction = undoManager.CreateTransaction(
                            (
                                from: from, 
                                to: default((FreePoint point, Line line, Vector2 location)?)
                            ),
                            redo: state => {
                                Point? fromPoint = null;
                                switch(state.from) {
                                    case (Point existingPoint, null):
                                        fromPoint = existingPoint;
                                        break;
                                    case (null, (FreePoint newPoint, Vector2 location)):
                                        fromPoint = newPoint;
                                        Surface.Add(newPoint.AsView(), DisplayStyle.Visible);
                                        Surface.SetPointLocation(newPoint, location);
                                        break;
                                }

                                var toInfo = state.to;
                                if(toInfo != null) {
                                    Surface.Add(toInfo.Value.point.AsView(), DisplayStyle.Visible);
                                    Surface.SetPointLocation(toInfo.Value.point, toInfo.Value.location);
                                    Surface.Add(toInfo.Value.line, DisplayStyle.Background);

                                }

                                return (
                                    from: (point: fromPoint!, isNew: state.from.IsRight()),
                                    to: toInfo != null ? (toInfo.Value.point, toInfo.Value.line) : null
                                );
                            },
                            undo: state => {
                                var toLocation = Surface.GetPointLocation(state.to!.Value.point);

                                Either<Point, (FreePoint point, Vector2 location)> from;
                                if(state.from.isNew) {
                                    from = ((FreePoint)state.from.point, Surface.GetPointLocation((FreePoint)state.from.point)).AsRight();
                                    Surface.Remove(state.from.point);
                                } else {
                                    from = state.from.point.AsLeft();
                                }
                                Surface.Remove(state.to!.Value.point);
                                Surface.Remove(state.to!.Value.line);

                                return (
                                    from, 
                                    (state.to!.Value.point, state.to!.Value.line, toLocation)
                                );
                            },
                            update: (((Point point, bool isNew) from, (FreePoint point, Line line)? to) state, Vector2 offset) => {
                                var toInfo = state.to;
                                if(toInfo == null) {
                                    var pointTo = Surface.Constructor.Point();
                                    Surface.Add(pointTo.AsView(), DisplayStyle.Visible);
                                    var line = Surface.Constructor.Line(state.from.point, pointTo);
                                    Surface.Add(line, DisplayStyle.Background);

                                    toInfo = (pointTo, line);
                                }
                                Surface.SetPointLocation(toInfo.Value.point, startPoint + offset);
                                return (state.from, toInfo);
                            }
                        );
                        transaction.Commit();
                        return DragInputState.GetDragState(
                            startPoint,
                            onDrag: offset => {
                                //if(offset.LengthSquared() > 0)
                                    transaction.Commit().Update(offset);
                                return true;
                            }
                        );

                        /*
                        var fromPoint = Surface.HitTest(startPoint).FirstOrDefault();
                        if(fromPoint == null) {
                            var newPoint = Surface.Constructor.Point();
                            Surface.Add(newPoint.AsView(), DisplayStyle.Visible);
                            Surface.SetPointLocation(newPoint, startPoint);
                            fromPoint = newPoint;
                        }
                        FreePoint? toPoint = null;
                        Line? line = null;
                        return DragInputState.GetDragState(
                            startPoint,
                            onDrag: offset => {
                                var point = Surface
                                    .HitTest(startPoint + offset)
                                    .Where(x => x != toPoint && x != fromPoint)
                                    .FirstOrDefault();
                                if(point != null) {
                                    if(toPoint != null && line != null) {
                                        Surface.Remove(toPoint);
                                        toPoint = null;
                                        Surface.Remove(line);
                                        line = null;
                                    }
                                    if(line == null) {
                                        line = Surface.Constructor.Line(fromPoint, point);
                                        Surface.Add(line, DisplayStyle.Background);
                                    }
                                } else {
                                    if(line != null && toPoint == null) {
                                        Surface.Remove(line);
                                        line = null;
                                    }
                                    if(offset.LengthSquared() > Surface.PointHitTestDistance * Surface.PointHitTestDistance && toPoint == null) {
                                        toPoint = Surface.Constructor.Point();
                                        Surface.Add(toPoint.AsView(), DisplayStyle.Visible);
                                        line = Surface.Constructor.Line(fromPoint, toPoint);
                                        Surface.Add(line, DisplayStyle.Background);
                                    }
                                    if(toPoint != null)
                                        Surface.SetPointLocation(toPoint, startPoint + offset);
                                }
                                return true;
                            }
                        );
                        */
                    }

                    throw new NotImplementedException();
                }
            });
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

    public interface IUpdatableTransaction<TUpdate> {
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
        //public UndoAction<TDo, TUndo> Execute<TDo, TUndo>(
        //    TDo data, 
        //    Func<TDo, TUndo> redo,
        //    Func<TUndo, TDo> undo
        //) {
        //    return new UndoAction<TDo, TUndo>(redo(data), redo, undo);
        //}

        IUpdatableTransaction<TUpdate> Execute<TDo, TUndo, TUpdate>(
            TDo data,
            Func<TDo, TUndo> redo,
            Func<TUndo, TDo> undo,
            Func<TUndo, TUpdate, TUndo> update
        ) {
            var action = new UndoActionUpdatable<TDo, TUndo, TUpdate>(redo(data), redo, undo, update);
            undoStack.Push(action);
            redoStack.Clear();
            return action;
        }

        public ITransaction<TUpdate> CreateTransaction<TDo, TUndo, TUpdate>(
            TDo data,
            Func<TDo, TUndo> redo,
            Func<TUndo, TDo> undo,
            Func<TUndo, TUpdate, TUndo> update
        ) {
            return new Transaction<TDo, TUndo, TUpdate>(this, data, redo, undo, update);
        }

        interface IUndoAction {
            IRedoAction Undo();
        }
        interface IRedoAction {
            IUndoAction Redo();
        }

        sealed class Transaction<TDo, TUndo, TUpdate> : ITransaction<TUpdate> {
            readonly UndoManager manager;
            readonly TDo data;
            readonly Func<TDo, TUndo> redo;
            readonly Func<TUndo, TDo> undo;
            readonly Func<TUndo, TUpdate, TUndo> update;

            public Transaction(UndoManager manager, TDo data, Func<TDo, TUndo> redo, Func<TUndo, TDo> undo, Func<TUndo, TUpdate, TUndo> update) {
                this.manager = manager;
                this.data = data;
                this.redo = redo;
                this.undo = undo;
                this.update = update;
            }

            IUpdatableTransaction<TUpdate>? action;

            IUpdatableTransaction<TUpdate> ITransaction<TUpdate>.Commit() {
                if(action == null) {
                    action = manager.Execute(data, redo, undo, update);
                }
                return action;
            }
        }
        class UndoAction<TDo, TUndo> : IUndoAction {
            protected TUndo data;
            readonly Func<TDo, TUndo> redo;
            readonly Func<TUndo, TDo> undo;
            public UndoAction(TUndo data, Func<TDo, TUndo> redo, Func<TUndo, TDo> undo) {
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
            readonly Func<TDo, TUndo> redo;
            readonly Func<TUndo, TDo> undo;
            public RedoAction(TDo data, Func<TDo, TUndo> redo, Func<TUndo, TDo> undo) {
                this.data = data;
                this.redo = redo;
                this.undo = undo;
            }
            IUndoAction IRedoAction.Redo() {
                var undoData = redo(data);
                return new UndoAction<TDo, TUndo>(undoData, redo, undo);
            }
        }

        class UndoActionUpdatable<TDo, TUndo, TUpdate> : UndoAction<TDo, TUndo>, IUpdatableTransaction<TUpdate> {
            readonly Func<TUndo, TUpdate, TUndo> update;
            public UndoActionUpdatable(TUndo data, Func<TDo, TUndo> redo, Func<TUndo, TDo> undo, Func<TUndo, TUpdate, TUndo> update) 
                : base(data, redo, undo) {
                this.update = update;
            }
            public void Update(TUpdate data) {
                this.data = update(this.data, data);
            }
        }
    }

    public interface ITransaction<TUpdate> {
        IUpdatableTransaction<TUpdate> Commit();
    }
}
