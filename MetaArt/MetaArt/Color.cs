using System;
using System.Linq;
namespace MetaArt {
    public readonly struct Color : IEquatable<Color> {
        public static readonly Color Empty;

        public readonly uint Value;

        public byte Alpha => (byte)((Value >> 24) & 0xFFu);

        public byte Red => (byte)((Value >> 16) & 0xFFu);

        public byte Green => (byte)((Value >> 8) & 0xFFu);

        public byte Blue => (byte)(Value & 0xFFu);

        public Color(uint value) {
            Value = value;
        }

        public Color(byte red, byte green, byte blue, byte alpha) {
            Value = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
        }

        public Color(byte red, byte green, byte blue) {
            Value = 0xFF000000u | (uint)(red << 16) | (uint)(green << 8) | blue;
        }

        public Color WithRed(byte red) {
            return new Color(red, Green, Blue, Alpha);
        }

        public Color WithGreen(byte green) {
            return new Color(Red, green, Blue, Alpha);
        }

        public Color WithBlue(byte blue) {
            return new Color(Red, Green, blue, Alpha);
        }

        public Color WithAlpha(byte alpha) {
            return new Color(Red, Green, Blue, alpha);
        }

        public override string ToString() {
            return $"#{Alpha:x2}{Red:x2}{Green:x2}{Blue:x2}";
        }

        public bool Equals(Color obj) {
            return obj.Value == Value;
        }

        public override bool Equals(object other) {
            if(other is Color obj) {
                return Equals(obj);
            }
            return false;
        }

        public static bool operator ==(Color left, Color right) {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right) {
            return !left.Equals(right);
        }

        public override int GetHashCode() {
            uint num = Value;
            return num.GetHashCode();
        }
    }
}
