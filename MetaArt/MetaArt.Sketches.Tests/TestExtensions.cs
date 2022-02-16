using MetaArt.D3;
using NUnit.Framework;
using System.Numerics;

namespace MetaArt.Sketches.Tests {
    public static class TestExtensions {
        public const float delta = 0.001f;
        public static Vector2 ProjectPoint(this Camera c, Vector3 point) {
            Vector3 p = c.TranslatePoint(point);

            var (x, y, _) = c.ToScreenCoords3(p);
            return new Vector2(x, y);
        }
        public static bool IsVisible(this Camera c, Vector3 vertex, Vector3 normal) {
            return Vector3.Dot(c.Location - vertex, normal) > 0;
        }
        public static Stream AsStream(this string s) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static void AssertVector(Vector2 expected, Vector2 actual) {
            if(FloatEqual(expected.X, actual.X) && FloatEqual(expected.Y, actual.Y))
                return;
            Assert.Fail($"Expected {PrintVector(expected)} but was {PrintVector(actual)}");
        }
        public static string PrintVector(Vector2 q) {
            return $"{q.X}f, {q.Y}f";
        }

        public static void AssertVector(Vector3 expected, Vector3 actual) {
            if(FloatEqual(expected.X, actual.X) && FloatEqual(expected.Y, actual.Y) && FloatEqual(expected.Z, actual.Z))
                return;
            Assert.Fail($"Expected {PrintVector(expected)} but was {PrintVector(actual)}");
        }
        public static string PrintVector(Vector3 q) {
            return $"{q.X}f, {q.Y}f, {q.Z}f";
        }

        public static void AssertQuaternion(Quaternion expected, Quaternion actual) {
            if(FloatEqual(expected.X, actual.X) && FloatEqual(expected.Y, actual.Y) && FloatEqual(expected.Z, actual.Z) && FloatEqual(expected.W, actual.W))
                return;
            Assert.Fail($"Expected {PrintQuaterion(expected)} but was {PrintQuaterion(actual)}");
        }

        public static string PrintQuaterion(Quaternion q) {
            return $"{q.X}f, {q.Y}f, {q.Z}f, {q.W}f";
        }

        public static bool FloatEqual(float x, float y) => Math.Abs(x - y) < delta;

        public static bool Intersects((Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4) q1, (Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4) q2) {
            return Extensions.GetQuadsIntersection(q1, q2) != null;
        }
    }
}
