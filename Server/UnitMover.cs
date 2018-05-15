using System.Collections.Generic;
using StrategicGame.Common;

namespace StrategicGame.Server {
    class UnitMover {
        Grid<Unit> _grid;
        FindPath _findPath;

        public delegate List<PathSegment> FindPath(Grid<Unit> grid, Int2 from, Int2 to);

        public UnitMover(Grid<Unit> grid, FindPath findPath) {
            _grid = grid;
            _findPath = findPath;
        }

        // There should a Unit at from.
        public void Move(Int2 from, Int2 to) { }

        // Returns a set of units that are moved since last Update().
        public HashSet<Unit> Update() { return new HashSet<Unit>(); }
    }
}
