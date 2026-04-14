using Microsoft.Xna.Framework;
using WaterBot.Framework.Models;

namespace WaterBot.Framework;

/// <summary>
/// Groups crop tiles into clusters using either adjacency-based DFS or greedy set cover.
/// All methods are stateless — they take input and return output with no shared mutable state.
/// </summary>
internal static class CropGrouper
{
    /// <summary>
    /// Group crop tiles by adjacency (4-connected). Tiles that share an orthogonal neighbor
    /// end up in the same group. Uses iterative DFS to avoid stack overflow on large farms.
    /// </summary>
    public static List<CropGroup> GroupByAdjacency(List<TileInfo> cropTiles, ITileGrid grid)
    {
        var groups = new List<CropGroup>();
        var cropPositions = new HashSet<Point>(cropTiles.Select(t => t.Position));
        var visited = new HashSet<Point>();

        foreach (var tile in cropTiles)
        {
            if (visited.Contains(tile.Position))
                continue;

            // Iterative DFS to find all connected crop tiles
            var group = new List<TileInfo>();
            var stack = new Stack<TileInfo>();
            stack.Push(tile);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (!visited.Add(current.Position))
                    continue;

                group.Add(current);

                // Check all 4 orthogonal neighbors
                foreach (var neighbor in grid.GetOrthogonalNeighbors(current.Position))
                {
                    if (cropPositions.Contains(neighbor.Position) && !visited.Contains(neighbor.Position))
                        stack.Push(neighbor);
                }
            }

            groups.Add(new CropGroup(group));
        }

        Logger.Debug($"CropGrouper: adjacency grouping produced {groups.Count} groups from {cropTiles.Count} crop tiles.");
        return groups;
    }

    /// <summary>
    /// Group crop tiles using greedy set cover. Repeatedly picks the standing position
    /// that can water the most uncovered crops (within 8-neighbor range), then removes
    /// those crops from the uncovered set. Produces fewer, larger groups.
    /// </summary>
    public static List<CropGroup> GroupByMinimalCover(List<TileInfo> cropTiles, ITileGrid grid)
    {
        var groups = new List<CropGroup>();
        var uncovered = new HashSet<Point>(cropTiles.Select(t => t.Position));

        while (uncovered.Count > 0)
        {
            // Find the tile position that covers the most uncovered crops.
            // A "standing position" covers all crop tiles within its 8 neighbors + itself.
            Point bestStand = Point.Zero;
            List<TileInfo> bestCovered = new();
            TileInfo? bestStandTile = null;

            // Candidates: every uncovered crop tile and its walkable neighbors
            var candidates = new HashSet<Point>();
            foreach (var pos in uncovered)
            {
                candidates.Add(pos);
                foreach (var neighbor in grid.GetAllNeighbors(pos))
                {
                    if (!neighbor.IsBlocked)
                        candidates.Add(neighbor.Position);
                }
            }

            foreach (var candidatePos in candidates)
            {
                var candidateTile = grid.GetOrQuery(candidatePos.X, candidatePos.Y);

                // Must be able to stand here
                if (candidateTile.IsBlocked)
                    continue;

                // Compute which uncovered crops this position can water
                var covered = new List<TileInfo>();

                // Can water self if it's an uncovered crop
                if (uncovered.Contains(candidatePos))
                    covered.Add(candidateTile);

                // Can water all 8 neighbors that are uncovered crops
                foreach (var neighbor in grid.GetAllNeighbors(candidatePos))
                {
                    if (uncovered.Contains(neighbor.Position))
                        covered.Add(neighbor);
                }

                if (covered.Count > bestCovered.Count)
                {
                    bestCovered = covered;
                    bestStand = candidatePos;
                    bestStandTile = candidateTile;
                }
            }

            if (bestCovered.Count == 0)
            {
                // Remaining crops are unreachable from any walkable tile — add them as solo groups
                foreach (var pos in uncovered.ToList())
                {
                    var tile = grid.GetOrQuery(pos.X, pos.Y);
                    groups.Add(new CropGroup(new List<TileInfo> { tile }));
                }
                uncovered.Clear();
                break;
            }

            // Remove covered crops from uncovered set
            foreach (var tile in bestCovered)
                uncovered.Remove(tile.Position);

            groups.Add(new CropGroup(bestCovered, standingCenter: bestStandTile));
        }

        Logger.Debug($"CropGrouper: minimal cover produced {groups.Count} groups from {cropTiles.Count} crop tiles.");
        return groups;
    }
}
