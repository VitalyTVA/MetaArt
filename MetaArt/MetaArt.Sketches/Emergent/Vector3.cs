using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Emergent {
	public readonly struct Vector3 : IEquatable<Vector3> {
		public static readonly Vector3 Empty;
		public readonly float X, Y, Z;
		public readonly bool IsEmpty => this == Empty;
		public readonly float Length => Sketch.sqrt(LengthSquared);

		public readonly float LengthSquared => X * X + Y * Y + Z * Z;

		public Vector3(float x, float y, float z) {
			X = x;
			Y = y;
            Z = z;
		}

		public override readonly string ToString() {
			return $"{{X={X}, Y={Y}}}";
		}

        //public static Point Normalize(Point point) {
        //	float num = point.x * point.x + point.y * point.y;
        //	double num2 = 1.0 / Math.Sqrt(num);
        //	return new Point((float)((double)point.x * num2), (float)((double)point.y * num2));
        //}

        //public static float Distance(Point point, Point other) {
        //	float num = point.x - other.x;
        //	float num2 = point.y - other.y;
        //	float num3 = num * num + num2 * num2;
        //	return (float)Math.Sqrt(num3);
        //}

        //public static float DistanceSquared(Point point, Point other) {
        //	float num = point.x - other.x;
        //	float num2 = point.y - other.y;
        //	return num * num + num2 * num2;
        //}

        //public static Point Reflect(Point point, Point normal) {
        //	float num = point.x * point.x + point.y * point.y;
        //	return new Point(point.x - 2f * num * normal.x, point.y - 2f * num * normal.y);
        //}

        public static Vector3 operator -(Vector3 v1, Vector3 v2) {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2) {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }


        public readonly bool Equals(Vector3 obj) {
			return X == obj.X && Y == obj.Y && Z == obj.Z;
		}

		public override readonly bool Equals(object obj) {
			if(obj is Vector3 v) {
				return Equals(v);
			}
			return false;
		}

        public override int GetHashCode() {
            throw new NotImplementedException(); //TODO GetHashCode
        }

        public static bool operator ==(Vector3 left, Vector3 right) {
			return left.Equals(right);
		}

		public static bool operator !=(Vector3 left, Vector3 right) {
			return !left.Equals(right);
		}

		public void Deconstruct(out float x, out float y, out float z) {
			x = X;
			y = Y;
            z = Z;
		}
        public float Distance(Vector3 v) => (this - v).Length;

        public Vector3 Vector3To(Vector3 v) => v - this;
        public float ScalarProduct(Vector3 v) => X * v.X + Y * v.Y + Z * v.Z;
        public Vector3 Normalize() {
            var len = Length;
            return new Vector3(X / len, Y / len, Z / len);
        }
        public Vector3 Negate(Vector3 v) => new Vector3(-X, -Y, -Z);


        //public Vector3 Rotate(float angle) {
        //    var s = Sketch.sin(angle);
        //    var c = Sketch.cos(angle);
        //    return new Vector3(X * c - Y * s, X * s + Y * c);
        //}

        public Vector3 Mult(float scalar) => new Vector3(X * scalar, Y * scalar, Z * scalar);
    }
}
