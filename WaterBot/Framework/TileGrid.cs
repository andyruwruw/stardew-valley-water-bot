using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using WaterBot.Framework.Models;

namespace WaterBot.Framework;

/// <summary>
/// Scans a game location for crop tiles and provides tile queries for pathfinding algorithms.
///
/// Coordinate convention: all (x, y) parameters and Point values use standard
/// game coordinates where x = column and y = row. No coordinate swapping anywhere.
/// </summary>
internal sealed class TileGrid : ITileGrid
{
    private static readonly Point[] OrthogonalOffsets = { new(0, -1), new(0, 1), new(-1, 0), new(1, 0) };
    private static readonly Point[] DiagonalOffsets = { new(-1, -1), new(1, -1), new(-1, 1), new(1, 1) };
    private static readonly Point[] AllOffsets =
    {
        new(0, -1), new(0, 1), new(-1, 0), new(1, 0),
        new(-1, -1), new(1, -1), new(-1, 1), new(1, 1)
    };

    private readonly Dictionary<Point, TileInfo> _tiles = new();
    private GameLocation _location = null!;

    /// <summary>Map width in tiles (columns).</summary>
    public int Width { get; private set; }

    /// <summary>Map height in tiles (rows).</summary>
    public int Height { get; private set; }

    /// <summary>All tiles that have an unwatered crop.</summary>
    public List<TileInfo> CropTiles { get; } = new();

    /// <summary>
    /// Scan the given location for crop tiles and their neighborhoods.
    /// Only tiles near crops are stored — not the entire map.
    /// </summary>
    public void Load(GameLocation location)
    {
        _location = location;
        _tiles.Clear();
        CropTiles.Clear();

        Width = location.map.Layers[0].LayerWidth;
        Height = location.map.Layers[0].LayerHeight;

        // Pass 1: find all crop tiles by iterating terrain features (not all W×H tiles)
        foreach (var pair in location.terrainFeatures.Pairs)
        {
            int x = (int)pair.Key.X;
            int y = (int)pair.Key.Y;

            if (pair.Value is HoeDirt hoeDirt && CropNeedsWatering(hoeDirt))
            {
                var tile = GetOrCreateTile(x, y);
                CropTiles.Add(tile);
            }
        }

        // Pass 2: scan the 8 neighbors of every crop tile so algorithms have
        // passability/water-source data for standing position selection
        foreach (var cropTile in CropTiles.ToArray())
        {
            foreach (var offset in AllOffsets)
            {
                int nx = cropTile.X + offset.X;
                int ny = cropTile.Y + offset.Y;
                if (IsInBounds(nx, ny))
                    GetOrCreateTile(nx, ny);
            }
        }

        Logger.Debug($"TileGrid loaded: {CropTiles.Count} crop tiles, {_tiles.Count} total tiles scanned (map is {Width}x{Height}).");
    }

    /// <summary>Whether a tile position is within map bounds.</summary>
    public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    /// <summary>Get a previously scanned tile, or null if not in the grid.</summary>
    public TileInfo? GetTile(Point position) => _tiles.GetValueOrDefault(position);

    /// <summary>
    /// Get a tile from the grid, or query the game API and cache it if not yet scanned.
    /// Use this for algorithms (like BFS) that may expand beyond the initial crop neighborhood.
    /// </summary>
    public TileInfo GetOrQuery(int x, int y)
    {
        var key = new Point(x, y);
        if (_tiles.TryGetValue(key, out var existing))
            return existing;

        return CreateAndCacheTile(x, y);
    }

    /// <summary>Get the 4 orthogonally adjacent tiles (up, down, left, right).</summary>
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

    /// <summary>Get all 8 surrounding tiles (orthogonal + diagonal).</summary>
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

    /// <summary>Check whether a tile blocks player movement. Queries the game API directly.</summary>
    public bool QueryIsBlocked(int x, int y)
    {
        // isCollidingPosition returns true when the tile BLOCKS movement.
        // Rectangle constructor: (x pixels, y pixels, width, height)
        return _location.isCollidingPosition(
            new Rectangle(x * 64 + 1, y * 64 + 1, 62, 62),
            Game1.viewport, isFarmer: true, -1, glider: false, Game1.player);
    }

    /// <summary>Check whether a tile can refill a watering can. Queries the game API directly.</summary>
    public bool QueryIsWaterSource(int x, int y)
    {
        // CanRefillWateringCanOnTile(tileX, tileY) — standard (x, y) order
        return _location.CanRefillWateringCanOnTile(x, y);
    }

    /// <summary>Check whether a HoeDirt tile has a crop that needs watering.</summary>
    private static bool CropNeedsWatering(HoeDirt hoeDirt)
    {
        // state 0 = dry, non-zero = already watered
        if (hoeDirt.state.Value != 0)
            return false;

        var crop = hoeDirt.crop;
        if (crop == null || crop.dead.Value)
            return false;

        // Needs watering if: still growing, or fully grown but has days remaining, or regrows after harvest
        return (crop.fullyGrown.Value && crop.dayOfCurrentPhase.Value > 0)
            || (crop.currentPhase.Value < crop.phaseDays.Count - 1)
            || crop.RegrowsAfterHarvest();
    }

    /// <summary>Get a tile from the cache, or create and cache it.</summary>
    private TileInfo GetOrCreateTile(int x, int y)
    {
        var key = new Point(x, y);
        if (_tiles.TryGetValue(key, out var existing))
            return existing;

        return CreateAndCacheTile(x, y);
    }

    /// <summary>Query game APIs for tile properties and cache the result.</summary>
    private TileInfo CreateAndCacheTile(int x, int y)
    {
        bool isBlocked = QueryIsBlocked(x, y);
        bool isWaterSource = QueryIsWaterSource(x, y);

        // Check if this tile needs watering (may be a crop tile discovered via neighbor scan)
        bool needsWatering = false;
        var vec = new Vector2(x, y);
        if (_location.terrainFeatures.TryGetValue(vec, out var feature) && feature is HoeDirt hoeDirt)
            needsWatering = CropNeedsWatering(hoeDirt);

        var tile = new TileInfo(x, y, isBlocked, isWaterSource, needsWatering);
        _tiles[new Point(x, y)] = tile;

        // If we discovered an unwatered crop via neighbor scan that wasn't in the initial pass, add it
        if (needsWatering && !CropTiles.Contains(tile))
            CropTiles.Add(tile);

        return tile;
    }
}
