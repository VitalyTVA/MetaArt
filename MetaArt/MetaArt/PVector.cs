using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace MetaArt.ProcessingCompatibility {
	public struct PVector : IEquatable<PVector> {
		public static readonly PVector Empty;

		public float x;

		public float y;

		public readonly bool IsEmpty => this == Empty;

		public readonly float Length => (float)Math.Sqrt(x * x + y * y);

		public readonly float LengthSquared => x * x + y * y;

		public PVector(float x, float y, float z = 0) {
			this.x = x;
			this.y = y;
			if(z != 0) //TODO z-coordinate
				throw new InvalidOperationException();
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

        public static PVector random2D() {
            var angle = Sketch.random(Sketch.TWO_PI);
			return fromAngle(angle);
        }
		public static PVector fromAngle(float angle) {
			return new PVector(Sketch.cos(angle), Sketch.sin(angle));
		}

		public PVector mult(float v) {
			x *= v;
			y *= v;
			return this;
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

		[Obsolete($"Use the {nameof(heading)}() method instead.")]
		public float heading2D() => heading();


		public static bool operator !=(PVector left, PVector right) {
			return !left.Equals(right);
		}

        public float mag() {
			return Sketch.sqrt(x * x + y * y);
        }

        public void Deconstruct(out float x, out float y) {
			x = this.x;
			y = this.y;
		}

        public static PVector sub(PVector b, PVector a) {
            return new PVector(b.x - a.x, b.y - a.y);
        }

        public void div(float v) {
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

        public void lerp(PVector v, float amt) {
			x = Sketch.lerp(x, v.x, amt);
			y = Sketch.lerp(y, v.y, amt);
		}

        public static float dist(PVector position1, PVector position2) {
			var x = position2.x - position1.x;
			var y = position2.y - position1.y;
			return Sketch.sqrt(x * x + y * y);
        }

        public PVector normalize() {
			var len = mag();
			//if(len <= 0.0000001) {
			//	return;
			//}
			div(len);
			return this;
        }

        public void sub(PVector other) {
            x -= other.x;
            y -= other.y;
        }

        public PVector copy() {
            return this; //TODO should vector be a class?
        }

        public void setMag(float mag) {
			var r = mag / this.mag();
			x *= r;
			y *= r;
		}

		public void limit(float max) {
			var len = mag();
			//if(len <= 0.0000001) {
			//	return;
			//}
			if(len > max) {
				var r = max / len;
				x *= r;
				y *= r;
			}
		}

        public static PVector div(PVector v, float mass) {
            return new PVector(v.x / mass, v.y / mass);
        }
    }
}
