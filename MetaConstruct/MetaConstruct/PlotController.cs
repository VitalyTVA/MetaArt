using MetaArt.Core;
using System.Numerics;

namespace MetaConstruct {
    public enum Tool { Pointer, Point, Line, Circle, LineSegment, CircleSegment }
    public class PlotController {
        Engine engine;
        public Surface Surface { get; private set; } = null!;
        public Scene scene => engine.scene;
        Tool tool;
        public PlotController(int width, int height) {
            engine = new Engine(width, height);
            scene.AddElement(new PlotElement() { 
                Rect = new Rect(0, 0, width, height),
                GetPressState = startPoint => {
                    if(tool == Tool.Point) {
                        var point = Surface.HitTest(startPoint).OfType<FreePoint>().FirstOrDefault();
                        if(point == null) {
                            point = Surface.Constructor.Point();
                            Surface.Add(point.AsView(), DisplayStyle.Visible);
                            Surface.SetPointLocation(point, startPoint);
                        }
                        var startPointLocation = Surface.GetPointLocation(point);
                        return DragInputState.GetDragState(
                            startPoint,
                            onDrag: offset => {
                                Surface.SetPointLocation(point, startPointLocation + offset);
                                return true;
                            }
                        );
                    }

                    if(tool == Tool.Line) {
                        var fromPoint = Surface.HitTest(startPoint)/*.OfType<FreePoint>()*/.FirstOrDefault();
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
                                    .Where(x => x != toPoint /*&& x != fromPoint*/)
                                    .FirstOrDefault();
                                if(point != null) {
                                    //if(toPoint != null && line != null) {
                                    //    Surface.Remove(toPoint);
                                    //    toPoint = null;
                                    //    Surface.Remove(line);
                                    //    line = null;
                                    //}
                                    //if(line == null) {
                                    //    line = Surface.Constructor.Line(fromPoint, point);
                                    //    Surface.Add(line, DisplayStyle.Background);
                                    //}
                                } else {
                                    //if(line != null && toPoint == null) {
                                    //    Surface.Remove(line);
                                    //    line = null;
                                    //}
                                    if(/*offset.LengthSquared() > 25 && */toPoint == null) {
                                        toPoint = Surface.Constructor.Point();
                                        Surface.Add(toPoint.AsView(), DisplayStyle.Visible);
                                        line = Surface.Constructor.Line(fromPoint, toPoint);
                                        Surface.Add(line, DisplayStyle.Background);
                                    }
                                    //if(toPoint != null)
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
}
