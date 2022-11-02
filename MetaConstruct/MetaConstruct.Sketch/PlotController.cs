using MetaArt.Core;
using System.Numerics;

namespace MetaConstruct {
    public class PlotController {
        Engine engine;
        public Surface Surface { get; private set; } = null!;
        public Scene scene => engine.scene;
        public PlotController(int width, int height) {
            engine = new Engine(width, height);
            scene.AddElement(new PlotElement() { 
                Rect = new Rect(0, 0, width, height),
                GetPressState = startPoint => {
                    var point = Surface.HitTest(startPoint);
                    if(point == null)
                        return null;
                    var startPointLocation = Surface.GetPointLocation(point);
                    return DragInputState.GetDragState(
                        startPoint,
                        onDrag: offset => {
                            Surface.SetPointLocation(point, startPointLocation + offset);
                            return true;
                        }
                    );
                }
            });
        }
        public void Load(Surface surface) {
            this.Surface = surface;
        }
    }

    class PlotElement : Element {
        public PlotElement() {
            HitTestVisible = true;
        }
    }
}
