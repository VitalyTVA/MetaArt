using System.Numerics;
using System.Text.RegularExpressions;

namespace MetaArt.D3;

public static class ObjLoader {
    public static IEnumerable<Model<VoidType>> Load(Stream stream) {
        //TODO optimize
        using var reader = new StreamReader(stream);
        var vertices = new List<Vector3>();
        var quads = new List<Quad<VoidType>>();
        Model<VoidType> CreateModel() => new Model<VoidType>(vertices.ToArray(), quads.ToArray());
        int startIndex = 0;
        while(!reader.EndOfStream) {
            var l = reader.ReadLine();
            if(l.StartsWith("#"))
                continue;
            if(l.StartsWith("o")) {
                if(vertices.Any())
                    yield return CreateModel();
                startIndex += vertices.Count;
                vertices.Clear();
                quads.Clear();
                continue;
            }
            if(l.StartsWith("s")) {
                if(l != "s off")
                    throw new NotSupportedException();
                continue;
            }
            if(l.StartsWith("v")) {
                var split = l.Split(' ');
                //TODO invariant culture to parse
                vertices.Add(new Vector3(float.Parse(split[1]), float.Parse(split[2]), -float.Parse(split[3])));
                continue;
            }
            if(l.StartsWith("f")) {
                var split = l.Split(' ');
                //TODO invariant culture to parse
                quads.Add(new Quad<VoidType>(
                    int.Parse(split[1]) - 1 - startIndex, 
                    int.Parse(split[2]) - 1 - startIndex, 
                    int.Parse(split[3]) - 1 - startIndex, 
                    int.Parse(split.Length == 5 ? split[4] : split[3]) - 1 - startIndex, 
                default));
                continue;
            }
        }
        yield return CreateModel();
    }
}
