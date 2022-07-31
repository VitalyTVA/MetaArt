using System.Numerics;
using static MetaArt.Core.MathFEx;

namespace MetaArt.D3;

public class Scene<T> {
    List<(Model<T> model, Vector2[] vertices, Vector3[] normalVertices)> models = new();
    TriRef<T>[] tris;

    public IEnumerable<Model<T>> GetModels() => models.Select(x => x.model);

    public Model<T> this[string id] => models.First(x => x.model.Id == id).model; //TODO use dictionary

    public Scene(params Model<T>[] models) {
        this.models.AddRange(models.Select(x => (x, new Vector2[x.Vertices.Length], new Vector3[x.Vertices.Length])));
        tris = models.SelectMany((x, j) => Enumerable.Range(0, x.Tris.Length).Select(i => new TriRef<T>(j, i))).ToArray();
    }
    public IEnumerable<(int, int, int, T, Vector2[], Vector3[])> GetTriangles(Camera camera) {
        foreach(var (model, vertices, normalVertices) in models) {
            for(int i = 0; i < model.Vertices.Length; i++) {
                normalVertices[i] = camera.TranslatePoint(model.GetVertex(i));
                vertices[i] = camera.ToScreenCoords(normalVertices[i]);
            }
        }
        Array.Sort(tris, (x, y) => {
            return Comparer<float>.Default.Compare(
                GetBox(y).max.Z,
                GetBox(x).max.Z 
            );
        });

        HashSet<(int, int)> swaps = new();
        int steps = 0;
        for(int pi = 0; pi < tris.Length; pi++) {
            var (p1, p2, p3) = GetNormalVertices(tris[pi]);
            var pBox = GetBox(tris[pi]);
            //if(pBox.min.Z <= 0)
            //    break;
            bool writeP = pi == tris.Length - 1;
            int qi = pi + 1;
            for(qi = pi + 1; qi < tris.Length; qi++) {
                steps++;
                if(steps > 100000)
                    throw new InvalidOperationException("Cycle detected");
                var (q1, q2, q3) = GetNormalVertices(tris[qi]);
                var qBox = GetBox(tris[qi]);
                if(pBox.min.Z >= qBox.max.Z) {
                    writeP = true;
                    break;
                }
                if(RangesAreApart((pBox.min.X, pBox.max.X), (qBox.min.X, qBox.max.X))
                    || RangesAreApart((pBox.min.Y, pBox.max.Y), (qBox.min.Y, qBox.max.Y))
                    || Extensions.VerticesOnSameSideOfPlane((q1, q2, q3), (p1, p2, p3), cameraOnSameSide: false)
                    || Extensions.VerticesOnSameSideOfPlane((p1, p2, p3), (q1, q2, q3), cameraOnSameSide: true)
                ) {
                    continue;
                }
                {
                    var (p1_, p2_, p3_) = GetVertices(tris[pi]);
                    var (q1_, q2_, q3_) = GetVertices(tris[qi]);
                    var intersection = Extensions.GetTrisIntersection(
                        (p1_, p2_, p3_),
                        (q1_, q2_, q3_)
                    );
                    if(intersection == null)
                        continue;
                    if(intersection != null) {
                        var castPoint = (Vector3.Zero, new Vector3(intersection.Value.X, intersection.Value.Y, camera.FocalDistance));
                        var ip = Extensions.PlaneLineIntersection((p1, p2, p3), castPoint);
                        var iq = Extensions.PlaneLineIntersection((q1, q2, q3), castPoint);
                        if(Greater(ip.LengthSquared(), iq.LengthSquared()))
                            continue;
                    }
                }
                if(swaps.Contains((pi, qi))) {
                    //continue;
                    throw new InvalidOperationException("Cycle detected: " + (pi, qi));
                }
                var t = tris[pi];
                swaps.Add((pi, qi));
                tris[pi] = tris[qi];
                tris[qi] = t;
                pi--;
                break;
            }
            if(!writeP)
                writeP = qi == tris.Length;
            if(writeP) {
                var tri = tris[pi];
                var (model, vertices, normalVertices) = models[tri.modelIndex];
                var (i1, i2, i3, value) = model.Tris[tri.triIndex];
                var v1 = normalVertices[i1];
                var n = Extensions.GetNormal(v1, normalVertices[i2], normalVertices[i3]);
                if(Vector3.Dot(v1, n) >= 0)
                    continue;
                yield return (i1, i2, i3, value, vertices, normalVertices);
            }
        }

        //foreach(var (modelIndex, triIndex) in tris) {
        //    var (model, vertices, normalVertices) = models[modelIndex];
        //    var (i1, i2, i3, value) = model.Tris[triIndex];
        //    var v1 = normalVertices[i1];
        //    var n = Extensions.GetNormal(v1, normalVertices[i2], normalVertices[i3]);
        //    if(Vector3.Dot(v1, n) >= 0)
        //        continue;
        //    yield return (i1, i2, i3, value, vertices, normalVertices);
        //}
    }

    (Vector2, Vector2, Vector2) GetVertices(TriRef<T> x) {
        var (model, vertices, _) = models[x.modelIndex];
        var v1 = vertices[model.Tris[x.triIndex].i1];
        var v2 = vertices[model.Tris[x.triIndex].i2];
        var v3 = vertices[model.Tris[x.triIndex].i3];
        return (v1, v2, v3);
    }
    (Vector3, Vector3, Vector3) GetNormalVertices(TriRef<T> x) {
        var (model, _, normalVertices) = models[x.modelIndex];
        var v1 = normalVertices[model.Tris[x.triIndex].i1];
        var v2 = normalVertices[model.Tris[x.triIndex].i2];
        var v3 = normalVertices[model.Tris[x.triIndex].i3];
        return (v1, v2, v3);
    }
    (Vector3 min, Vector3 max) GetBox(TriRef<T> x) {
        var (v1, v2, v3) = GetVertices(x);
        var (vn1, vn2, vn3) = GetNormalVertices(x);
        return new(
            new Vector3(Min3(v1.X, v2.X, v3.X), Min3(v1.Y, v2.Y, v3.Y), Min3(vn1.Z, vn2.Z, vn3.Z)),
            new Vector3(Max3(v1.X, v2.X, v3.X), Max3(v1.Y, v2.Y, v3.Y), Max3(vn1.Z, vn2.Z, vn3.Z))
        );
    }
    static float Max3(float f1, float f2, float f3) => Max(f1, Max(f2, f3));
    static float Min3(float f1, float f2, float f3) => Min(f1, Min(f2, f3));
}
public record struct TriRef<T>(int modelIndex, int triIndex);
