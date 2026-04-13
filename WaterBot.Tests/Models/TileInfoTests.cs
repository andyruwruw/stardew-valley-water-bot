using Microsoft.Xna.Framework;
using WaterBot.Framework.Models;
using Xunit;

namespace WaterBot.Tests.Models;

public class TileInfoTests
{
    [Fact]
    public void Equals_SameCoords_ReturnsTrue()
    {
        var a = new TileInfo(3, 5, false, false, false);
        var b = new TileInfo(3, 5, true, true, true);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentCoords_ReturnsFalse()
    {
        var a = new TileInfo(3, 5, false, false, false);
        var b = new TileInfo(4, 5, false, false, false);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void GetHashCode_SameCoords_SameHash()
    {
        var a = new TileInfo(7, 2, false, false, false);
        var b = new TileInfo(7, 2, true, true, true);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ManhattanDistanceTo_KnownDistance()
    {
        var tile = new TileInfo(1, 1, false, false, false);
        Assert.Equal(5, tile.ManhattanDistanceTo(new Point(4, 3)));
    }

    [Fact]
    public void ManhattanDistanceTo_SamePoint_Zero()
    {
        var tile = new TileInfo(3, 3, false, false, false);
        Assert.Equal(0, tile.ManhattanDistanceTo(new Point(3, 3)));
    }

    [Fact]
    public void Position_ReturnsCorrectPoint()
    {
        var tile = new TileInfo(4, 7, false, false, false);
        Assert.Equal(new Point(4, 7), tile.Position);
    }

    [Fact]
    public void NeedsWatering_IsMutable()
    {
        var tile = new TileInfo(0, 0, false, false, needsWatering: true);
        Assert.True(tile.NeedsWatering);
        tile.NeedsWatering = false;
        Assert.False(tile.NeedsWatering);
    }

    [Fact]
    public void HashSet_WorksCorrectly()
    {
        var set = new HashSet<TileInfo>
        {
            new TileInfo(1, 2, false, false, false),
            new TileInfo(1, 2, true, true, true), // duplicate coords
            new TileInfo(3, 4, false, false, false)
        };
        Assert.Equal(2, set.Count);
    }
}
