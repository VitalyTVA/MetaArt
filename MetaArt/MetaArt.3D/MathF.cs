namespace MetaArt.D3;
public static class MathF {
    public static readonly float PI = (float)Math.PI;
    public static float Constrain(float amt, float low, float high) => Math.Min(Math.Max(amt, low), high);
    public static float Max(float v1, float v2) => Math.Max(v1, v2);
    public static float Min(float v1, float v2) => Math.Min(v1, v2);

}
