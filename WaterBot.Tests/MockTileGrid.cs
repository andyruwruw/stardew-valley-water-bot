using Microsoft.Xna.Framework;
using WaterBot.Framework;
using WaterBot.Framework.Models;

namespace WaterBot.Tests;

/// <summary>
/// Test implementation of <see cref="ITileGrid"/> built from a character grid.
///
/// Legend:
///   '.' = walkable empty
///   'C' = crop (needs watering, walkable)
///   '#' = blocked
///   '~' = water source (walkable)
///   'W' = water source (blocked)
///   'X' = blocked crop
/// </summary>
internal class MockTileGrid : ITileGrid
{
    private readonly Dictionary<Point, TileInfo> _tiles = new();

    public int Width { get; }
    public int Height { get; }

    /// <summary>All crop tiles in the grid.</summary>
    public List<TileInfo> CropTiles { get; } = new();

    private static readonly Point[] OrthogonalOffsets = { new(0, -1), new(0, 1), new(-1, 0), new(1, 0) };
    private static readonly Point[] AllOffsets =
    {
        new(0, -1), new(0, 1), new(-1, 0), new(1, 0),
        new(-1, -1), new(1, -1), new(-1, 1), new(1, 1)
    };

    /// <summary>
    /// Build a MockTileGrid from a 2D char array.
    /// First index is row (y), second is column (x).
    /// </summary>
    public MockTileGrid(char[,] grid)
    {
        Height = grid.GetLength(0);
        Width = grid.GetLength(1);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                char c = grid[y, x];
                bool isBlocked = c is '#' or 'W' or 'X';
                bool isWaterSource = c is '~' or 'W';
                bool needsWatering = c is 'C' or 'X';

                var tile = new TileInfo(x, y, isBlocked, isWaterSource, needsWatering);
                _tiles[new Point(x, y)] = tile;

                if (needsWatering)
                    CropTiles.Add(tile);
            }
        }
    }

    public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public TileInfo? GetTile(Point position) => _tiles.GetValueOrDefault(position);

    public TileInfo GetOrQuery(int x, int y)
    {
        var key = new Point(x, y);
        if (_tiles.TryGetValue(key, out var tile))
            return tile;

        // Out-of-grid tiles are blocked empty
        var fallback = new TileInfo(x, y, isBlocked: true, isWaterSource: false, needsWatering: false);
        _tiles[key] = fallback;
        return fallback;
    }

    public IEnumerable<TileInfo> GetOrthogonalNeighbors(Point position)
    {
        foreach (var offset in OrthogonalOffsets)
        {
            int nx = position.X + offset.X;
            int ny = position.Y + offset.Y;
            if (IsInBounds(nx, ny))
                yield return GetOrQuery(nx, ny);
        }
    }

    public IEnumerable<TileInfo> GetAllNeighbors(Point position)
    {
        foreach (var offset in AllOffsets)
        {
            int nx = position.X + offset.X;
            int ny = position.Y + offset.Y;
            if (IsInBounds(nx, ny))
                yield return GetOrQuery(nx, ny);
        }
    }
}
