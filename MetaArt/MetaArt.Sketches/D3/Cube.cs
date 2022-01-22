using System.Numerics;
using MetaArt.D3;
using Vector = MetaArt.Vector;

namespace D3;
class Cube {
    Vector3[] vertices = null!;
    void setup() {
        size(600, 400);

        var side = 100;

        vertices = new[] {
            new Vector3(side, side, side),
            new Vector3(side, -side, side),
            new Vector3(-side, -side, side),
            new Vector3(-side, side, side),

            new Vector3(side, side, -side),
            new Vector3(side, -side, -side),
            new Vector3(-side, -side, -side),
            new Vector3(-side, side, -side),
        };
    }

    float yaw, pitch;

    void draw() {
        noSmooth();
        stroke(White);
        strokeWeight(1);
        noFill();
        background(0);
        translate(width / 2, height / 2);
        scale(1, -1);

        //var q = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);
        var q = Quaternion.CreateFromYawPitchRoll(0, pitch, 0) * Quaternion.CreateFromYawPitchRoll(yaw, 0, 0);

        var v = Vector3.Transform(new Vector3(0, 0, -300), Quaternion.Conjugate(q));

        //circle(0, 0, 50);
        var c = new Camera(
            //new Vector3(0, 300, -300),
            //Quaternion.CreateFromYawPitchRoll(0.0f, -PI / 4, 0.0f),

            //new Vector3(300, 0, -300),
            //Quaternion.CreateFromYawPitchRoll(PI / 4, 0.0f, 0.0f),

            //new Vector3(300, 0, -300),
            //Quaternion.CreateFromAxisAngle(new Vector3(0, 1f, 0), PI / 4), 

            //new Vector3(0, 300, -300),
            //Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -PI / 4), 

            //new Vector3(0, 0, -300),
            //Quaternion.Identity,

            v,
            q,

            200
        );

        //vertices.RotateY(deltaTime / 1000f);

        var lines = new[] {
            (0, 1), (1, 2), (2, 3), (3, 0),
            (4, 5), (5, 6), (6, 7), (7, 4),
            (0, 4), (1, 5), (2, 6), (3, 7),
        };
        //foreach(var (from, to) in lines) {
        //    c.line3(vertices[from], vertices[to]);
        //}

        var quads = new[] {
            (0, 1, 2, 3, Colors.Red),
            (7, 6, 5, 4, Colors.Blue),
            (4, 5, 1, 0, Colors.Pink),
            (3, 2, 6, 7, Colors.Green),
            (3, 7, 4, 0, Colors.Yellow),
            (1, 5, 6, 2, Colors.Orange),
        };

        var pointPlains = quads.Select(x => {
            var v1 = vertices[x.Item1];
            var v2 = vertices[x.Item2];
            var v3 = vertices[x.Item3];
            var v4 = vertices[x.Item4];

            var (x1, y1, z1) = v1; 
            var (x2, y2, z2) = v3;
            var points = Enumerable
                .Range(0, 30)
                .Select(_ => new Vector3(random(x1, x2), random(y1, y2), random(z1, z2)))
                .ToArray();
            return (points, (v1, v2, v3, v4));
        }).ToArray();

        var pointSize = 2;

        randomSeed(0);
        var backgroundPoints = Enumerable.Range(0, 400).Select(_ => (random(-width / 2, width / 2), random(-height / 2, height / 2)));
        stroke(White);
        strokeWeight(pointSize);
        foreach(var (x, y) in backgroundPoints) {
            point(x, y);
        }

        fill(Black);
        foreach(var (points, (v1, v2, v3, v4)) in pointPlains) {
            var n = CameraExtensions.GetNormal(v1, v2, v3);
            if(!c.IsVisible(v1, n)) continue;
            noStroke();
            c.quad3(v1, v2, v3, v4);
            stroke(White);
            strokeWeight(pointSize);
            foreach(var p in points) {
                var (x, y) = c.ProjectPoint(p);
                point(x, y);
            }
        }

        //foreach(var (v1, v2, v3, v4, color) in quads) {
        //    fill(color);
        //    c.quad3(vertices[v1], vertices[v2], vertices[v3], vertices[v4]);
        //}
    }

    void keyPressed() {
        float step = PI / 60;

        if(key == 'a') yaw -= step;
        if(key == 'd') yaw += step;
        if(key == 'w' && pitch > - PI / 4) pitch -= step;
        if(key == 's' && pitch < PI / 4) pitch += step;
    }
}
static class CameraExtensions {
    public static void RotateY(this Vector3[] vertices, float angle) {
        for(int i = 0; i < vertices.Length; i++) {
            var (x, y, z) = vertices[i];
            (x, z) = new Vector(x, z).Rotate(angle);
            vertices[i] = new Vector3(x, y, z);
        }
    }
    public static void line3(this Camera c, Vector3 p1, Vector3 p2) { 
        var (x1, y1) = c.ProjectPoint(p1);
        var (x2, y2) = c.ProjectPoint(p2);
        line(x1, y1, x2, y2);
    }
    public static void quad3(this Camera c, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
        var n = GetNormal(p1, p2, p3);
        if(!c.IsVisible(p1, n)) return;

        var (x1, y1) = c.ProjectPoint(p1);
        var (x2, y2) = c.ProjectPoint(p2);
        var (x3, y3) = c.ProjectPoint(p3);
        var (x4, y4) = c.ProjectPoint(p4);
        quad(x1, y1, x2, y2, x3, y3, x4, y4);
    }

    public static Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3) {
        return Vector3.Cross(p2 - p1, p3 - p2);
    }

    public static bool IsVisible(this Camera c, Vector3 vertex, Vector3 normal) {
        return Vector3.Dot(vertex - c.Location, normal) > 0;
    }

    public static void Deconstruct(this Vector3 v, out float x, out float y, out float z) {
        x = v.X;
        y = v.Y;
        z = v.Z;
    }
    public static void Deconstruct(this Vector2 v, out float x, out float y) {
        x = v.X;
        y = v.Y;
    }
}
