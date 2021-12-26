using System;
using System.Linq;

namespace MetaArt {
    public struct Size : IEquatable<Size> {
        public static readonly Size Empty;

        private float w;

        private float h;

        public readonly bool IsEmpty => this == Empty;

        public float Width {
            readonly get {
                return w;
            }
            set {
                w = value;
            }
        }

        public float Height {
            readonly get {
                return h;
            }
            set {
                h = value;
            }
        }

        public Size(float width, float height) {
            w = width;
            h = height;
        }
        //TODO size operation
        //public Size(SKPoint pt) {
        //	w = pt.X;
        //	h = pt.Y;
        //}

        //public readonly SKPoint ToPoint() {
        //	return new SKPoint(w, h);
        //}

        //public readonly SKSizeI ToSizeI() {
        //	checked {
        //		int width = (int)w;
        //		int height = (int)h;
        //		return new SKSizeI(width, height);
        //	}
        //}

        public override readonly string ToString() {
            return $"{{Width={w}, Height={h}}}";
        }

        //public static Size Add(Size sz1, Size sz2) {
        //	return sz1 + sz2;
        //}

        //public static Size Subtract(Size sz1, Size sz2) {
        //	return sz1 - sz2;
        //}

        //public static Size operator +(Size sz1, Size sz2) {
        //	return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        //}

        //public static Size operator -(Size sz1, Size sz2) {
        //	return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        //}

        //public static explicit operator SKPoint(Size size) {
        //	return new SKPoint(size.Width, size.Height);
        //}

        //public static implicit operator Size(SKSizeI size) {
        //	return new SKSize(size.Width, size.Height);
        //}

        public readonly bool Equals(Size obj) {
            if(w == obj.w) {
                return h == obj.h;
            }
            return false;
        }

        public override readonly bool Equals(object obj) {
            if(obj is Size obj2) {
                return Equals(obj2);
            }
            return false;
        }

        public static bool operator ==(Size left, Size right) {
            return left.Equals(right);
        }

        public static bool operator !=(Size left, Size right) {
            return !left.Equals(right);
        }

        public override readonly int GetHashCode() {
            throw new NotImplementedException(); //TODO GetHashCode
        }

        public void Deconstruct(out float w, out float h) {
            w = this.w;
            h = this.h;
        }
    }
}
