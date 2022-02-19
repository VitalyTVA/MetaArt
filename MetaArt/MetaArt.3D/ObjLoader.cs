using System.Numerics;
using System.Text.RegularExpressions;

namespace MetaArt.D3;
public record struct QuadInfo(int Index, int LineIndex);
public record struct LoadOptions<T>(Func<QuadInfo, T>? getValue = null, float scale = 1, bool invert = false);
public static class ObjLoader {
    public static IEnumerable<Model<T>> Load<T>(Stream stream, LoadOptions<T> options) {
        //TODO optimize
        var getValue = options.getValue ?? (_ => default!);
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
                vertices.Add(new Vector3(
                    float.Parse(split[1]) * options.scale, 
                    float.Parse(split[2]) * options.scale, 
                    -float.Parse(split[3]) * options.scale
                ));
                continue;
            }
            if(l.StartsWith("f")) {
                var split = l.Substring(2).Split(' ');
                for(int i = 0; i + 2 < split.Length; i += 2) {
                    var (i1, i2, i3, i4) = (
                        int.Parse(split[0]) - 1 - startIndex,
                        int.Parse(split[i + 1]) - 1 - startIndex,
                        int.Parse(split[i + 2]) - 1 - startIndex,
                        int.Parse(i + 3 < split.Length ? split[i + 3] : split[i + 2]) - 1 - startIndex
                    );
                    if(options.invert)
                        (i1, i2, i3, i4) = i3 != i4 ? (i4, i3, i2, i1) : (i3, i2, i1, i1);
                    quads.Add(new Quad<T>(
                        i1, i2, i3,
                        value: getValue(new QuadInfo(quads.Count, lineIndex))
                    ));
                    if(i3 != i4) {
                        quads.Add(new Quad<T>(
                            i1, i3, i4,
                            value: getValue(new QuadInfo(quads.Count, lineIndex))
                        ));
                    }
                }
                //TODO invariant culture to parse
                continue;
            }
        }
        yield return CreateModel();
    }
}
