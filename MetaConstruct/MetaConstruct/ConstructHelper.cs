using MetaArt.Core;
using System.Numerics;

namespace MetaConstruct {
    public static class ConstructHelper {
        public static (Vector2, Vector2)? GetLineCircleIntersections(Vector2 center, float radius, Vector2 point1, Vector2 point2) {
            float cx = center.X;
            float cy = center.Y;

            var d = point2 - point1;

            float A = d.LengthSquared();
            float B = 2 * (d.X * (point1.X - cx) + d.Y * (point1.Y - cy));
            float C = (point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - radius * radius;

            float det = B * B - 4 * A * C;
            if(MathF.FloatsEqual(A, 0) || MathF.Less(det, 0)) {
                return null;
            } else {
                var t1 = (float)((-B + Math.Sqrt(det)) / (2 * A));
                var intersection1 = new Vector2(point1.X + t1 * d.X, point1.Y + t1 * d.Y);

                var t2 = (float)((-B - Math.Sqrt(det)) / (2 * A));
                var intersection2 = new Vector2(point1.X + t2 * d.X, point1.Y + t2 * d.Y);

                return (intersection1, intersection2);
            }
        }


        public static Vector2? GetLinesIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2) {
            // determinant
            float d = (a1.X - a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X - b2.X);

            // check if lines are parallel
            if(MathF.FloatsEqual(d, 0)) return null;

            float px = (a1.X * a2.Y - a1.Y * a2.X) * (b1.X - b2.X) - (a1.X - a2.X) * (b1.X * b2.Y - b1.Y * b2.X);
            float py = (a1.X * a2.Y - a1.Y * a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X * b2.Y - b1.Y * b2.X);
            return new Vector2(px, py) / d;
        }
        public static (Vector2, Vector2)? GetCirclesIntersections(Vector2 center1, Vector2 center2, float radius1, float radius2) {
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
