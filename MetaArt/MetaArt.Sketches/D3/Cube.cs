using System.Numerics;
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
        foreach(var (from, to) in lines) {
            c.line3(vertices[from], vertices[to]);
        }
    }

    void keyPressed() {
        float step = PI / 30;

        if(key == 'a') yaw -= step;
        if(key == 'd') yaw += step;
        if(key == 'w' && pitch > - PI / 4) pitch -= step;
        if(key == 's' && pitch < PI / 4) pitch += step;
    }
}

class Camera {
    public readonly Vector3 Location;
    readonly float FocalDistance;
    readonly Quaternion Rotation;

    public Camera(Vector3 location, Quaternion rotation, float focalDistance) {
        Location = location;

        this.Rotation = Quaternion.Normalize(rotation);
        FocalDistance = focalDistance;
    }

    public Vector ProjectPoint(Vector3 point) {
        var p = point - Location;


        p = Vector3.Transform(p, Rotation);

        var r = new Vector(
            p.X * FocalDistance / p.Z,
            p.Y * FocalDistance / p.Z
        );
        return r;//.Negate();

        //var t = Vector3.Dot(ScreenCenter - point, Screen) / Vector3.Dot(Location - point, Screen);
        //var v = Location * t  + point * (1 - t);
        //return new Vector(v.X, v.Y);
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
    public static void Deconstruct(this Vector3 v, out float x, out float y, out float z) {
        x = v.X;
        y = v.Y;
        z = v.Z;
    }
}
