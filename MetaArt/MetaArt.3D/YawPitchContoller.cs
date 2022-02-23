using System.Numerics;
using static MetaArt.D3.MathFEx;

namespace MetaArt.D3;
public class YawPitchContoller {
    float yaw;
    float pitch;
    readonly float pitchMax;

    public YawPitchContoller(float yaw = 0, float pitch = 0, float pitchMax = PI / 4) {
        this.yaw = yaw;
        this.pitch = pitch;
        this.pitchMax = pitchMax;
    }

    public void SetYaw(float yaw) => this.yaw = yaw;

    public void ChangeYaw(float yawDelta) {
        SetYaw(yaw + yawDelta);
    }
    public void ChangePitch(float pitchDelta) {
        pitch += pitchDelta;
        pitch = Constrain(pitch, -pitchMax, pitchMax);
    }
    public Quaternion CreateRotation() {
        return Quaternion.CreateFromYawPitchRoll(0, pitch, 0) * Quaternion.CreateFromYawPitchRoll(yaw, 0, 0);
    }
}
