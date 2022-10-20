using MetaArt.Core;
using System.Numerics;

namespace MetaConstruct {
    public static class ConstructHelper {
        public static (Vector2, Vector2)? GetCircleIntersections(Vector2 center1, Vector2 center2, float radius1, float radius2) {
            var (r1, r2) = (radius1, radius2);
            (float x1, float y1, float x2, float y2) = (center1.X, center1.Y, center2.X, center2.Y);
            // d = distance from center1 to center2
            float d = MathF.Sqrt(MathF.Pow(x1 - x2, 2) + MathF.Pow(y1 - y2, 2));
            // Return an empty array if there are no intersections
            if(!(MathF.LessOrEqual(MathF.Abs(r1 - r2), d) && MathF.LessOrEqual(d, r1 + r2))) 
                return null;

            // Intersections i1 and possibly i2 exist
            var dsq = d * d;
            var (r1sq, r2sq) = (r1 * r1, r2 * r2);
            var r1sq_r2sq = r1sq - r2sq;
            var a = r1sq_r2sq / (2 * dsq);
            var t = 2 * (r1sq + r2sq) / dsq - r1sq_r2sq * r1sq_r2sq / (dsq * dsq) - 1;
            if(MathF.FloatsEqual(t, 0)) t = 0;
            var c = MathF.Sqrt(t);

            var fx = (x1 + x2) / 2 + a * (x2 - x1);
            var gx = c * (y2 - y1) / 2;

            var fy = (y1 + y2) / 2 + a * (y2 - y1);
            var gy = c * (x1 - x2) / 2;

            var i1 = new Vector2((float)(fx + gx), (float)(fy + gy));
            var i2 = new Vector2((float)(fx - gx), (float)(fy - gy));

            return (i1, i2);
        }
    }
}
