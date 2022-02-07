namespace MetaArt.D3;
public static class MathF {
    public static readonly float PI = (float)Math.PI;
    public static float Constrain(float amt, float low, float high) => Math.Min(Math.Max(amt, low), high);

}
