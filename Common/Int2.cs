namespace StrategicGame.Common {
    public struct Int2 {
        public readonly int X, Y;

        public Int2(int value) : this(value, value) { }
        public Int2(int x, int y) { X = x; Y = y; }

        public static bool operator==(Int2 a, Int2 b) {
            return a.X == b.X && a.Y == b.Y;
        }
        
        public static bool operator!=(Int2 a, Int2 b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            return obj is Int2 && this == (Int2)obj;
        }

        public override int GetHashCode() {
            unchecked { return X + 31 * Y; }
        }

        public override string ToString() {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}
