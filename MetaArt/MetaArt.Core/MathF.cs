using System.Numerics;

namespace MetaArt.Core;
public static class MathFEx {
    public const float PI = (float)Math.PI;
    const float delta = 0.0002f;
    public static float Constrain(float amt, float low, float high) => Math.Min(Math.Max(amt, low), high);
    public static float Max(float v1, float v2) => Math.Max(v1, v2);
    public static float Min(float v1, float v2) => Math.Min(v1, v2);
    public static bool GreaterOrEqual(float x, float y) => x + delta > y;
    public static bool LessOrEqual(float x, float y) => x - delta < y;
    public static bool Greater(float x, float y) => x - delta > y;
    public static bool Less(float x, float y) => x + delta < y;

    public static bool RangesAreApart((float from, float to) r1, (float from, float to) r2) {
#if DEBUG
        if(r1.from > r1.to || r2.from > r2.to)
            throw new InvalidOperationException();
#endif
        if(Less(r2.from, r1.to) && LessOrEqual(r1.from, r2.from))
            return false;

        if(Less(r1.from, r2.to) && LessOrEqual(r2.from, r1.from))
            return false;

        return true;
    }

    static readonly Random rnd = new Random(0);
    public static float Random(float low, float high) => Lerp(low, high, (float)rnd.NextDouble());
    public static float Lerp(float start, float stop, float amt) => start * (1 - amt) + stop * amt;

    public static bool VectorsEqual(Vector2 v1, Vector2 v2) => LessOrEqual((v1 - v2).LengthSquared(), 0);
}
