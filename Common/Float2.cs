namespace StrategicGame.Common {
    public struct Float2 {
        public readonly float X, Y;

        public Float2(Int2 value) : this(value.X, value.Y) { }
        public Float2(float value) : this(value, value) { }
        public Float2(float x, float y) { X = x; Y = y; }

        public static Float2 operator*(Float2 v, float s) {
            return new Float2(v.X * s, v.Y * s);
        }

        public static Float2 operator+(Float2 a, Float2 b) {
            return new Float2(a.X + b.X, a.Y + b.Y);
        }

        public static Float2 Lerp(Float2 from, Float2 to, float t) {
            return from * (1 - t) + to * t;
        }

        public override string ToString() {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}
