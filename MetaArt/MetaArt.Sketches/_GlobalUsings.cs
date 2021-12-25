global using MetaArt;
global using System;
global using System.Linq;
global using System.Collections.Generic;

class SketchesProvider : ISkecthesProvider {
    ICollection<SketchGroup> ISkecthesProvider.Groups => new[] {
        new SketchGroup {
            Name = "p5.js demos",
            Sketches = new[] {
                new SkecthInfo(typeof(Circle), description: "A simple circle."),
                new SkecthInfo(typeof(Shape), description: "Shape and stroke operations."),
                new SkecthInfo(typeof(TransformAndBlend), description: "Using transform and blend mode."),
                new SkecthInfo(typeof(Loop), description: "Creating a seamless loop with a fixed duration."),
                new SkecthInfo(typeof(Rotate), description: "Rotating shapes around their center."),
                new SkecthInfo(typeof(Lerp), description: "Using linear interpolation (lerp) for animation."),
                new SkecthInfo(typeof(Repeat), description: "Repeating shape with a loop and linearly spacing them."),
                new SkecthInfo(typeof(Trigonometry), description: "Using trigonometry to create radial motion."),
                new SkecthInfo(typeof(Mouse), description: "Simple interpolation with mouse movement."),
            }
        },
        new SketchGroup {
            Name = "test",
            Sketches = new[] {
                new SkecthInfo(typeof(BackgroundAnimation)),
                new SkecthInfo(typeof(FPS)),
                new SkecthInfo(typeof(HeavyAnimations)),
                new SkecthInfo(typeof(MousePressed)),
                new SkecthInfo(typeof(Performance)),
                new SkecthInfo(typeof(PMouse)),
                new SkecthInfo(typeof(SetBackgroundInSetup)),
            }
        },
    };
}