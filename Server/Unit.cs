using StrategicGame.Common;

namespace StrategicGame.Server {
    class Unit {
        public Unit(Float2 position) {
            Id = UnitId.New();
            Position = position;
        }

        public UnitId Id { get; }
        public Float2 Position { get; set; }
        public float Speed { get { return 8f; } }
    }
}
