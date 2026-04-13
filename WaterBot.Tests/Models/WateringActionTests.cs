using Microsoft.Xna.Framework;
using WaterBot.Framework.Models;
using Xunit;

namespace WaterBot.Tests.Models;

public class WateringActionTests
{
    [Fact]
    public void TryDequeueTarget_ReturnsFifoOrder()
    {
        var action = new WateringAction(
            new Point(0, 0),
            new[] { new Point(1, 0), new Point(2, 0), new Point(3, 0) },
            WateringAction.ActionType.Water);

        Assert.Equal(new Point(1, 0), action.TryDequeueTarget());
        Assert.Equal(new Point(2, 0), action.TryDequeueTarget());
        Assert.Equal(new Point(3, 0), action.TryDequeueTarget());
    }

    [Fact]
    public void TryDequeueTarget_ReturnsNull_WhenEmpty()
    {
        var action = new WateringAction(new Point(0, 0), WateringAction.ActionType.Water);
        Assert.Null(action.TryDequeueTarget());
    }

    [Fact]
    public void IsComplete_TrueAfterAllDequeued()
    {
        var action = new WateringAction(
            new Point(0, 0),
            new[] { new Point(1, 0) },
            WateringAction.ActionType.Water);

        Assert.False(action.IsComplete);
        action.TryDequeueTarget();
        Assert.True(action.IsComplete);
    }

    [Fact]
    public void EnqueueTarget_AddsToEnd()
    {
        var action = new WateringAction(new Point(0, 0), WateringAction.ActionType.Water);
        action.EnqueueTarget(new Point(5, 5));
        action.EnqueueTarget(new Point(6, 6));

        Assert.Equal(new Point(5, 5), action.TryDequeueTarget());
        Assert.Equal(new Point(6, 6), action.TryDequeueTarget());
        Assert.Null(action.TryDequeueTarget());
    }

    [Fact]
    public void StandPosition_StoredCorrectly()
    {
        var action = new WateringAction(new Point(7, 3), WateringAction.ActionType.Refill);
        Assert.Equal(new Point(7, 3), action.StandPosition);
        Assert.Equal(WateringAction.ActionType.Refill, action.Type);
    }
}
