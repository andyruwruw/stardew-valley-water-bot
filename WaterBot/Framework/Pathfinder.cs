using Microsoft.Xna.Framework;

namespace WaterBot.Framework;

/// <summary>
/// BFS pathfinding through an <see cref="ITileGrid"/>.
/// Stateless, no game dependency — testable with MockTileGrid.
/// </summary>
internal static class Pathfinder
{
    /// <summary>
    /// BFS pathfinding from start to end through walkable tiles.
    /// Returns a queue of tile waypoints (excluding start, including end), or null if unreachable.
    /// The end tile is allowed even if blocked (farmer may need to reach a blocked crop's neighbor).
    /// </summary>
    public static Queue<Point>? FindPath(Point start, Point end, ITileGrid grid)
    {
        if (start == end)
            return new Queue<Point>();

        var visited = new HashSet<Point> { start };
        var queue = new Queue<Point>();
        var cameFrom = new Dictionary<Point, Point>();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var neighbor in grid.GetOrthogonalNeighbors(current))
            {
                if (!visited.Add(neighbor.Position))
                    continue;

                bool isEnd = neighbor.Position == end;

                // Allow the end tile even if blocked
                if (!isEnd && neighbor.IsBlocked)
                    continue;

                cameFrom[neighbor.Position] = current;

                if (isEnd)
                    return ReconstructPath(start, end, cameFrom);

                queue.Enqueue(neighbor.Position);
            }
        }

        return null;
    }

    private static Queue<Point> ReconstructPath(Point start, Point end, Dictionary<Point, Point> cameFrom)
    {
        var path = new List<Point>();
        var step = end;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Reverse();
        return new Queue<Point>(path);
    }
}
