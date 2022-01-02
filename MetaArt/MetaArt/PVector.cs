using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace MetaArt {
	public struct PVector : IEquatable<PVector> {
		public static readonly PVector Empty;

		public float x;

		public float y;

		public readonly bool IsEmpty => this == Empty;

		public readonly float Length => (float)Math.Sqrt(x * x + y * y);

		public readonly float LengthSquared => x * x + y * y;

		public PVector(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public override readonly string ToString() {
			return $"{{X={x}, Y={y}}}";
		}


		public readonly bool Equals(PVector obj) {
			if(x == obj.x) {
				return y == obj.y;
			}
			return false;
		}

        public void mult(float v) {
			x *= v;
			y *= v;
		}

        public override readonly bool Equals(object obj) {
			if(obj is PVector obj2) {
				return Equals(obj2);
			}
			return false;
		}

        public override int GetHashCode() {
            throw new NotImplementedException(); //TODO GetHashCode
        }

        public static bool operator ==(PVector left, PVector right) {
			return left.Equals(right);
		}

        public float heading() {
            var angle = Math.Atan2(y, x);
			return (float)angle;
        }

        public static bool operator !=(PVector left, PVector right) {
			return !left.Equals(right);
		}

        public float mag() {
			return Sketch.sqrt(x * x + y + y);
        }

        public void Deconstruct(out float x, out float y) {
			x = this.x;
			y = this.y;
		}

        public static PVector sub(PVector b, PVector a) {
            return new PVector(b.x - a.x, b.y - a.y);
        }

        public void div(int v) {
            x /= v;
			y /= v;
        }

        public void add(PVector v) {
			x += v.x;
			y += v.y;
        }

        public void rotate(float rad) {
			float sin = (float)Math.Sin(rad);
			float cos = (float)Math.Cos(rad);

			float tx = x;
			float ty = y;
			x = (cos * tx) - (sin * ty);
			y = (sin * tx) + (cos * ty);
		}
    }
}
