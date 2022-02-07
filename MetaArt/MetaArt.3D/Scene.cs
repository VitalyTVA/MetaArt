using System.Numerics;

namespace MetaArt.D3;

public class Scene<T> {
    List<(Model<T>, Vector3[])> models = new();
    public Scene(params Model<T>[] models) {
        this.models.AddRange(models.Select(x => (x, new Vector3[x.Vertices.Length])));
    }
    public IEnumerable<(int, int, int, int, T, Vector3[])> GetQuads(Camera camera) {
        foreach(var (model, vertices) in models) {
            for(int i = 0; i < model.Vertices.Length; i++) {
                vertices[i] = camera.TranslatePoint(model.GetVertex(i));
            }
        }
        foreach(var (model, vertices) in models) {
            foreach(var (i1, i2, i3, i4, value) in model.Quads) {
                var v1 = vertices[i1];
                var n = Extensions.GetNormal(v1, vertices[i2], vertices[i3]);
                if(Vector3.Dot(v1, n) >= 0) 
                    continue;

                yield return (i1, i2, i3, i4, value, vertices);
            }
        }
    }
}
