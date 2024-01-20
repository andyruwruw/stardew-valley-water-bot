namespace BotFramework.Enums
{
    /// <summary>
    /// Types of <see cref="Behaviors.Behavior">Behaviors</see>.
    /// </summary>
    public enum BehaviorType
    {
        /// <summary>
        /// Default action depending on conditions.
        /// </summary>
        Task = 0,

        /// <summary>
        /// Condition constantly checked to determine whether to trigger behavior.
        /// </summary>
        Reaction = 1,

        /// <summary>
        /// Navigate to a target.
        /// </summary>
        Navigate = 2,
    }
}
