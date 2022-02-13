using System.Numerics;

namespace MetaArt.D3;
public static class MathFEx {
    public static readonly float PI = (float)Math.PI;
    const float delta = 0.0002f;
    public static float Constrain(float amt, float low, float high) => Math.Min(Math.Max(amt, low), high);
    public static float Max(float v1, float v2) => Math.Max(v1, v2);
    public static float Min(float v1, float v2) => Math.Min(v1, v2);
    public static bool Greater(float x, float y) => x + delta > y;
    public static bool Less(float x, float y) => x - delta < y;
    public static bool GreaterOrEqual(float x, float y) => x - delta > y;
    public static bool LessOrEqual(float x, float y) => x + delta < y;

    public static bool RangesAreApart((float from, float to) r1, (float from, float to) r2) {
#if DEBUG
        if(r1.from > r1.to || r2.from > r2.to)
            throw new InvalidOperationException();
#endif
        if(LessOrEqual(r2.from, r1.to) && Less(r1.from, r2.from))
            return false;

        if(LessOrEqual(r1.from, r2.to) && Less(r2.from, r1.from))
            return false;

        return true;
        
    }
}
