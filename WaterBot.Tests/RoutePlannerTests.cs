using Microsoft.Xna.Framework;
using WaterBot.Framework;
using WaterBot.Framework.Models;
using Xunit;

namespace WaterBot.Tests;

public class RoutePlannerTests
{
    #region OrderGroups

    [Fact]
    public void OrderGroups_SingleGroup_ReturnsIt()
    {
        var group = MakeGroup(new Point(5, 5));
        var result = RoutePlanner.OrderGroups(new List<CropGroup> { group }, new Point(0, 0));

        Assert.Single(result);
    }

    [Fact]
    public void OrderGroups_NearestGroupFirst()
    {
        var far = MakeGroup(new Point(10, 10));
        var near = MakeGroup(new Point(1, 1));
        var mid = MakeGroup(new Point(5, 5));

        var result = RoutePlanner.OrderGroups(
            new List<CropGroup> { far, near, mid },
            new Point(0, 0));

        Assert.Equal(new Point(1, 1), result[0].Centroid);
    }

    [Fact]
    public void OrderGroups_ThreeGroupsInLine_SpatialOrder()
    {
        var a = MakeGroup(new Point(2, 0));
        var b = MakeGroup(new Point(6, 0));
        var c = MakeGroup(new Point(10, 0));

        var result = RoutePlanner.OrderGroups(
            new List<CropGroup> { c, a, b },
            new Point(0, 0));

        Assert.Equal(new Point(2, 0), result[0].Centroid);
        Assert.Equal(new Point(6, 0), result[1].Centroid);
        Assert.Equal(new Point(10, 0), result[2].Centroid);
    }

    #endregion

    #region PlanWateringPath

    [Fact]
    public void PlanWateringPath_EveryCropInExactlyOneAction()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', 'C', 'C', '.' },
            { 'C', 'C', 'C', 'C' },
            { '.', 'C', 'C', '.' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);
        Assert.Single(groups);

        var actions = RoutePlanner.PlanWateringPath(groups[0], new Point(0, 0), grid);

        var allTargets = actions.SelectMany(a => a.Targets).ToList();
        var cropPositions = grid.CropTiles.Select(t => t.Position).ToHashSet();

        // Every crop should appear exactly once
        Assert.Equal(cropPositions.Count, allTargets.Count);
        Assert.Equal(cropPositions, allTargets.ToHashSet());
    }

    [Fact]
    public void PlanWateringPath_StandingPositionsNotBlocked()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', '.' },
            { '.', 'C', '.' },
            { '.', '.', '.' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);
        var actions = RoutePlanner.PlanWateringPath(groups[0], new Point(0, 0), grid);

        foreach (var action in actions)
        {
            var standTile = grid.GetOrQuery(action.StandPosition.X, action.StandPosition.Y);
            Assert.False(standTile.IsBlocked, $"Standing position ({action.StandPosition.X}, {action.StandPosition.Y}) is blocked");
        }
    }

    [Fact]
    public void PlanWateringPath_MinimalCoverGroup_SingleAction()
    {
        // A minimal cover group with a standing center should produce one action
        var center = new TileInfo(1, 1, false, false, false);
        var tiles = new List<TileInfo>
        {
            new(0, 0, false, false, true),
            new(1, 0, false, false, true),
            new(2, 0, false, false, true),
            new(1, 1, false, false, true)
        };
        var group = new CropGroup(tiles, standingCenter: center);

        var grid = new MockTileGrid(new char[,]
        {
            { 'C', 'C', 'C' },
            { '.', 'C', '.' }
        });

        var actions = RoutePlanner.PlanWateringPath(group, new Point(0, 0), grid);

        Assert.Single(actions);
        Assert.Equal(new Point(1, 1), actions[0].StandPosition);
        Assert.Equal(4, actions[0].Targets.Count);
    }

    [Fact]
    public void PlanWateringPath_BlockedCrop_StandsOnNeighbor()
    {
        // Crop on blocked tile — bot must stand on adjacent walkable tile
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', '.' },
            { '.', 'X', '.' },
            { '.', '.', '.' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);
        var actions = RoutePlanner.PlanWateringPath(groups[0], new Point(0, 0), grid);

        foreach (var action in actions)
        {
            var standTile = grid.GetOrQuery(action.StandPosition.X, action.StandPosition.Y);
            Assert.False(standTile.IsBlocked);
        }
    }

    #endregion

    private static CropGroup MakeGroup(Point centroid)
    {
        var tile = new TileInfo(centroid.X, centroid.Y, false, false, true);
        return new CropGroup(new List<TileInfo> { tile });
    }
}
