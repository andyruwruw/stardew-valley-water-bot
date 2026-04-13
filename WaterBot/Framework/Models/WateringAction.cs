using Microsoft.Xna.Framework;

namespace WaterBot.Framework.Models;

/// <summary>
/// Represents a position to stand at and a queue of tile positions to water or refill from there.
/// </summary>
internal sealed class WateringAction
{
    /// <summary>The type of action to perform on target tiles.</summary>
    public enum ActionType
    {
        Water,
        Refill
    }

    /// <summary>Where the player should stand to perform the action.</summary>
    public Point StandPosition { get; }

    /// <summary>The type of action (water crops or refill can).</summary>
    public ActionType Type { get; }

    /// <summary>Remaining target tiles to act on.</summary>
    public Queue<Point> Targets { get; }

    /// <summary>Whether all targets have been processed.</summary>
    public bool IsComplete => Targets.Count == 0;

    public WateringAction(Point standPosition, ActionType type)
    {
        StandPosition = standPosition;
        Type = type;
        Targets = new Queue<Point>();
    }

    public WateringAction(Point standPosition, IEnumerable<Point> targets, ActionType type)
    {
        StandPosition = standPosition;
        Type = type;
        Targets = new Queue<Point>(targets);
    }

    /// <summary>
    /// Dequeue the next target tile, or return null if all targets are complete.
    /// </summary>
    public Point? TryDequeueTarget()
    {
        return Targets.Count > 0 ? Targets.Dequeue() : null;
    }

    /// <summary>Add a target tile to the end of the queue.</summary>
    public void EnqueueTarget(Point target)
    {
        Targets.Enqueue(target);
    }
}
