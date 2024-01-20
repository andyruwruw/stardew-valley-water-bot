namespace BotFramework.Enums
{
    /// <summary>
    /// Types of <see cref="Action">Actions</see>.
    /// </summary>
    public enum CallOrder
    {
        /// <summary>
        /// Targets are dequeued on state change.
        /// </summary>
        BeforeAll,

        /// <summary>
        /// Targets are dequeued prior to other target's execution.
        /// </summary>
        BeforeEach,

        /// <summary>
        /// Target enhances other targets. This means checks will be made on-route or during tasks and executed if within range.
        /// </summary>
        AdjacencyTrigger,

        /// <summary>
        /// Targets enhances other targets.
        /// </summary>
        Simultaneous,

        /// <summary>
        /// Targets are dequeued after other target's execution.
        /// </summary>
        AfterEach,

        /// <summary>
        /// Targets are dequeued after all other tasks.
        /// </summary>
        AfterAll,
    }
}
