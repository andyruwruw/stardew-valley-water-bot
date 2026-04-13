using Microsoft.Xna.Framework;
using WaterBot.Framework.Models;

namespace WaterBot.Framework;

/// <summary>
/// Orders crop groups by nearest-neighbor traversal and plans within-group watering paths.
/// All methods are stateless.
/// </summary>
internal static class RoutePlanner
{
    /// <summary>
    /// Order groups by greedy nearest-neighbor: starting from the player position,
    /// always move to the closest unvisited group centroid (Manhattan distance).
    /// </summary>
    public static List<CropGroup> OrderGroups(List<CropGroup> groups, Point playerPosition)
    {
        if (groups.Count <= 1)
            return new List<CropGroup>(groups);

        var remaining = new HashSet<int>(Enumerable.Range(0, groups.Count));
        var ordered = new List<CropGroup>(groups.Count);
        var current = playerPosition;

        while (remaining.Count > 0)
        {
            int nearest = -1;
            int nearestDist = int.MaxValue;

            foreach (int i in remaining)
            {
                int dist = ManhattanDistance(current, groups[i].Centroid);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = i;
                }
            }

            remaining.Remove(nearest);
            ordered.Add(groups[nearest]);
            current = groups[nearest].Centroid;
        }

        Logger.Debug($"RoutePlanner: ordered {ordered.Count} groups starting from ({playerPosition.X}, {playerPosition.Y}).");
        return ordered;
    }

    /// <summary>
    /// Plan the watering path within a single group. For each crop tile, determine
    /// a standing position and which tiles can be watered from there.
    /// Returns a sequence of <see cref="WateringAction"/>s.
    /// </summary>
    public static List<WateringAction> PlanWateringPath(CropGroup group, Point playerPosition, ITileGrid grid)
    {
        // If the group has a pre-determined standing center (from minimal cover),
        // produce a single action that waters all tiles from that position.
        if (group.StandingCenter is not null)
        {
            var action = new WateringAction(
                group.StandingCenter.Position,
                group.Tiles.Select(t => t.Position),
                WateringAction.ActionType.Water);
            return new List<WateringAction> { action };
        }

        var actions = new List<WateringAction>();
        var watered = new HashSet<Point>();
        var visited = new HashSet<Point>();

        // Start DFS from the tile in the group closest to the player
        var startTile = group.FindClosestTile(playerPosition);
        var stack = new Stack<TileInfo>();
        stack.Push(startTile);

        // Outer loop: handles disconnected sub-regions within the group
        bool keepGoing;
        do
        {
            keepGoing = false;

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (!visited.Add(current.Position))
                    continue;

                // If already watered by a previous action's reach, just traverse through
                if (watered.Contains(current.Position))
                {
                    PushUnvisitedGroupNeighbors(current.Position, group, visited, grid, stack);
                    continue;
                }

                // Determine where to stand to water this tile
                Point standPos;
                if (!current.IsBlocked)
                {
                    standPos = current.Position;
                }
                else
                {
                    // Can't stand on this tile — find the best adjacent walkable tile
                    var best = FindBestStandingNeighbor(current.Position, group, watered, visited, grid);
                    if (best == null)
                        continue; // unreachable tile, skip
                    standPos = best.Value;
                }

                // Build the watering action from this standing position
                var action = new WateringAction(standPos, WateringAction.ActionType.Water);

                // Water the standing tile itself if it's an unwatered crop
                TryWaterTile(standPos, group, watered, action);

                // Water all 4 orthogonal neighbors that are unwatered crops
                foreach (var neighbor in grid.GetOrthogonalNeighbors(standPos))
                {
                    TryWaterTile(neighbor.Position, group, watered, action);
                    if (!visited.Contains(neighbor.Position) && group.Contains(neighbor.Position))
                        stack.Push(neighbor);
                }

                // Water all 4 diagonal neighbors that are unwatered crops
                foreach (var neighbor in grid.GetAllNeighbors(standPos))
                {
                    // GetAllNeighbors includes orthogonal — only process diagonals here
                    var offset = new Point(neighbor.X - standPos.X, neighbor.Y - standPos.Y);
                    if (offset.X != 0 && offset.Y != 0) // diagonal
                        TryWaterTile(neighbor.Position, group, watered, action);
                }

                if (action.Targets.Count > 0)
                    actions.Add(action);
            }

            // Check for any unvisited tiles in the group (disconnected sub-region)
            foreach (var tile in group.Tiles)
            {
                if (!visited.Contains(tile.Position))
                {
                    stack.Push(tile);
                    keepGoing = true;
                    break;
                }
            }
        } while (keepGoing);

        return actions;
    }

    /// <summary>If the position is an unwatered crop in the group, add it to the action and mark watered.</summary>
    private static void TryWaterTile(Point position, CropGroup group, HashSet<Point> watered, WateringAction action)
    {
        if (group.Contains(position) && watered.Add(position))
            action.EnqueueTarget(position);
    }

    /// <summary>Find the best walkable neighbor to stand on to water a blocked tile.</summary>
    private static Point? FindBestStandingNeighbor(Point blockedTile, CropGroup group, HashSet<Point> watered, HashSet<Point> visited, ITileGrid grid)
    {
        Point? best = null;
        int bestScore = -1;

        foreach (var neighbor in grid.GetAllNeighbors(blockedTile))
        {
            if (neighbor.IsBlocked)
                continue;

            int score = 0;
            if (group.Contains(neighbor.Position)) score += 4;
            if (neighbor.NeedsWatering) score += 3;
            if (!watered.Contains(neighbor.Position)) score += 2;
            if (!visited.Contains(neighbor.Position)) score += 1;

            if (score > bestScore)
            {
                bestScore = score;
                best = neighbor.Position;
            }
        }

        return best;
    }

    /// <summary>Push unvisited group members adjacent to the given position onto the DFS stack.</summary>
    private static void PushUnvisitedGroupNeighbors(Point position, CropGroup group, HashSet<Point> visited, ITileGrid grid, Stack<TileInfo> stack)
    {
        foreach (var neighbor in grid.GetOrthogonalNeighbors(position))
        {
            if (!visited.Contains(neighbor.Position) && group.Contains(neighbor.Position))
                stack.Push(neighbor);
        }
    }

    private static int ManhattanDistance(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}
