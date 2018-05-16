namespace StrategicGame.Common {
    public struct Int2 {
        public readonly int X, Y;

        public static readonly Int2 Left = new Int2(-1, 0);
        public static readonly Int2 Right = new Int2(1, 0);
        public static readonly Int2 Up = new Int2(0, -1);
        public static readonly Int2 Down = new Int2(0, 1);

        public Int2(int value) : this(value, value) { }
        public Int2(int x, int y) { X = x; Y = y; }

        public int DistanceSquared(Int2 other) {
            var d = this - other;
            return d.X * d.X + d.Y * d.Y;
        }

        public static Int2 operator+(Int2 a, Int2 b) {
            return new Int2(a.X + b.X, a.Y + b.Y);
        }
        
        public static Int2 operator-(Int2 a, Int2 b) {
            return new Int2(a.X - b.X, a.Y - b.Y);
        }

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
