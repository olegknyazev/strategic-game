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
        public static List<PathSegment> Find(Grid grid, Int2 from, Int2 to, int maxLength = -1) {
            var pathInfo = new Dictionary<Int2, PathNode> {{ from, new PathNode(from) }};
            var toProcess = new List<NodeToProcess> { new NodeToProcess(from, 0) };
            Int2 cell = from;
            while (toProcess.Count > 0) {
                int minIdx = 0;
                for (int i = 1; i < toProcess.Count; ++i)
                    if (toProcess[i].EstimatedDistance < toProcess[minIdx].EstimatedDistance)
                        minIdx = i;
                cell = toProcess[minIdx].Node;
                toProcess.RemoveAt(minIdx);
                var currentLength = pathInfo[cell].PathLength;
                if (cell == to || currentLength == maxLength)
                    break;
                var lengthFromCurrent = currentLength + 1;
                foreach (var neighbour in Neighbours(cell))
                    if (grid[neighbour] == null) {
                        PathNode pathNode;
                        if (pathInfo.TryGetValue(neighbour, out pathNode)) {
                            if (lengthFromCurrent < pathNode.PathLength)
                                pathInfo[neighbour] = new PathNode(cell, lengthFromCurrent);
                        } else {
                            pathInfo.Add(neighbour, new PathNode(cell, lengthFromCurrent));
                            toProcess.Add(new NodeToProcess(neighbour, neighbour.DistanceSquared(to)));
                        }
                }
            }
            var path = new List<PathSegment>();
            var curr = cell;
            PathNode node;
            while ((node = pathInfo[curr]).PathLength > 0) {
                var prev = node.ComeFrom;
                path.Add(new PathSegment(prev, curr));
                curr = prev;
            };
            path.Reverse();
            return path; 
        }

        struct NodeToProcess {
            public readonly Int2 Node;
            public readonly int EstimatedDistance;

            public NodeToProcess(Int2 node, int distance) {
                Node = node;
                EstimatedDistance = distance;
            }
        }

        struct PathNode {
            public readonly Int2 ComeFrom;
            public readonly int PathLength;

            public PathNode(Int2 comeFrom, int pathLength = 0) {
                ComeFrom = comeFrom;
                PathLength = pathLength;
            }
        }

        static IEnumerable<Int2> Neighbours(Int2 cell) {
            yield return cell + Int2.Left;
            yield return cell + Int2.Right;
            yield return cell + Int2.Up;
            yield return cell + Int2.Down;
        }
    }
}
