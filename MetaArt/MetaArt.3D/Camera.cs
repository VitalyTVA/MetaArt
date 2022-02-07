using System.Numerics;

namespace MetaArt.D3;

public class Camera {
    public readonly Vector3 Location;
    readonly float FocalDistance;
    public readonly Quaternion Rotation;

    public Camera(Vector3 location, Quaternion rotation, float focalDistance) {
        Location = location;

        this.Rotation = Quaternion.Normalize(rotation);
        FocalDistance = focalDistance;
    }

    public Vector2 ToScreenCoords(Vector3 p) {
        var ratio = FocalDistance / p.Z;
        var r = new Vector2(
            p.X * ratio,
            p.Y * ratio
        );
        return r;
    }

    public Vector3 TranslatePoint(Vector3 point) {
        var p = point - Location;
        p = Vector3.Transform(p, Rotation);
        return p;
    }
}