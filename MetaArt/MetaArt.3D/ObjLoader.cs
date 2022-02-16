using System.Numerics;
using System.Text.RegularExpressions;

namespace MetaArt.D3;
public record struct QuadInfo(int Index, int LineIndex);
public static class ObjLoader {
    public static IEnumerable<Model<T>> Load<T>(Stream stream, Func<QuadInfo, T>? getValue = null) {
        //TODO optimize
        getValue = getValue ?? (_ => default!);
        using var reader = new StreamReader(stream);
        var vertices = new List<Vector3>();
        var quads = new List<Quad<T>>();
        Model<T> CreateModel() => new Model<T>(vertices.ToArray(), quads.ToArray());
        int startIndex = 0;
        int lineIndex = 0;
        while(!reader.EndOfStream) {
            var l = reader.ReadLine();
            lineIndex++;
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
                quads.Add(new Quad<T>(
                    int.Parse(split[1]) - 1 - startIndex, 
                    int.Parse(split[2]) - 1 - startIndex, 
                    int.Parse(split[3]) - 1 - startIndex, 
                    int.Parse(split.Length == 5 ? split[4] : split[3]) - 1 - startIndex, 
                    value: getValue(new QuadInfo(quads.Count, lineIndex))));
                continue;
            }
        }
        yield return CreateModel();
    }
}
