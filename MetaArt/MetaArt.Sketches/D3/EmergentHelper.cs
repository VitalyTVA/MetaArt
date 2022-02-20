using MetaArt.D3;
using System.Numerics;

namespace D3;
public static class EmergentHelper {
    public static void Render(Scene<int[]> scene, Camera c) {
        noSmooth();
        noStroke();
        noFill();
        background(0);
        CameraExtensions.InitCoords(1);

        var pointSize = 1.5f;
        fill(Black);

        foreach(var (i1, i2, i3, points, vertices, _) in scene.GetTriangles(c)) {
            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];
            noStroke();
            CameraExtensions.triangle(v1, v2, v3);
            stroke(White);
            strokeWeight(pointSize);
            foreach(var p in points) {
                var (x, y) = vertices[p];
                point(x, y);
            }
        }
    }

    public static Model<int[]> AddRandomPoints(this Model<VoidType> model, float density) {
        var vertices = new List<Vector3>(model.Vertices);
        List<Tri<int[]>> planes = new();
        foreach(var (i1, i2, i3, _) in model.Tris) {
            var v1 = model.Vertices[i1];
            var v2 = model.Vertices[i2];
            var v3 = model.Vertices[i3];

            var count = vertices.Count;
            vertices.AddRange(GetTrianglePoints(v1, v2, v3, density));

            var pointIndices = Enumerable.Range(count, vertices.Count - count).ToArray();
            planes.Add((i1, i2, i3, pointIndices));
        }
        return new Model<int[]>(vertices.ToArray(), planes.ToArray(), id: model.Id) { Scale = model.Scale };
    }

    static IEnumerable<Vector3> GetTrianglePoints(Vector3 v1, Vector3 v2, Vector3 v3, float density) {
        var s = TriangleArea(v1, v2, v3);
        var count = density * s;
        const int N = 20;
        if(count < N) {
            count = randomBinomial(N, s * density / N);
        }
        var a = v2 - v1;
        var b = v3 - v1;
        for(int i = 0; i < count; i++) {
            var u1 = random(1);
            var u2 = random(1);
            if(u1 + u2 > 1) {
                u1 = 1 - u1;
                u2 = 1 - u2;
            }
            yield return v1 + a * u1 + b * u2;
        }
    }
    static int randomBinomial(int count, float p) {
        var result = 0;
        for(int i = 0; i < count; i++) {
            result += randomBernoulli(p) ? 1 : 0;
        }
        return result;
    }
    static bool randomBernoulli(float p) {
        return random(1) < p;
    }
    static float TriangleArea(Vector3 v1, Vector3 v2, Vector3 v3) {
        var a = (v1 - v2).Length();
        var b = (v1 - v3).Length();
        var c = (v3 - v2).Length();
        var p = (a + b + c) / 2;
        return sqrt(p * (p - a) * (p - b) * (p - c));
    }
}
