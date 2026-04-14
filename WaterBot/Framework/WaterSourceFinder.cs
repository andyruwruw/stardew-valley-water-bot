using Microsoft.Xna.Framework;
using WaterBot.Framework.Models;

namespace WaterBot.Framework;

/// <summary>
/// BFS search for the nearest water source to refill the watering can.
/// Stateless — uses local <see cref="HashSet{T}"/> for visited tracking.
/// </summary>
internal static class WaterSourceFinder
{
    /// <summary>
    /// Find the nearest reachable water source from the given start position.
    /// Returns a <see cref="WateringAction"/> with the standing position and refill target,
    /// or null if no water source is reachable.
    /// </summary>
    public static WateringAction? FindNearest(Point start, ITileGrid grid)
    {
        var visited = new HashSet<Point>();
        var queue = new Queue<(Point Current, Point Previous)>();
        queue.Enqueue((start, start));

        while (queue.Count > 0)
        {
            var (current, previous) = queue.Dequeue();

            if (!visited.Add(current))
                continue;

            var tile = grid.GetOrQuery(current.X, current.Y);

            // Found a water source
            if (tile.IsWaterSource)
            {
                // If the water tile is blocked, stand on the previous tile
                var standPos = tile.IsBlocked ? previous : current;

                return new WateringAction(
                    standPos,
                    new[] { current },
                    WateringAction.ActionType.Refill);
            }

            // Don't expand through blocked tiles (except the start position)
            if (tile.IsBlocked && current != start)
                continue;

            // Expand to orthogonal neighbors
            foreach (var neighbor in grid.GetOrthogonalNeighbors(current))
            {
                if (!visited.Contains(neighbor.Position))
                {
                    // Only expand to walkable tiles or water sources
                    if (!neighbor.IsBlocked || neighbor.IsWaterSource)
                        queue.Enqueue((neighbor.Position, current));
                }
            }
        }

        Logger.Warn("WaterSourceFinder: no reachable water source found.");
        return null;
    }

}
