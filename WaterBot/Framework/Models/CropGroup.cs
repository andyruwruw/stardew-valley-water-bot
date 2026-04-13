using Microsoft.Xna.Framework;

namespace WaterBot.Framework.Models;

/// <summary>
/// A cluster of adjacent crop tiles that can be watered as a unit.
/// Centroid is computed at construction. Contains uses O(1) HashSet lookup.
/// </summary>
internal sealed class CropGroup
{
    private readonly List<TileInfo> _tiles;
    private readonly HashSet<Point> _positions;

    /// <summary>The tiles in this group.</summary>
    public IReadOnlyList<TileInfo> Tiles => _tiles;

    /// <summary>The geometric center of the group, snapped to the nearest walkable tile.</summary>
    public Point Centroid { get; }

    /// <summary>
    /// Optional pre-determined standing center (used by minimal cover grouping
    /// where the standing position is known at grouping time).
    /// </summary>
    public TileInfo? StandingCenter { get; }

    public CropGroup(List<TileInfo> tiles, TileInfo? standingCenter = null)
    {
        _tiles = tiles;
        _positions = new HashSet<Point>(tiles.Select(t => t.Position));
        StandingCenter = standingCenter;
        Centroid = standingCenter?.Position ?? ComputeCentroid(tiles);
    }

    /// <summary>Whether the group contains a tile at the given position. O(1).</summary>
    public bool Contains(Point position) => _positions.Contains(position);

    /// <summary>Find the tile in this group nearest to the given position (Euclidean).</summary>
    public TileInfo FindClosestTile(Point target)
    {
        TileInfo? best = null;
        var bestDistSq = double.MaxValue;

        foreach (var tile in _tiles)
        {
            double dx = target.X - tile.X;
            double dy = target.Y - tile.Y;
            double distSq = dx * dx + dy * dy;

            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                best = tile;
            }
        }

        return best!;
    }

    /// <summary>Compute the centroid as the tile in the list closest to the average position.</summary>
    private static Point ComputeCentroid(List<TileInfo> tiles)
    {
        if (tiles.Count == 0)
            return Point.Zero;

        int sumX = 0, sumY = 0;
        foreach (var tile in tiles)
        {
            sumX += tile.X;
            sumY += tile.Y;
        }

        var avgX = sumX / tiles.Count;
        var avgY = sumY / tiles.Count;
        var avg = new Point(avgX, avgY);

        // Prefer walkable tiles near the centroid
        TileInfo? bestWalkable = null;
        var bestWalkableDist = int.MaxValue;
        TileInfo? bestAny = null;
        var bestAnyDist = int.MaxValue;

        foreach (var tile in tiles)
        {
            int dist = tile.ManhattanDistanceTo(avg);
            if (!tile.IsBlocked && dist < bestWalkableDist)
            {
                bestWalkableDist = dist;
                bestWalkable = tile;
            }
            if (dist < bestAnyDist)
            {
                bestAnyDist = dist;
                bestAny = tile;
            }
        }

        return (bestWalkable ?? bestAny)?.Position ?? avg;
    }
}
