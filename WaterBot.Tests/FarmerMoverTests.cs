using Microsoft.Xna.Framework;
using WaterBot.Framework;
using Xunit;

namespace WaterBot.Tests;

public class PathfinderTests
{
    [Fact]
    public void FindPath_DirectPath_ReturnsWaypoints()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', '.', '.' }
        });

        var path = Pathfinder.FindPath(new Point(0, 0), new Point(3, 0), grid);

        Assert.NotNull(path);
        Assert.Equal(3, path.Count); // 3 waypoints: (1,0), (2,0), (3,0) — excludes start
        Assert.Equal(new Point(1, 0), path.Dequeue());
        Assert.Equal(new Point(2, 0), path.Dequeue());
        Assert.Equal(new Point(3, 0), path.Dequeue());
    }

    [Fact]
    public void FindPath_AroundObstacle()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '#', '.' },
            { '.', '.', '.' }
        });

        var path = Pathfinder.FindPath(new Point(0, 0), new Point(2, 0), grid);

        Assert.NotNull(path);
        // Must go around: (0,0) → (0,1) → (1,1) → (2,1) → (2,0)
        var waypoints = path.ToList();
        Assert.Contains(new Point(2, 0), waypoints); // reaches destination
        Assert.DoesNotContain(new Point(1, 0), waypoints); // doesn't go through wall
    }

    [Fact]
    public void FindPath_Unreachable_ReturnsNull()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '#', '.' }
        });

        // Wall completely blocks the path in a 1-row grid
        var path = Pathfinder.FindPath(new Point(0, 0), new Point(2, 0), grid);

        Assert.Null(path);
    }

    [Fact]
    public void FindPath_AlreadyAtTarget_ReturnsEmptyPath()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.' }
        });

        var path = Pathfinder.FindPath(new Point(0, 0), new Point(0, 0), grid);

        Assert.NotNull(path);
        Assert.Empty(path);
    }

    [Fact]
    public void FindPath_AdjacentTarget_SingleWaypoint()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.' }
        });

        var path = Pathfinder.FindPath(new Point(0, 0), new Point(1, 0), grid);

        Assert.NotNull(path);
        Assert.Single(path);
        Assert.Equal(new Point(1, 0), path.Dequeue());
    }

    [Fact]
    public void FindPath_LargeOpenGrid_FindsShortestPath()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', '.', '.', '.' },
            { '.', '.', '.', '.', '.' },
            { '.', '.', '.', '.', '.' },
            { '.', '.', '.', '.', '.' },
            { '.', '.', '.', '.', '.' }
        });

        var path = Pathfinder.FindPath(new Point(0, 0), new Point(4, 4), grid);

        Assert.NotNull(path);
        // Manhattan distance = 8, BFS finds shortest path
        Assert.Equal(8, path.Count);
    }

    [Fact]
    public void FindPath_MazeAroundWalls()
    {
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', '#', '.', '.' },
            { '#', '.', '#', '.', '.' },
            { '.', '.', '.', '.', '#' },
            { '.', '#', '#', '.', '.' },
            { '.', '.', '.', '.', '.' }
        });

        var path = Pathfinder.FindPath(new Point(0, 0), new Point(4, 4), grid);

        Assert.NotNull(path);
        // Verify all waypoints are walkable (not walls)
        foreach (var point in path)
        {
            var tile = grid.GetOrQuery(point.X, point.Y);
            Assert.False(tile.IsBlocked, $"Waypoint ({point.X},{point.Y}) is blocked");
        }
    }

    [Fact]
    public void FindPath_BlockedEndTile_StillReaches()
    {
        // End tile is blocked but BFS should still find path TO it
        // (farmer may need to stand adjacent to a blocked crop)
        var grid = new MockTileGrid(new char[,]
        {
            { '.', '.', '#' }
        });

        var path = Pathfinder.FindPath(new Point(0, 0), new Point(2, 0), grid);

        Assert.NotNull(path);
        Assert.Contains(new Point(2, 0), path);
    }
}
