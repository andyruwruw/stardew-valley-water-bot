namespace BotFramework.Enums
{
    /// <summary>
	/// Types of <see cref="Target.Target">Target</see>.
	/// </summary>
	public enum TargetType
    {
        /// <summary>
        /// Target stimulus is a building.
        /// </summary>
        Building = 0,

        /// <summary>
        /// <para>Target stimulus is a <see cref="StardewValley.Character"/></para>
        /// <para><see cref="StardewValley.NPC"/>, <see cref="StardewValley.Characters.Pet"/>, <see cref="StardewValley.Monsters.Monster"/>, <see cref="StardewValley.Farmer"/> are all of type <see cref="StardewValley.Character"/>.</para>
        /// </summary>
        Character = 1,

        /// <summary>
        /// Target stimulus is a <see cref="StardewValley.GameLocation"/>.
        /// </summary>
        GameLocation = 2,

        /// <summary>
        /// Target stimulus is an <see cref="StardewValley.InteriorDoor"/>.
        /// </summary>
        InteriorDoor = 3,

        /// <summary>
        /// Target stimulus is an <see cref="StardewValley.Object"/>.
        /// </summary>
        Object = 4,

        /// <summary>
        /// Target stimulus is a <see cref="StardewValley.Projectiles.Projectile"/>.
        /// </summary>
        Projectile = 5,

        /// <summary>
        /// Target stimulus is a <see cref="StardewValley.TerrainFeatures.TerrainFeature"/>.
        /// </summary>
        TerrainFeature = 6,

        /// <summary>
        /// Target stimulus is a <see cref="xTile.Tiles.Tile"/>.
        /// </summary>
        Tile = 7,

        /// <summary>
        /// Target stimulus is a <see cref="StardewValley.Warp"/>.
        /// </summary>
        Warp = 8,

        /// <summary>
        /// Target stimulus is a custom condition.
        /// </summary>
        Condition = 9,
    }
}
