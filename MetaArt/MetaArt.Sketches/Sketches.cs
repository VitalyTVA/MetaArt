using System.Reflection;

public class Sketches : ISkecthesProvider {
    ICollection<SketchGroup> ISkecthesProvider.Groups => new[] {
        SketchGroup.FromNamespace("SpringsPhysics", Assembly.GetExecutingAssembly()),
        SketchGroup.FromNamespace("Meta", Assembly.GetExecutingAssembly()),
        SketchGroup.FromNamespace("Interaction", Assembly.GetExecutingAssembly()),
        SketchGroup.FromNamespace("Motion", Assembly.GetExecutingAssembly()),
        SketchGroup.FromNamespace("Input", Assembly.GetExecutingAssembly()),
        SketchGroup.FromNamespace("Vectors", Assembly.GetExecutingAssembly()),
        SketchGroup.FromNamespace("Fractals", Assembly.GetExecutingAssembly()),
        SketchGroup.FromNamespace("Simulate", Assembly.GetExecutingAssembly()),

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
        SketchGroup.FromNamespace("TestSketches", Assembly.GetExecutingAssembly()),
    };
}