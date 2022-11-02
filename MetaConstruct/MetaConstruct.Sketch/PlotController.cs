using MetaArt.Core;
using System.Numerics;

namespace MetaConstruct {
    public class PlotController {
        Engine engine;
        public Surface Surface { get; private set; } = null!;
        public Scene scene => engine.scene;
        public PlotController(int width, int height) {
            engine = new Engine(width, height);
        }
        public void Load(Surface surface) {
            this.Surface = surface;
        }
    }
}
