using StrategicGame.Common;
using System;
using System.Collections.Generic;

namespace StrategicGame.Server {
    class Unit {
        static uint _s_id;

        uint _id;
        float _x;
        float _y;

        public Unit(float x, float y) {
            _id = ++_s_id;
            _x = x;
            _y = y;
        }

        public uint Id { get { return _id; } }

        public float X { get { return _x; } }
        public float Y { get { return _y; } }
    }
}
