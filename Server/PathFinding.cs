using System.Collections.Generic;
using StrategicGame.Common;

namespace StrategicGame.Server {
    struct PathSegment {
        public readonly Int2 Start;
        public readonly Int2 End;

        public PathSegment(Int2 start, Int2 end) {
            Start = start;
            End = end;
        }
    }

    static class PathFinding {
        public static List<PathSegment> Find(Int2 from, Int2 to) { 
            return new List<PathSegment>(); 
        }
    }
}
