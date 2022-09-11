using MetaArt.D3;
using System.Numerics;

namespace D3;

class Lights {
    public YawPitchContoller lightsController = new();
    float ambientLight = 0.5f;
    float directionalLight = 0.5f;
    public void ChangeAmbient(float delta) {
        ambientLight = constrain(ambientLight + delta, 0, 1);
    }
    public Func<Vector3, Vector3, Vector3, float> GetLuminocityCalulator(Camera c) {
        var lightDirection = Vector3.Transform(lightsController.GetDirection(), c.Rotation);

        return (v1, v2, v3) => {
            var norm = Vector3.Normalize(Extensions.GetNormal(v3, v2, v1));
            return ambientLight + max(0, Vector3.Dot(lightDirection, norm)) * directionalLight;
        };
    }
}