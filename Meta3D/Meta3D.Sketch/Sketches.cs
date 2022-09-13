using System.Reflection;

public class Meta3DSketches : ISkecthesProvider {
    ICollection<SketchGroup> ISkecthesProvider.Groups => new[] {
        SketchGroup.FromNamespace("D3", Assembly.GetExecutingAssembly()),
    };
}