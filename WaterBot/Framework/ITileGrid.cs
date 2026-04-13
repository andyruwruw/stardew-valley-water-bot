using Microsoft.Xna.Framework;
using WaterBot.Framework.Models;

namespace WaterBot.Framework;

/// <summary>
/// Abstraction over tile grid queries used by algorithms.
/// Implemented by <see cref="TileGrid"/> (game) and MockTileGrid (tests).
/// </summary>
internal interface ITileGrid
{
    int Width { get; }
    int Height { get; }
    bool IsInBounds(int x, int y);
    TileInfo GetOrQuery(int x, int y);
    TileInfo? GetTile(Point position);
    IEnumerable<TileInfo> GetOrthogonalNeighbors(Point position);
    IEnumerable<TileInfo> GetAllNeighbors(Point position);
}
