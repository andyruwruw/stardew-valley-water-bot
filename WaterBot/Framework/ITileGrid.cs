using Microsoft.Xna.Framework;
using WaterBot.Framework.Models;

namespace WaterBot.Framework;

/// <summary>
/// Abstraction over tile grid queries used by algorithms.
/// Implemented by <see cref="TileGrid"/> for live game data and MockTileGrid for unit tests.
/// All coordinates use standard game convention: x = column, y = row.
/// </summary>
internal interface ITileGrid
{
    /// <summary>Map width in tiles (number of columns).</summary>
    int Width { get; }

    /// <summary>Map height in tiles (number of rows).</summary>
    int Height { get; }

    /// <summary>Check whether the given tile coordinates are within the map boundaries.</summary>
    /// <param name="x">Column index.</param>
    /// <param name="y">Row index.</param>
    bool IsInBounds(int x, int y);

    /// <summary>
    /// Get a tile at the given coordinates. If the tile has not been scanned yet,
    /// query the game API (or mock) and cache the result for future lookups.
    /// </summary>
    /// <param name="x">Column index.</param>
    /// <param name="y">Row index.</param>
    TileInfo GetOrQuery(int x, int y);

    /// <summary>
    /// Get a previously scanned tile at the given position, or null if the position
    /// has not been queried yet.
    /// </summary>
    /// <param name="position">Tile position to look up.</param>
    TileInfo? GetTile(Point position);

    /// <summary>
    /// Get the 4 orthogonally adjacent tiles (up, down, left, right).
    /// Only returns tiles within map bounds; out-of-bounds neighbors are omitted.
    /// </summary>
    /// <param name="position">Center tile position.</param>
    IEnumerable<TileInfo> GetOrthogonalNeighbors(Point position);

    /// <summary>
    /// Get all 8 surrounding tiles (orthogonal + diagonal).
    /// Only returns tiles within map bounds; out-of-bounds neighbors are omitted.
    /// </summary>
    /// <param name="position">Center tile position.</param>
    IEnumerable<TileInfo> GetAllNeighbors(Point position);
}
