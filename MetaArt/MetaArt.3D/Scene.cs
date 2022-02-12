using System.Numerics;
using static MetaArt.D3.MathFEx;

namespace MetaArt.D3;

public class Scene<T> {
    List<(Model<T> model, Vector3[] vertices, Vector3[] normalVertices)> models = new();
    QuadRef<T>[] quads;

    public IEnumerable<Model<T>> GetModels() => models.Select(x => x.model);
    
    public Scene(params Model<T>[] models) {
        this.models.AddRange(models.Select(x => (x, new Vector3[x.Vertices.Length], new Vector3[x.Vertices.Length])));
        quads = models.SelectMany((x, j) => Enumerable.Range(0, x.Quads.Length).Select(i => new QuadRef<T>(j, i))).ToArray();
    }
    public IEnumerable<(int, int, int, int, T, Vector3[])> GetQuads(Camera camera) {
        foreach(var (model, vertices, normalVertices) in models) {
            for(int i = 0; i < model.Vertices.Length; i++) {
                normalVertices[i] = camera.TranslatePoint(model.GetVertex(i));
                vertices[i] = camera.ToScreenCoords3(normalVertices[i]);
            }
        }
        Array.Sort(quads, (x, y) => {
            return Comparer<float>.Default.Compare(
                GetBox(y).max.Z,
                GetBox(x).max.Z 
            );
        });

        int steps = 0;
        for(int pi = 0; pi < quads.Length; pi++) {
            var (p1, p2, p3, p4) = GetNormalVertices(quads[pi]);
            var pBox = GetBox(quads[pi]);
            bool writeP = pi == quads.Length - 1;
            int qi = pi + 1;
            for(qi = pi + 1; qi < quads.Length; qi++) {
                steps++;
                if(steps > 1000)
                    throw new InvalidOperationException("Cycle detected");
                var (q1, q2, q3, q4) = GetNormalVertices(quads[qi]);
                var qBox = GetBox(quads[qi]);
                if(pBox.min.Z >= qBox.max.Z) {
                    writeP = true;
                    break;
                }
                if(RangesAreApart((pBox.min.X, pBox.max.X), (qBox.min.X, qBox.max.X))
                    || RangesAreApart((pBox.min.Y, pBox.max.Y), (qBox.min.Y, qBox.max.Y))
                    || Extensions.VerticesOnSameSideOfPlane((q1, q2, q3), (p1, p2, p3, p4), camera.Location, cameraOnSameSide: false)
                    || Extensions.VerticesOnSameSideOfPlane((p1, p2, p3), (q1, q2, q3, q4), camera.Location, cameraOnSameSide: true)
                ) {
                    continue;
                }
                var t = quads[pi];
                quads[pi] = quads[qi];
                quads[qi] = t;
                pi--;
                break;
            }
            if(!writeP)
                writeP = qi == quads.Length;
            if(writeP) {
                var quad = quads[pi];
                var (model, vertices, normalVertices) = models[quad.modelIndex];
                var (i1, i2, i3, i4, value) = model.Quads[quad.quadIndex];
                var v1 = normalVertices[i1];
                var n = Extensions.GetNormal(v1, normalVertices[i2], normalVertices[i3]);
                if(Vector3.Dot(v1, n) >= 0)
                    continue;
                yield return (i1, i2, i3, i4, value, vertices);
            }
        }

        //foreach(var (modelIndex, quadIndex) in quads) {
        //    var (model, vertices, normalVertices) = models[modelIndex];
        //    var (i1, i2, i3, i4, value) = model.Quads[quadIndex];
        //    var v1 = normalVertices[i1];
        //    var n = Extensions.GetNormal(v1, normalVertices[i2], normalVertices[i3]);
        //    if(Vector3.Dot(v1, n) >= 0)
        //        continue;
        //    yield return (i1, i2, i3, i4, value, vertices);
        //}
    }

    (Vector3, Vector3, Vector3, Vector3) GetVertices(QuadRef<T> x) {
        var (model, vertices, _) = models[x.modelIndex];
        var v1 = vertices[model.Quads[x.quadIndex].i1];
        var v2 = vertices[model.Quads[x.quadIndex].i2];
        var v3 = vertices[model.Quads[x.quadIndex].i3];
        var v4 = vertices[model.Quads[x.quadIndex].i4];
        return (v1, v2, v3, v4);
    }
    (Vector3, Vector3, Vector3, Vector3) GetNormalVertices(QuadRef<T> x) {
        var (model, _, normalVertices) = models[x.modelIndex];
        var v1 = normalVertices[model.Quads[x.quadIndex].i1];
        var v2 = normalVertices[model.Quads[x.quadIndex].i2];
        var v3 = normalVertices[model.Quads[x.quadIndex].i3];
        var v4 = normalVertices[model.Quads[x.quadIndex].i4];
        return (v1, v2, v3, v4);
    }
    (Vector3 min, Vector3 max) GetBox(QuadRef<T> x) {
        var (v1, v2, v3, v4) = GetVertices(x);
        var (vn1, vn2, vn3, vn4) = GetNormalVertices(x);
        return new(
            new Vector3(Min4(v1.X, v2.X, v3.X, v4.X), Min4(v1.Y, v2.Y, v3.Y, v4.Y), Min4(vn1.Z, vn2.Z, vn3.Z, vn4.Z)),
            new Vector3(Max4(v1.X, v2.X, v3.X, v4.X), Max4(v1.Y, v2.Y, v3.Y, v4.Y), Max4(vn1.Z, vn2.Z, vn3.Z, vn4.Z))
        );
    }
    static float Max4(float f1, float f2, float f3, float f4) => Max(f1, Max(f2, Max(f3, f4)));
    static float Min4(float f1, float f2, float f3, float f4) => Min(f1, Min(f2, Min(f3, f4)));
}
public record struct QuadRef<T>(int modelIndex, int quadIndex);
