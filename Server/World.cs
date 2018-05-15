using StrategicGame.Common;
using System;
using System.Collections.Generic;

namespace StrategicGame.Server {
    class World {
        int _width;
        int _height;
        List<Unit> _units = new List<Unit>();
        
        public static World RandomWorld() { return new World(); }

        public IEnumerable<Message> Status { 
            get { 
                yield return new WorldParameters((short)_width, (short)_height);
                foreach (var unit in _units)
                    yield return new UnitPosition(unit.Id, unit.X, unit.Y, 0);
            }
        }
        
        public List<Message> Simulate(List<Message> commands) {
            return new List<Message>();
        }

        World() {
            var r = new Random();
            _width = r.Next(7, 12);
            _height = r.Next(7, 12);
            for (int i = 0, count = r.Next(2, 5); i < count; ++i)
                _units.Add(new Unit(r.Next(0, _width - 1), r.Next(0, _height - 1)));
        }
    }
}
