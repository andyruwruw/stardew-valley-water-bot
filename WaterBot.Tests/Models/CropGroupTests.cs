using Microsoft.Xna.Framework;
using WaterBot.Framework.Models;
using Xunit;

namespace WaterBot.Tests.Models;

public class CropGroupTests
{
    [Fact]
    public void Contains_PresentTile_ReturnsTrue()
    {
        var tiles = new List<TileInfo> { new(1, 2, false, false, true), new(3, 4, false, false, true) };
        var group = new CropGroup(tiles);
        Assert.True(group.Contains(new Point(1, 2)));
        Assert.True(group.Contains(new Point(3, 4)));
    }

    [Fact]
    public void Contains_AbsentTile_ReturnsFalse()
    {
        var tiles = new List<TileInfo> { new(1, 2, false, false, true) };
        var group = new CropGroup(tiles);
        Assert.False(group.Contains(new Point(5, 5)));
    }

    [Fact]
    public void FindClosestTile_ReturnsNearest()
    {
        var tiles = new List<TileInfo>
        {
            new(0, 0, false, false, true),
            new(10, 10, false, false, true),
            new(3, 3, false, false, true)
        };
        var group = new CropGroup(tiles);
        var closest = group.FindClosestTile(new Point(2, 2));
        Assert.Equal(new Point(3, 3), closest.Position);
    }

    [Fact]
    public void Centroid_PrefersWalkableTile()
    {
        // 3 tiles: (0,0) walkable, (1,0) blocked, (2,0) walkable
        // Geometric center is (1,0) which is blocked → should pick (0,0) or (2,0)
        var tiles = new List<TileInfo>
        {
            new(0, 0, isBlocked: false, false, true),
            new(1, 0, isBlocked: true, false, true),
            new(2, 0, isBlocked: false, false, true)
        };
        var group = new CropGroup(tiles);
        var centroid = group.Centroid;

        // Should not be the blocked tile
        Assert.NotEqual(new Point(1, 0), centroid);
        // Should be one of the walkable tiles
        Assert.True(centroid == new Point(0, 0) || centroid == new Point(2, 0));
    }

    [Fact]
    public void Centroid_WithStandingCenter_UsesIt()
    {
        var center = new TileInfo(5, 5, false, false, false);
        var tiles = new List<TileInfo>
        {
            new(4, 5, false, false, true),
            new(5, 5, false, false, true),
            new(6, 5, false, false, true)
        };
        var group = new CropGroup(tiles, standingCenter: center);
        Assert.Equal(new Point(5, 5), group.Centroid);
    }

    [Fact]
    public void Tiles_ReturnsAllTiles()
    {
        var tiles = new List<TileInfo> { new(1, 1, false, false, true), new(2, 2, false, false, true) };
        var group = new CropGroup(tiles);
        Assert.Equal(2, group.Tiles.Count);
    }
}
