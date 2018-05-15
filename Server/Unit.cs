using StrategicGame.Common;

namespace StrategicGame.Server {
    class Unit {
        UnitId _id;
        float _x;
        float _y;

        public Unit(float x, float y) {
            _id = UnitId.New();
            _x = x;
            _y = y;
        }

        public UnitId Id { get { return _id; } }

        public float X { get { return _x; } }
        public float Y { get { return _y; } }
    }
}
