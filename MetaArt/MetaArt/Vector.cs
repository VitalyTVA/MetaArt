using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace MetaArt {
	public readonly struct Vector : IEquatable<Vector> {
		public static readonly Vector Empty;

		public readonly float X;

		public readonly float Y;

		public readonly bool IsEmpty => this == Empty;

		public readonly float Length => (float)Math.Sqrt(X * X + Y * Y);

		public readonly float LengthSquared => X * X + Y * Y;

		public Vector(float x, float y) {
			this.X = x;
			this.Y = y;
		}

		//public void Offset(Vector p) {
		//	x += p.x;
		//	y += p.y;
		//}

		//public void Offset(float dx, float dy) {
		//	x += dx;
		//	y += dy;
		//}

		public override readonly string ToString() {
			return $"{{X={X}, Y={Y}}}";
		}

        //TODO point operations

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

        //public static Point Add(Point pt, SKSizeI sz) {
        //	return pt + sz;
        //}

        //public static Point Add(Point pt, SKSize sz) {
        //	return pt + sz;
        //}

        //public static Point Add(Point pt, SKPointI sz) {
        //	return pt + sz;
        //}

        //public static Point Add(Point pt, Point sz) {
        //	return pt + sz;
        //}

        //public static Point Subtract(Point pt, SKSizeI sz) {
        //	return pt - sz;
        //}

        //public static Point Subtract(Point pt, SKSize sz) {
        //	return pt - sz;
        //}

        //public static Point Subtract(Point pt, SKPointI sz) {
        //	return pt - sz;
        //}

        //public static Point Subtract(Point pt, Point sz) {
        //	return pt - sz;
        //}

        //public static Point operator +(Point pt, SKSizeI sz) {
        //	return new SKPoint(pt.x + (float)sz.Width, pt.y + (float)sz.Height);
        //}

        //public static Point operator +(Point pt, SKSize sz) {
        //	return new SKPoint(pt.x + sz.Width, pt.y + sz.Height);
        //}

        //public static Point operator +(Point pt, SKPointI sz) {
        //	return new SKPoint(pt.x + (float)sz.X, pt.y + (float)sz.Y);
        //}

        //public static Point operator +(Point pt, Point sz) {
        //	return new Point(pt.x + sz.X, pt.y + sz.Y);
        //}

        //public static Point operator -(Point pt, SKSizeI sz) {
        //	return new SKPoint(pt.X - (float)sz.Width, pt.Y - (float)sz.Height);
        //}

        public static Vector operator -(Vector v1, Vector v2) {
            return new Vector(v1.X - v2.X, v2.Y - v2.Y);
        }

        public static Vector operator +(Vector v1, Vector v2) {
            return new Vector(v1.X + v2.X, v2.Y + v2.Y);
        }

        //public static Point operator -(Point pt, SKPointI sz) {
        //	return new SKPoint(pt.X - (float)sz.X, pt.Y - (float)sz.Y);
        //}

        //public static Point operator -(Point pt, Point sz) {
        //	return new Point(pt.X - sz.X, pt.Y - sz.Y);
        //}

        public readonly bool Equals(Vector obj) {
			if(X == obj.X) {
				return Y == obj.Y;
			}
			return false;
		}

		public override readonly bool Equals(object obj) {
			if(obj is Vector obj2) {
				return Equals(obj2);
			}
			return false;
		}

        public override int GetHashCode() {
            throw new NotImplementedException(); //TODO GetHashCode
        }

        public static bool operator ==(Vector left, Vector right) {
			return left.Equals(right);
		}

		public static bool operator !=(Vector left, Vector right) {
			return !left.Equals(right);
		}

		public void Deconstruct(out float x, out float y) {
			x = this.X;
			y = this.Y;
		}
        public float Distance(Vector v) => (this - v).Length;

        public Vector VectorTo(Vector v) => v - this;
        public float ScalarProduct(Vector v) => X * v.X + Y * v.Y;
        public Vector Normalize() {
            var len = Length;
            return new Vector(X / len, this.Y / len);
        }
        public Vector Negate(Vector v) => new Vector(-X, -Y);


        public Vector Rotate(float angle) {
            var s = Sketch.sin(angle);
            var c = Sketch.cos(angle);
            return new Vector(X * c - Y * s, X * s + Y * c);
        }

        public Vector Mult(float scalar) => new Vector(X * scalar, Y * scalar);

        /*

    subtract(v: Vector): Vector {
        return new Vector(this.x - v.x, this.y - v.y);
    }
    add(v: Vector): Vector {
        return new Vector(this.x + v.x, this.y  + v.y);
    }
    setLength(length: number): Vector {
        return this.normalize().mult(length);
    }
        */
    }
}
