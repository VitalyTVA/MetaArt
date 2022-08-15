using System.Numerics;

namespace MetaArt.Core;
public static class MathFEx {
    public const float PI = (float)Math.PI;
    const float delta = 0.0002f;
    public static float Constrain(float amt, float low, float high) => Math.Min(Math.Max(amt, low), high);
    public static float Max(float v1, float v2) => Math.Max(v1, v2);
    public static float Min(float v1, float v2) => Math.Min(v1, v2);
    public static float Sin(float angle) => (float)Math.Sin(angle);
    public static float Cos(float angle) => (float)Math.Cos(angle);
    public static bool GreaterOrEqual(float x, float y) => x + delta > y;
    public static bool LessOrEqual(float x, float y) => x - delta < y;
    public static bool Greater(float x, float y) => x - delta > y;
    public static bool Less(float x, float y) => x + delta < y;
    public static bool FloatsEqual(float x, float y) => Math.Abs(x - y) < delta;
    public static bool RectssEqual(Rect x, Rect y) => VectorsEqual(x.Location, y.Location) && VectorsEqual(x.Size, y.Size);

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

    public static Vector2 GetRestrictedLocation(this Rect rect, Rect containingRect) { 
        var location = rect.Location;
        location.X = Max(location.X, containingRect.Left);
        location.Y = Max(location.Y, containingRect.Top);
        location.X -= Max(0, rect.Right - containingRect.Right);
        location.Y -= Max(0, rect.Bottom - containingRect.Bottom);
        return location;
    }
    public static Rect GetRestrictedRect(this Rect rect, Rect containingRect) {
        return new Rect(rect.GetRestrictedLocation(containingRect), rect.Size);
    }
    public static Rect Inflate(this Rect rect, Vector2 size) {
        return new Rect(rect.Location - size, rect.Size + size * 2);
    }
    public static Vector2 SetX(this Vector2 vector, float x) {
        vector.X = x;
        return vector;
    }
    public static Vector2 SetY(this Vector2 vector, float y) {
        vector.Y = y;
        return vector;
    }
}
