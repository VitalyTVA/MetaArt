using System.Numerics;
using static MetaArt.D3.MathFEx;

namespace MetaArt.D3;
public class YawPitchContoller {
    float yaw;
    float pitch;

    public void Yaw(float yawDelta) {
        yaw += yawDelta;
    }
    public void Pitch(float pitchDelta) {
        pitch += pitchDelta;
        pitch = Constrain(pitch, -PI / 4, PI / 4);
    }
    public Quaternion CreateRotation() {
        return Quaternion.CreateFromYawPitchRoll(0, pitch, 0) * Quaternion.CreateFromYawPitchRoll(yaw, 0, 0);
    }
}
