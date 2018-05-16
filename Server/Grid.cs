using StrategicGame.Common;
using System.Collections.Generic;

namespace StrategicGame.Server {
    class Grid {
        Dictionary<Unit, Int2> _positionByUnit = new Dictionary<Unit, Int2>();
        Dictionary<Int2, Unit> _unitByPosition = new Dictionary<Int2, Unit>();

        public Grid(int width, int height) {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }

        public Unit this[int x, int y] {
            get { return this[new Int2(x, y)]; }
        }

        public Unit this[Int2 position] {
            get { 
                Unit unit = null;
                _unitByPosition.TryGetValue(position, out unit);
                return unit;
            }
        }

        public Int2 this[Unit unit] {
            get { return _positionByUnit[unit]; }
            set {
                Int2 previousPosition;
                if (_positionByUnit.TryGetValue(unit, out previousPosition))
                    _unitByPosition.Remove(previousPosition);
                _positionByUnit[unit] = value;
                _unitByPosition[value] = unit;
            }
        }
    }
}
