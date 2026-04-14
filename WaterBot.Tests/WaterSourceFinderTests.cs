using Microsoft.Xna.Framework;
using WaterBot.Framework;
using WaterBot.Framework.Models;
using Xunit;

namespace WaterBot.Tests;

public class WaterSourceFinderTests
{
    [Fact]
    public void FindNearest_AdjacentWater_FoundImmediately()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '~' }
        });

        var result = WaterSourceFinder.FindNearest(new Point(0, 0), grid);

        Assert.NotNull(result);
        Assert.Equal(WateringAction.ActionType.Refill, result.Type);
    }

    [Fact]
    public void FindNearest_WaterThreeTilesAway()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', '.', '~' }
        });

        var result = WaterSourceFinder.FindNearest(new Point(0, 0), grid);

        Assert.NotNull(result);
        Assert.Contains(new Point(3, 0), result.Targets);
    }

    [Fact]
    public void FindNearest_BlockedWater_StandsOnLastWalkable()
    {
        // W = water but blocked (like a pond tile you can't walk on)
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', 'W' }
        });

        var result = WaterSourceFinder.FindNearest(new Point(0, 0), grid);

        Assert.NotNull(result);
        // Should stand on (1,0) — the last walkable tile before the blocked water
        Assert.Equal(new Point(1, 0), result.StandPosition);
    }

    [Fact]
    public void FindNearest_NoWater_ReturnsNull()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', '.' },
            { '.', '.', '.' }
        });

        var result = WaterSourceFinder.FindNearest(new Point(0, 0), grid);

        Assert.Null(result);
    }

    [Fact]
    public void FindNearest_WaterBehindWall_NotReachable()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '#', '~' }
        });

        // BFS can't reach water because wall blocks the only path
        // (only orthogonal expansion, and blocked tiles stop expansion)
        var result = WaterSourceFinder.FindNearest(new Point(0, 0), grid);

        // Water IS reachable because BFS expands to water tiles even if blocked
        // The water tile '~' is walkable, but '#' blocks. BFS won't expand through '#'.
        // However, water behind a wall in a 1-row grid means there's no path around.
        Assert.Null(result);
    }

    [Fact]
    public void FindNearest_WaterAroundWall_Reachable()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '#', '~' },
            { '.', '.', '.' }
        });

        var result = WaterSourceFinder.FindNearest(new Point(0, 0), grid);

        Assert.NotNull(result);
    }

}
