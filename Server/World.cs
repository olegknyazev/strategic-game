using StrategicGame.Common;
using System.Collections.Generic;

namespace StrategicGame.Server {
    class World {
        int _width;
        int _height;

        public World(int width, int height) {
            _width = width;
            _height = height;
        }

        public IEnumerable<Message> Status { 
            get { yield return new WorldParameters((short)_width, (short)_height); } 
        }
    }
}
