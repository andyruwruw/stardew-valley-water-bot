using Microsoft.Xna.Framework;
using WaterBot.Framework;
using WaterBot.Framework.Models;
using Xunit;

namespace WaterBot.Tests;

public class CropGrouperTests
{
    #region GroupByAdjacency

    [Fact]
    public void Adjacency_SingleCrop_OneGroupOfOne()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', '.' },
            { '.', 'C', '.' },
            { '.', '.', '.' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);

        Assert.Single(groups);
        Assert.Single(groups[0].Tiles);
    }

    [Fact]
    public void Adjacency_TwoByTwoBlock_OneGroupOfFour()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { 'C', 'C' },
            { 'C', 'C' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);

        Assert.Single(groups);
        Assert.Equal(4, groups[0].Tiles.Count);
    }

    [Fact]
    public void Adjacency_TwoSeparateCrops_TwoGroups()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { 'C', '.', '.', '.', 'C' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);

        Assert.Equal(2, groups.Count);
        Assert.Single(groups[0].Tiles);
        Assert.Single(groups[1].Tiles);
    }

    [Fact]
    public void Adjacency_LShape_OneGroup()
    {
        // L-shape requires checking all 4 directions
        var grid = new MockTileGrid(new char[,]
        {
            { 'C', '.', '.' },
            { 'C', '.', '.' },
            { 'C', 'C', 'C' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);

        Assert.Single(groups);
        Assert.Equal(5, groups[0].Tiles.Count);
    }

    [Fact]
    public void Adjacency_DiagonalOnly_SeparateGroups()
    {
        // Diagonal crops are NOT connected in 4-connected adjacency
        var grid = new MockTileGrid(new char[,]
        {
            { 'C', '.', '.' },
            { '.', 'C', '.' },
            { '.', '.', 'C' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);

        Assert.Equal(3, groups.Count);
    }

    [Fact]
    public void Adjacency_VerticalLine_OneGroup()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { 'C' },
            { 'C' },
            { 'C' },
            { 'C' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);

        Assert.Single(groups);
        Assert.Equal(4, groups[0].Tiles.Count);
    }

    [Fact]
    public void Adjacency_NoCrops_EmptyResult()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.' },
            { '.', '.' }
        });

        var groups = CropGrouper.GroupByAdjacency(grid.CropTiles, grid);

        Assert.Empty(groups);
    }

    #endregion

    #region GroupByMinimalCover

    [Fact]
    public void MinimalCover_ThreeByThreeBlock_OneGroup()
    {
        // A single center tile can water all 8 neighbors
        var grid = new MockTileGrid(new char[,]
        {
            { 'C', 'C', 'C' },
            { 'C', 'C', 'C' },
            { 'C', 'C', 'C' }
        });

        var groups = CropGrouper.GroupByMinimalCover(grid.CropTiles, grid);

        Assert.Single(groups);
        Assert.Equal(9, groups[0].Tiles.Count);
    }

    [Fact]
    public void MinimalCover_AllCropsCovered()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { 'C', '.', 'C' },
            { '.', 'C', '.' },
            { 'C', '.', 'C' }
        });

        var groups = CropGrouper.GroupByMinimalCover(grid.CropTiles, grid);

        var allCovered = groups.SelectMany(g => g.Tiles.Select(t => t.Position)).ToHashSet();
        foreach (var crop in grid.CropTiles)
            Assert.Contains(crop.Position, allCovered);
    }

    [Fact]
    public void MinimalCover_StandingPositionsAreWalkable()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', 'C', '.' },
            { 'C', '.', 'C' },
            { '.', 'C', '.' }
        });

        var groups = CropGrouper.GroupByMinimalCover(grid.CropTiles, grid);

        foreach (var group in groups)
        {
            if (group.StandingCenter is not null)
                Assert.False(group.StandingCenter.IsBlocked);
        }
    }

    #endregion
}
