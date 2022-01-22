using System.Numerics;

namespace MetaArt.D3;

public class Camera {
    public readonly Vector3 Location;
    readonly float FocalDistance;
    readonly Quaternion Rotation;

    public Camera(Vector3 location, Quaternion rotation, float focalDistance) {
        Location = location;

        this.Rotation = Quaternion.Normalize(rotation);
        FocalDistance = focalDistance;
    }

    public Vector2 ProjectPoint(Vector3 point) {
        var p = point - Location;


        p = Vector3.Transform(p, Rotation);

        var r = new Vector2(
            p.X * FocalDistance / p.Z,
            p.Y * FocalDistance / p.Z
        );
        return r;//.Negate();

        //var t = Vector3.Dot(ScreenCenter - point, Screen) / Vector3.Dot(Location - point, Screen);
        //var v = Location * t  + point * (1 - t);
        //return new Vector(v.X, v.Y);
    }
}
