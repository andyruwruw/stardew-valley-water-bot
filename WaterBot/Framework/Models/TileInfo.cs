using Microsoft.Xna.Framework;

namespace WaterBot.Framework.Models;

/// <summary>
/// Represents a single map tile with its static properties.
/// Coordinates and blocking/water-source status are immutable after construction.
/// <see cref="NeedsWatering"/> is mutable — the bot marks tiles as handled during operation.
/// </summary>
internal sealed class TileInfo : IEquatable<TileInfo>
{
    /// <summary>Tile X coordinate (column).</summary>
    public int X { get; }

    /// <summary>Tile Y coordinate (row).</summary>
    public int Y { get; }

    /// <summary>Whether this tile blocks player movement.</summary>
    public bool IsBlocked { get; }

    /// <summary>Whether this tile can refill a watering can (water body).</summary>
    public bool IsWaterSource { get; }

    /// <summary>Whether this tile has an unwatered crop. Set to false by the bot after watering.</summary>
    public bool NeedsWatering { get; set; }

    /// <summary>This tile's position as a <see cref="Point"/>.</summary>
    public Point Position => new(X, Y);

    public TileInfo(int x, int y, bool isBlocked, bool isWaterSource, bool needsWatering)
    {
        X = x;
        Y = y;
        IsBlocked = isBlocked;
        IsWaterSource = isWaterSource;
        NeedsWatering = needsWatering;
    }

    /// <summary>Manhattan distance to another point.</summary>
    public int ManhattanDistanceTo(Point other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    public bool Equals(TileInfo? other)
    {
        if (other is null) return false;
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj) => Equals(obj as TileInfo);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString()
    {
        if (IsWaterSource) return "~";
        if (NeedsWatering && IsBlocked) return "0";
        if (NeedsWatering) return "O";
        if (IsBlocked) return "#";
        return " ";
    }
}
