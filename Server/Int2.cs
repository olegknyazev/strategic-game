namespace StrategicGame.Server {
    struct Int2 {
        public readonly int X, Y;

        public Int2(int value) : this(value, value) { }
        public Int2(int x, int y) { X = x; Y = y; }
    }
}
