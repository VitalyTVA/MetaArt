using MetaArt.Core;
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
                        point = point ?? Surface.Constructor.Point();
                        var transaction = undoManager.CreateTransaction(
                            (point: point, location: isNew ? startPoint : Surface.GetPointLocation(point), isNew: isNew),
                            redo: state => {
                                if(state.isNew) {
                                    Surface.Add(point.AsView(), DisplayStyle.Visible);
                                    Surface.SetPointLocation(point, state.location);
                                    return (state.point, location: default(Vector2), isNew: state.isNew);
                                } else {
                                    var location = Surface.GetPointLocation(state.point);
                                    Surface.SetPointLocation(state.point, state.location);
                                    return (state.point, location, isNew: state.isNew);
                                }
                            },
                            undo: state => {
                                var location = Surface.GetPointLocation(state.point);
                                if(state.isNew) {
                                    Surface.Remove(state.point);
                                    return (state.point, location, state.isNew);
                                } else {
                                    Surface.SetPointLocation(state.point, state.location);
                                    return (state.point, location, state.isNew);
                                }
                            },
                            update: ((FreePoint point, Vector2 location, bool isNew) state, Vector2 offset) => {
                                Surface.SetPointLocation(state.point, startPointLocation + offset);
                            }
                        );
                        if(isNew)
                            transaction.Commit();
                        //var startPointLocation = Surface.GetPointLocation(point);
                        return DragInputState.GetDragState(
                            startPoint,
                            onDrag: offset => {
                                if(offset.LengthSquared() > 0)
                                    transaction.Commit().Update(offset);
                                //Surface.SetPointLocation(point, startPointLocation + offset);
                                return true;
                            }
                        );
                    }

                    if(tool == Tool.Line) {
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


    interface IUndoAction {
        IRedoAction Undo();
    }
    interface IRedoAction {
        IUndoAction Redo();
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

        public UndoActionUpdatable<TDo, TUndo, TUpdate> Execute<TDo, TUndo, TUpdate>(
            TDo data,
            Func<TDo, TUndo> redo,
            Func<TUndo, TDo> undo,
            Action<TUndo, TUpdate> update
        ) {
            var action = new UndoActionUpdatable<TDo, TUndo, TUpdate>(redo(data), redo, undo, update);
            undoStack.Push(action);
            redoStack.Clear();
            return action;
        }

        public Transaction<TDo, TUndo, TUpdate> CreateTransaction<TDo, TUndo, TUpdate>(
            TDo data,
            Func<TDo, TUndo> redo,
            Func<TUndo, TDo> undo,
            Action<TUndo, TUpdate> update
        ) {
            return new Transaction<TDo, TUndo, TUpdate>(this, data, redo, undo, update);
        }
    }

    public class UndoAction<TDo, TUndo> : IUndoAction {
        protected readonly TUndo data;
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

    public class RedoAction<TDo, TUndo> : IRedoAction {
        protected TDo data;
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

    public class UndoActionUpdatable<TDo, TUndo, TUpdate> : UndoAction<TDo, TUndo> {
        readonly Action<TUndo, TUpdate> update;
        public UndoActionUpdatable(TUndo data, Func<TDo, TUndo> redo, Func<TUndo, TDo> undo, Action<TUndo, TUpdate> update) : base(data, redo, undo) {
            this.update = update;
        }
        public void Update(TUpdate data) {
            update(this.data, data);
            //this.data = data;
        }
    }

    public sealed class Transaction<TDo, TUndo, TUpdate>  {
        readonly UndoManager manager;
        readonly TDo data;
        readonly Func<TDo, TUndo> redo;
        readonly Func<TUndo, TDo> undo;
        readonly Action<TUndo, TUpdate> update;

        public Transaction(UndoManager manager, TDo data, Func<TDo, TUndo> redo, Func<TUndo, TDo> undo, Action<TUndo, TUpdate> update) {
            this.manager = manager;
            this.data = data;
            this.redo = redo;
            this.undo = undo;
            this.update = update;
        }

        UndoActionUpdatable<TDo, TUndo, TUpdate>? action;

        public UndoActionUpdatable<TDo, TUndo, TUpdate> Commit() {
            if(action == null) {
                action = manager.Execute(data, redo, undo, update);
            }
            return action;
        }
    }

}
