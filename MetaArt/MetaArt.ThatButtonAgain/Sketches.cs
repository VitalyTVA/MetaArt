using System.Reflection;
using ThatButtonAgain;

public class ThatButtonSketches : ISkecthesProvider {
    ICollection<SketchGroup> ISkecthesProvider.Groups => new[] {
        new SketchGroup {
            Name = "That Button Again",
            Sketches = Enumerable
                .Range(0, GameController.Levels.Length)
                .Select(i => new SkecthInfo(typeof(Level), parameters: new object[] { i }, name: "Level " + i))
                .ToArray()
        },
    };
}