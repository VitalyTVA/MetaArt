using System.Numerics;

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
                GetQuadZ(y), 
                GetQuadZ(x)
            );
        });
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
}
public record struct QuadRef<T>(int modelIndex, int quadIndex);
