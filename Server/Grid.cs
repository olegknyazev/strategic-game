namespace StrategicGame.Server {
    class Grid<T> {
        T[] _data;

        public Grid(int width, int height) {
            Width = width;
            Height = height;
            _data = new T[width * height];
        }

        public T this[int x, int y] {
            get { return _data[Idx(x, y)]; }
            set { _data[Idx(x, y)] = value; }
        }

        public int Width { get; }
        public int Height { get; }

        int Idx(int x, int y) { return y * Width + x; }
    }
}
