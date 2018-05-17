﻿using System;
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
            return new World(stepsPerSecond);
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

        // TODO use static methods instead of constructor for generation
        World(int stepsPerSecond) {
            var r = new Random();
            _width = r.Next(7, 12);
            _height = r.Next(7, 12);
            _grid = new Grid(_width, _height);
            for (int spawned = 0, count = r.Next(20, 20); spawned < count;) {
                var pos = new Int2(r.Next(0, _width - 1), r.Next(0, _height - 1));
                if (_grid[pos] == null) {
                    var unit = new Unit(new Float2(pos));
                    _units.Add(unit.Id, unit);
                    _grid[unit] = pos;
                    ++spawned;
                }
            }
            _mover = new UnitMover(_grid, Pathfinding.Find);
            _stepsPerSecond = stepsPerSecond;
            _stepTime = 1f / _stepsPerSecond;
        }

        void Do(SendUnits cmd) {
            foreach (var unitId in cmd.Units)
                _mover.Move(_units[unitId], cmd.Destination);
        }

        UnitPosition PositionOf(Unit unit, bool moving = false) {
            return new UnitPosition(unit.Id, unit.Position.X, unit.Position.Y, _frame, moving);
        }
    }
}
