using System.Numerics;
using System.Text.RegularExpressions;

namespace MetaArt.D3;
public record struct TriInfo(int Index, int LineIndex);
public record struct LoadOptions<T>(Func<TriInfo, T>? getValue = null, float scale = 1, bool invert = false);
public static class ObjLoader {
    public static Model<T>[] Load<T>(Stream stream, LoadOptions<T> options) {
        return Load<T>(stream, options, out bool _);
    }
    public static Model<T>[] Load<T>(Stream stream, LoadOptions<T> options, out bool allowDuplicateVerticesInSeparateObjects) {
        //TODO optimize
        var getValue = options.getValue ?? (_ => default!);
        using var reader = new StreamReader(stream);
        var vertices = new List<Vector3>();
        var tris = new List<Tri<T>>();
        allowDuplicateVerticesInSeparateObjects = false;
        var models = new List<Model<T>>();
        void CreateModel() => models.Add(new Model<T>(vertices.ToArray(), tris.ToArray()));
        int startIndex = 0;
        int lineIndex = 0;
        while(!reader.EndOfStream) {
            var l = reader.ReadLine();
            lineIndex++;
            if(l.StartsWith("#")) {
                if(l == "#AllowDuplicateVerticesInSeparateObjects")
                    allowDuplicateVerticesInSeparateObjects = true;
                continue;
            }
            if(l.StartsWith("o")) {
                if(vertices.Any())
                    CreateModel();
                startIndex += vertices.Count;
                vertices.Clear();
                tris.Clear();
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
                for(int i = 0; i + 2 < split.Length; i += 1) {
                    var (i1, i2, i3) = (
                        int.Parse(split[0]) - 1 - startIndex,
                        int.Parse(split[i + 1]) - 1 - startIndex,
                        int.Parse(split[i + 2]) - 1 - startIndex
                    );
                    if(options.invert)
                        (i1, i3) = (i3, i1);
                    tris.Add(new Tri<T>(
                        i1, i2, i3,
                        value: getValue(new TriInfo(tris.Count, lineIndex))
                    ));
                }
                //TODO invariant culture to parse
                continue;
            }
        }
        CreateModel();
        return models.ToArray();
    }
}
