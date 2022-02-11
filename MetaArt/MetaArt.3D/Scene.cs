using System.Numerics;
using static MetaArt.D3.MathF;

namespace MetaArt.D3;

public class Scene<T> {
    List<(Model<T> model, Vector3[] vertices)> models = new();
    QuadRef<T>[] quads;

    public IEnumerable<Model<T>> GetModels() => models.Select(x => x.model);
    
    public Scene(params Model<T>[] models) {
        this.models.AddRange(models.Select(x => (x, new Vector3[x.Vertices.Length])));
        quads = models.SelectMany((x, j) => Enumerable.Range(0, x.Quads.Length).Select(i => new QuadRef<T>(j, i))).ToArray();
    }
    public IEnumerable<(int, int, int, int, T, Vector3[])> GetQuads(Camera camera) {
        foreach(var (model, vertices) in models) {
            for(int i = 0; i < model.Vertices.Length; i++) {
                vertices[i] = camera.TranslatePoint(model.GetVertex(i));
            }
        }
        Array.Sort(quads, (x, y) => {
            return Comparer<float>.Default.Compare(
                GetBox(y).max.Z, 
                GetBox(x).max.Z
            );
        });
        for(int i = 0; i < quads.Length; i++) {

        }
        foreach(var (modelIndex, quadIndex) in quads) {
            var (model, vertices) = models[modelIndex];
            var (i1, i2, i3, i4, value) = model.Quads[quadIndex];
            var v1 = vertices[i1];
            var n = Extensions.GetNormal(v1, vertices[i2], vertices[i3]);
            if(Vector3.Dot(v1, n) >= 0) 
                continue;
            yield return (i1, i2, i3, i4, value, vertices);
        }
    }

    float GetQuadZ(QuadRef<T> x) {
        var (model, vertices) = models[x.modelIndex];
        return vertices[model.Quads[x.quadIndex].i1].Z;
    }

    (Vector3 min, Vector3 max) GetBox(QuadRef<T> x) {
        var (model, vertices) = models[x.modelIndex];
        var v1 = vertices[model.Quads[x.quadIndex].i1];
        var v2 = vertices[model.Quads[x.quadIndex].i2];
        var v3 = vertices[model.Quads[x.quadIndex].i3];
        var v4 = vertices[model.Quads[x.quadIndex].i4];
        return new(
            new Vector3(Min4(v1.X, v2.X, v3.X, v4.X), Min4(v1.Y, v2.Y, v3.Y, v4.Y), Min4(v1.Z, v2.Z, v3.Z, v4.Z)),
            new Vector3(Max4(v1.X, v2.X, v3.X, v4.X), Max4(v1.Y, v2.Y, v3.Y, v4.Y), Max4(v1.Z, v2.Z, v3.Z, v4.Z))
        );
    }
    static float Max4(float f1, float f2, float f3, float f4) => Max(f1, Max(f2, Max(f3, f4)));
    static float Min4(float f1, float f2, float f3, float f4) => Min(f1, Min(f2, Min(f3, f4)));
}
public record struct QuadRef<T>(int modelIndex, int quadIndex);
