using MetaConstruct;
using System.Reflection;

public class MetaCostructSketches : ISkecthesProvider {
    ICollection<SketchGroup> ISkecthesProvider.Groups => new[] {
        new SketchGroup {
            Name = "That Button Again",
            Sketches = PlotsCollection.Plots
                .Select((x, i) => new SkecthInfo(
                    typeof(Plot),
                    parameters: new object[] { x.action },
                    name: i + " - " + x.name
                ))
                .ToArray()
        }
    };
}