using System;
using System.Collections.Generic;
using System.Linq;
using StrategicGame.Common;

namespace StrategicGame.Server {
    class World {
        int _width;
        int _height;
        Grid _grid;
        Dictionary<UnitId, Unit> _units = new Dictionary<UnitId, Unit>();
        UnitMover _mover;
        int _stepsPerSecond;
        float _stepTime;
        uint _frame;
        
        public static World RandomWorld(int stepsPerSecond) {
            var rnd = new Random();
            var world = new World(rnd.Next(7, 12), rnd.Next(7, 12), stepsPerSecond);
            world.SpawnRandomUnits(rnd.Next(3, 5), rnd);
            return world;
        }
        
        public IEnumerable<StatePortion> InstantState {
            get { 
                yield return new WorldParameters((short)_width, (short)_height);
                foreach (var unit in _units.Values)
                    yield return PositionOf(unit);
            }
        }
        
        public List<StatePortion> Simulate(List<Command> commands) {
            foreach (var cmd in commands)
                if (cmd is SendUnits)
                    Do((SendUnits)cmd);
            var movedUnits = _mover.Update(_stepTime);
            ++_frame;
            return movedUnits
                .Select(mu => PositionOf(mu.Unit, !mu.StoppedMoving))
                .OfType<StatePortion>()
                .ToList();
        }
        
        World(int width, int height, int stepsPerSecond) {
            _width = width;
            _height = height;
            _grid = new Grid(width, height);
            _mover = new UnitMover(_grid, Pathfinding.Find);
            _stepsPerSecond = stepsPerSecond;
            _stepTime = 1f / _stepsPerSecond;
        }

        void SpawnRandomUnits(int count, Random rnd) {
            for (int spawned = 0; spawned < count;) {
                var pos = new Int2(rnd.Next(0, _width - 1), rnd.Next(0, _height - 1));
                if (_grid[pos] == null) {
                    var unit = new Unit(new Float2(pos));
                    _units.Add(unit.Id, unit);
                    _grid[unit] = pos;
                    ++spawned;
                }
            }
        }

        void Do(SendUnits cmd) {
            foreach (var unitId in cmd.Units)
                _mover.Move(_units[unitId], cmd.Destination);
        }

        UnitPosition PositionOf(Unit unit, bool moving = false) {
            return new UnitPosition(unit.Id, unit.Position.X, unit.Position.Y, _frame, moving);
        }

        public override string ToString() {
            return string.Format("[World {0} x {1}, {2} units]", _width, _height, _grid.UnitCount);
        }
    }
}
