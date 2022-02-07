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

                //vertices[i] = camera.TranslatePoint(model.Vertices[i]);
            }
        }
        foreach(var (model, vertices) in models) {
            foreach(var (i1, i2, i3, i4, value) in model.Quads) {
                //var v1  = model.Vertices[i1];
                var v1 = model.GetVertex(i1);
                var n = Extensions.GetNormal(v1, model.GetVertex(i2), model.GetVertex(i3));
                if(!camera.IsVisible(v1, n)) 
                    continue;
                yield return (i1, i2, i3, i4, value, vertices);
            }
        }
    }
}
