namespace BotFramework.Enums
{
    /// <summary>
    /// Selectors applied after targets are found.
    /// </summary>
    /// <remarks>
    /// Applied after targets are found, replacing the targets with spacially related targets.
    /// </remarks>
    public enum PostQuerySelector
    {
        /// <summary>
        /// Tile of or under instaces of the target
        /// </summary>
        TileOf,

        /// <summary>
        /// Tiles around instances of the target, not including other instances. Only applies to TileTarget.
        /// </summary>
        SurroundingTiles,

        /// <summary>
        /// Tiles surrounded by instances of the target, not including other instances. Only applies to TileTarget.
        /// </summary>
        EnclosedTiles,

        /// <summary>
        /// Tile north (-Y) of instaces of the target.
        /// </summary>
        North,

        /// <summary>
        /// Tile east (-X) of instaces of the target.
        /// </summary>
        East,

        /// <summary>
        /// Tile south (+Y) of instaces of the target.
        /// </summary>
        South,

        /// <summary>
        /// Tile west (+X) of instaces of the target.
        /// </summary>
        West,

        /// <summary>
        /// Tile north-east (-Y, -X) of instaces of the target.
        /// </summary>
        NorthEast,

        /// <summary>
        /// Tile north-west (-Y, +X) of instaces of the target.
        /// </summary>
        NorthWest,

        /// <summary>
        /// Tiles north-east (+Y, -X) of instaces of the target.
        /// </summary>
        SouthEast,

        /// <summary>
        /// Tiles north-east (+Y, +X) of instaces of the target.
        /// </summary>
        SouthWest,
    }
}
