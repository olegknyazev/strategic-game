using System;
using System.Collections.Generic;
using StrategicGame.Common;

namespace StrategicGame.Server {
    using Path = List<PathSegment>;

    class UnitMover {
        Grid _grid;
        FindPath _findPath;

        class MovementState {
            Grid _grid;
            FindPath _findPath;
            Unit _unit;
            Int2 _position;
            Int2 _nextPosition;
            Int2 _destination;
            Path _path;
            float _progress;

            public MovementState(Grid grid, Unit unit, Int2 destination, FindPath findPath) {
                _grid = grid;
                _findPath = findPath;
                _unit = unit;
                _position = grid[_unit];
                _nextPosition = _position;
                _destination = destination;
            }

            public Unit Unit { get { return _unit; } }

            public bool Finished { get { return _position == _destination && _progress >= 1; } }

            public void SwitchDestination(Int2 to) {
                _destination = to;
            }

            public void Update(float dt) {
                var movementRemains = _unit.Speed * dt;
                while (movementRemains > 0) {
                    if (_position == _nextPosition || _progress >= 1) {
                        _position = _nextPosition;
                        _nextPosition = CellTowards(_position, _destination);
                        if (_nextPosition != _position) {
                            _grid[_unit] = _nextPosition;
                            _progress = 0;
                        } else { // if we haven't reached destination, it's unreachable
                            _destination = _position;
                            break;
                        }
                    }
                    if (_progress < 1) {
                        var drain = Math.Min(movementRemains, 1 - _progress);
                        _progress += drain;
                        movementRemains -= drain;
                        _unit.Position = Float2.Lerp(new Float2(_position), new Float2(_nextPosition), _progress);
                    }
                };
            }

            Int2 CellTowards(Int2 position, Int2 destination) {
                if (position == destination)
                    return position;
                var path = _findPath(_grid, position, destination);
                if (path.Count == 0)
                    return position;
                return CellTowardsDirect(position, path[0].End);
            }

            static Int2 CellTowardsDirect(Int2 position, Int2 destination) {
                if (position == destination)
                    return position;
                if (position.X > destination.X)
                    return new Int2(position.X - 1, position.Y);
                else if (position.X < destination.X)
                    return new Int2(position.X + 1, position.Y);
                else if (position.Y > destination.Y)
                    return new Int2(position.X, position.Y - 1);
                else if (position.Y < destination.Y)
                    return new Int2(position.X, position.Y + 1);
                else
                    return position;
            }
        }

        Dictionary<Unit, MovementState> _movingUnits = new Dictionary<Unit, MovementState>();

        public delegate Path FindPath(Grid grid, Int2 from, Int2 to);

        public struct MovedUnit {
            public readonly Unit Unit;
            public readonly bool StoppedMoving;

            public MovedUnit(Unit unit, bool stoppedMoving) {
                Unit = unit;
                StoppedMoving = stoppedMoving;
            }
        }

        public UnitMover(Grid grid, FindPath findPath) {
            _grid = grid;
            _findPath = findPath;
        }

        public void Move(Unit unit, Int2 destination) {
            MovementState state;
            if (_movingUnits.TryGetValue(unit, out state)) {
                state.SwitchDestination(destination);
                return;
            }
            state = new MovementState(_grid, unit, destination, _findPath);
            _movingUnits.Add(unit, state);
        }

        // Returns a set of units moved since last Update().
        public HashSet<MovedUnit> Update(float dt) {
            var toRemove = new List<Unit>();
            var movedUnits = new HashSet<MovedUnit>();
            foreach (var movement in _movingUnits.Values) {
                movement.Update(dt);
                if (movement.Finished)
                    toRemove.Add(movement.Unit);
                movedUnits.Add(new MovedUnit(movement.Unit, movement.Finished));
            }
            foreach (var unit in toRemove)
                _movingUnits.Remove(unit);
            return movedUnits;
        }
    }
}
