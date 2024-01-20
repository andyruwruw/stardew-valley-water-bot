namespace BotFramework.Enums
{
    /// <summary>
    /// How aware the Bot is of Stimuli.
    /// </summary>
    public enum BotAwareness
    {
        /// <summary>
        /// Bot is aware of all <see cref="Target">Targets</see> anywhere.
        /// </summary>
        Omniscient = 0,

        /// <summary>
        /// Bot is aware of any <see cref="Target">Targets</see> in the current <see cref="GameLocation"/>.
        /// </summary>
        Location = 1,

        /// <summary>
        /// Bot is aware of any <see cref="Target">Targets</see> in the <see cref="Farmer">Farmer's</see> POV.
        /// </summary>
        Spectator = 2,

        /// <summary>
        /// Bot is aware of any <see cref="Target">Targets</see> in a set radius around the in the <see cref="Farmer">Farmer</see>.
        /// </summary>
        Perimeter = 3,
    }
}
