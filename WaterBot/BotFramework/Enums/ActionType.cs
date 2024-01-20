namespace BotFramework.Enums
{
    /// <summary>
    /// Types of <see cref="Action">Actions</see>.
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// Don't do anything to it.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the act key on it.
        /// </summary>
        Act = 1,

        /// <summary>
        /// Use the use tool key on it.
        /// </summary>
        UseTool = 2,

        /// <summary>
        /// Place an item down on it.
        /// </summary>
        Place = 3,

        /// <summary>
        /// Put an item in it.
        /// </summary>
        Put = 4,

        /// <summary>
        /// Murder it.
        /// </summary>
        Kill = 5,

        /// <summary>
        /// Don't get close to it.
        /// </summary>
        Avoid = 6,

        /// <summary>
        /// Run circles around it like a lunatic.
        /// </summary>
        Encircle = 7,
    }
}
