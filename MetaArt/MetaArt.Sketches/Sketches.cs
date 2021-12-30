public class Sketches : ISkecthesProvider {
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
                new SkecthInfo(typeof(Rects), description: "Creating a pattern with randomly placed rectangles."),
                new SkecthInfo(typeof(LineToCircle), description: "Animating from a line to a circle using a polygon function."),
                new SkecthInfo(typeof(Hexagon), description: "Repeating shapes to make a looping hexagonal design."),
                new SkecthInfo(typeof(Rings), description: "Using the arc() function to build concentric rings."),
                new SkecthInfo(typeof(Lines), description: "Animating grid of line segments with sin."),
                new SkecthInfo(typeof(Lewitt), description: "Recreating Sol LeWitt 'Wall Drawing #130' from 1972."),
                new SkecthInfo(typeof(Substrate), description: "The advanced example, implementing Jared Tarbell's 'Substrate' algorithm."),
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
        new SketchGroup {
            Name = "Input",
            Sketches = new[] {
                new SkecthInfo(typeof(Clock)),
                new SkecthInfo(typeof(Constrain)),
                new SkecthInfo(typeof(StoringInput)),
            }
        },
    };
}