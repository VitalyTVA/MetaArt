using System.Reflection;

public class MetaCostructSketches : ISkecthesProvider {
    ICollection<SketchGroup> ISkecthesProvider.Groups => new[] {
        SketchGroup.FromNamespace("Construct", Assembly.GetExecutingAssembly()),
    };
}