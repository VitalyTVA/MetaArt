using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace MetaArt {
	public struct Point : IEquatable<Point> {
		public static readonly Point Empty;

		private float x;

		private float y;

		public readonly bool IsEmpty => this == Empty;

		public readonly float Length => (float)Math.Sqrt(x * x + y * y);

		public readonly float LengthSquared => x * x + y * y;

		public float X {
			readonly get {
				return x;
			}
			set {
				x = value;
			}
		}

		public float Y {
			readonly get {
				return y;
			}
			set {
				y = value;
			}
		}

		public Point(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public void Offset(Point p) {
			x += p.x;
			y += p.y;
		}

		public void Offset(float dx, float dy) {
			x += dx;
			y += dy;
		}

		public override readonly string ToString() {
			return $"{{X={x}, Y={y}}}";
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

		//public static Point operator -(Point pt, SKSize sz) {
		//	return new SKPoint(pt.X - sz.Width, pt.Y - sz.Height);
		//}

		//public static Point operator -(Point pt, SKPointI sz) {
		//	return new SKPoint(pt.X - (float)sz.X, pt.Y - (float)sz.Y);
		//}

		//public static Point operator -(Point pt, Point sz) {
		//	return new Point(pt.X - sz.X, pt.Y - sz.Y);
		//}

		public readonly bool Equals(Point obj) {
			if(x == obj.x) {
				return y == obj.y;
			}
			return false;
		}

		public override readonly bool Equals(object obj) {
			if(obj is Point obj2) {
				return Equals(obj2);
			}
			return false;
		}

        public override int GetHashCode() {
            throw new NotImplementedException(); //TODO GetHashCode
        }

        public static bool operator ==(Point left, Point right) {
			return left.Equals(right);
		}

		public static bool operator !=(Point left, Point right) {
			return !left.Equals(right);
		}

		public void Deconstruct(out float x, out float y) {
			x = this.x;
			y = this.y;
		}
	}
}
