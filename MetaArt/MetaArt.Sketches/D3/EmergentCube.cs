using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class EmergentCube {
    Scene<int[]> scene = null!;
    void setup() {
        size(600, 400);

        var density = 0.003f;
        var models = new[] {
            Loader.LoadModels("icosphere", new LoadOptions<VoidType>(scale: 500, invert: true)).Select(x => AddRandomPoints(x, density / 5)),
            
            //Loader.LoadModels("cubes", new LoadOptions<VoidType>(scale: 50)).Select(x => AddRandomPoints(x, density)),
            //Loader.LoadModels("heart", new LoadOptions<VoidType>(scale: 100)).Select(x => AddRandomPoints(x, density)),
            Loader.LoadModels("primitives", new LoadOptions<VoidType>(scale: 50)).Select(x => AddRandomPoints(x, density)),
        }.SelectMany(x => x).ToArray();
        scene = new Scene<int[]>(models);
    }

    static Model<int[]> AddRandomPoints(Model<VoidType> model, float density) {
        var vertices = new List<Vector3>(model.Vertices);
        List<Quad<int[]>> planes = new();
        foreach(var (i1, i2, i3, _) in model.Quads) {
            var v1 = model.Vertices[i1];
            var v2 = model.Vertices[i2];
            var v3 = model.Vertices[i3];

            var count = vertices.Count;
            vertices.AddRange(GetTrianglePoints(v1, v2, v3, density));

            var pointIndices = Enumerable.Range(count, vertices.Count - count).ToArray();
            planes.Add((i1, i2, i3, pointIndices));
        }
        return new Model<int[]>(vertices.ToArray(), planes.ToArray()) { Scale = model.Scale };
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
            yield return v1 + a * u1  + b * u2;
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

    YawPitchContoller controller = new();
    void draw() {
        noSmooth();
        noStroke();
        noFill();
        background(0);
        CameraExtensions.InitCoords(1);

        var c = controller.CreateCamera(600, 400);

        var dx = mouseX - pmouseX;
        var dy = mouseY - pmouseY;
        if(isLeftMousePressed) {
            const float scale = 200;
            controller.Pitch(-dy / scale);
            controller.Yaw(-dx / scale);
        }
        if(isRightMousePressed) {
            foreach(var model in scene.GetModels().Skip(1)) {
                model.Rotate(c, dx, -dy);

            }
        }

        var pointSize = 1.5f;
        fill(Black);

        foreach(var (i1, i2, i3, i4, points, vertices, _) in scene.GetQuads(c)) {
            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];
            var v4 = vertices[i4];
            noStroke();
            CameraExtensions.quad3(v1, v2, v3, v4);
            stroke(White);
            strokeWeight(pointSize);
            foreach(var p in points) {
                var (x, y) = vertices[p];
                point(x, y);
            }
        }
    }

    void keyPressed() {
        CameraExtensions.MoveOnShepeOnKeyPressed(controller);
    }
}
