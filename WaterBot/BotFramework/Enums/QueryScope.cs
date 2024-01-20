namespace BotFramework.Enums
{
    /// <summary>
    /// Where should the bot look for targets?
    /// </summary>
    public enum QueryScope
    {
        /// <summary>
        /// Just this current location.
        /// </summary>
        CurrentLocation = 0,

        /// <summary>
        /// Go everywhere you can walk.
        /// </summary>
        FootTrafficable = 1,

        /// <summary>
        /// All areas on the Farm. (Greenhouse, Home, Cave)
        /// </summary>
        FarmLocations = 2,
    }
}
