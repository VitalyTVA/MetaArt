using System.Numerics;
using static MetaArt.D3.MathFEx;

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
            var (x1, x2, x3, x4) = GetVertices(x);
            var (y1, y2, y3, y4) = GetVertices(y);
            return Comparer<float>.Default.Compare(
                GetBox(y1, y2, y3, y4).max.Z,
                GetBox(x1, x2, x3, x4).max.Z 
            );
        });

        int steps = 0;
        for(int pi = 0; pi < quads.Length; pi++) {
            var (p1, p2, p3, p4) = GetVertices(quads[pi]);
            var pBox = GetBox(p1, p2, p3, p4);
            bool writeP = pi == quads.Length - 1;
            int qi = pi + 1;
            for(qi = pi + 1; qi < quads.Length; qi++) {
                steps++;
                if(steps > 1000)
                    throw new InvalidOperationException("Cycle detected");
                var (q1, q2, q3, q4) = GetVertices(quads[qi]);
                var qBox = GetBox(q1, q2, q3, q4);
                if(pBox.min.Z >= qBox.max.Z) {
                    writeP = true;
                    break;
                }
                if(RangesAreApart((pBox.min.X, pBox.max.X), (qBox.min.X, qBox.max.X))
                    || RangesAreApart((pBox.min.Y, pBox.max.Y), (qBox.min.Y, qBox.max.Y))
                    || Extensions.VerticesOnDifferentSideOfPlaneWithCamera((q1, q2, q3), (p1, p2, p3, p4), camera.Location)
                    || Extensions.VerticesOnSameSideOfPlaneWithCamera((p1, p2, p3), (q1, q2, q3, q4), camera.Location)
                ) {
                    continue;
                }
                var t = quads[pi];
                quads[pi] = quads[qi];
                quads[qi] = t;
                pi--;
                break;

                //throw new NotImplementedException();
            }
            if(!writeP)
                writeP = qi == quads.Length;
            if(writeP) {
                var quad = quads[pi];
                var (model, vertices) = models[quad.modelIndex];
                var (i1, i2, i3, i4, value) = model.Quads[quad.quadIndex];
                var v1 = vertices[i1];
                var n = Extensions.GetNormal(v1, vertices[i2], vertices[i3]);
                if(Vector3.Dot(v1, n) >= 0)
                    continue;
                yield return (i1, i2, i3, i4, value, vertices);
            }
        }
        //foreach(var (modelIndex, quadIndex) in quads) {
        //    var (model, vertices) = models[modelIndex];
        //    var (i1, i2, i3, i4, value) = model.Quads[quadIndex];
        //    var v1 = vertices[i1];
        //    var n = Extensions.GetNormal(v1, vertices[i2], vertices[i3]);
        //    if(Vector3.Dot(v1, n) >= 0) 
        //        continue;
        //    yield return (i1, i2, i3, i4, value, vertices);
        //}
    }

    (Vector3, Vector3, Vector3, Vector3) GetVertices(QuadRef<T> x) {
        var (model, vertices) = models[x.modelIndex];
        var v1 = vertices[model.Quads[x.quadIndex].i1];
        var v2 = vertices[model.Quads[x.quadIndex].i2];
        var v3 = vertices[model.Quads[x.quadIndex].i3];
        var v4 = vertices[model.Quads[x.quadIndex].i4];
        return (v1, v2, v3, v4);
    }
    (Vector3 min, Vector3 max) GetBox(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        return new(
            new Vector3(Min4(v1.X, v2.X, v3.X, v4.X), Min4(v1.Y, v2.Y, v3.Y, v4.Y), Min4(v1.Z, v2.Z, v3.Z, v4.Z)),
            new Vector3(Max4(v1.X, v2.X, v3.X, v4.X), Max4(v1.Y, v2.Y, v3.Y, v4.Y), Max4(v1.Z, v2.Z, v3.Z, v4.Z))
        );
    }
    static float Max4(float f1, float f2, float f3, float f4) => Max(f1, Max(f2, Max(f3, f4)));
    static float Min4(float f1, float f2, float f3, float f4) => Min(f1, Min(f2, Min(f3, f4)));
}
public record struct QuadRef<T>(int modelIndex, int quadIndex);
