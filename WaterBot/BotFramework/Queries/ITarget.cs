using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using BotFramework.Enums;

namespace BotFramework.Queries
{
    /// <summary>
    /// Used for selecting targets of <see cref="Behavior">Behaviors</see>
    /// </summary>
    public interface ITarget
    {
        /// <summary>
        /// Returns this <see cref="TargetType"/>.
        /// </summary>
        /// <returns><see cref="TargetType"/> of target.</returns>
        TargetType GetTargetType();

        /// <summary>
        /// Whether the value is already known.
        /// </summary>
        /// <returns>Whether the value is already known.</returns>
        bool HasDirectValue();

        /// <summary>
        /// Direct access to targeted <see cref="Building"/>.
        /// </summary>
        /// <returns>Building of target.</returns>
        IList<Building> DirectBuildingValue();

        /// <inheritdoc cref="DirectBuildingValue()"/>
        IList<Building> DirectBuildingValue(params Building[] buildings);

        /// <summary>
        /// Checks if an item matches the target.
        /// </summary>
        /// <param name="potential">Potential match.</param>
        /// <returns>Whether this item is targetable.</returns>
        bool ValidateBuilding(Building potential);

        /// <summary>
        /// Direct access to targeted <see cref="Character"/>.
        /// </summary>
        /// <returns>Character of target.</returns>
        IList<Character> DirectCharacterValue();

        /// <inheritdoc cref="DirectCharacterValue()"/>
        IList<Character> DirectCharacterValue(params Character[] characters);

        /// <summary>
        /// Checks if an item matches the target.
        /// </summary>
        /// <param name="potential">Potential match.</param>
        /// <returns>Whether this item is targetable.</returns>
        bool ValidateCharacter(Character potential);

        /// <summary>
        /// Direct access to targeted <see cref="GameLocation"/>.
        /// </summary>
        /// <returns>GameLocation of target.</returns>
        IList<GameLocation> DirectGameLocationValue();

        /// <inheritdoc cref="DirectGameLocationValue()"/>
        IList<GameLocation> DirectGameLocationValue(params GameLocation[] gameLocations);

        /// <summary>
        /// Checks if an item matches the target.
        /// </summary>
        /// <param name="potential">Potential match.</param>
        /// <returns>Whether this item is targetable.</returns>
        bool ValidateGameLocation(GameLocation potential);

        /// <summary>
        /// Direct access to targeted <see cref="InteriorDoor"/>.
        /// </summary>
        /// <returns>InteriorDoor of target.</returns>
        IList<InteriorDoor> DirectInteriorDoorValue();

        /// <inheritdoc cref="DirectInteriorDoorValue()"/>
        IList<InteriorDoor> DirectInteriorDoorValue(params InteriorDoor[] interiorDoors);

        /// <summary>
        /// Checks if an item matches the target.
        /// </summary>
        /// <param name="potential">Potential match.</param>
        /// <returns>Whether this item is targetable.</returns>
        bool ValidateInteriorDoor(InteriorDoor potential);

        /// <summary>
        /// Direct access to targeted <see cref="Object"/>.
        /// </summary>
        /// <returns>Object of target.</returns>
        IList<StardewValley.Object> DirectObjectValue();

        /// <inheritdoc cref="DirectObjectValue()"/>
        IList<StardewValley.Object> DirectObjectValue(params StardewValley.Object[] stardewObjects);

        /// <summary>
        /// Checks if an item matches the target.
        /// </summary>
        /// <param name="potential">Potential match.</param>
        /// <returns>Whether this item is targetable.</returns>
        bool ValidateObject(StardewValley.Object potential);

        /// <summary>
        /// Direct access to targeted <see cref="Projectile"/>.
        /// </summary>
        /// <returns>Projectile of target.</returns>
        IList<Projectile> DirectProjectileValue();

        /// <inheritdoc cref="DirectProjectileValue()"/>
        IList<Projectile> DirectProjectileValue(params Projectile[] projectiles);

        /// <summary>
        /// Checks if an item matches the target.
        /// </summary>
        /// <param name="potential">Potential match.</param>
        /// <returns>Whether this item is targetable.</returns>
        bool ValidateProjectile(Projectile potential);

        /// <summary>
        /// Direct access to targeted <see cref="TerrainFeature"/>.
        /// </summary>
        /// <returns>TerrainFeature of target.</returns>
        IList<TerrainFeature> DirectTerrainFeatureValue();

        /// <inheritdoc cref="DirectTerrainFeatureValue()"/>
        IList<TerrainFeature> DirectTerrainFeatureValue(params TerrainFeature[] terrainFeatures);

        /// <summary>
        /// Checks if an item matches the target.
        /// </summary>
        /// <param name="potential">Potential match.</param>
        /// <returns>Whether this item is targetable.</returns>
        bool ValidateTerrainFeature(
            GameLocation location,
            TerrainFeature potential
        );

        /// <summary>
        /// Direct access to targeted <see cref="Tile"/>.
        /// </summary>
        /// <returns>Tile of target.</returns>
        IList<Vector2> DirectTileValue();

        /// <inheritdoc cref="DirectTileValue()"/>
        IList<Vector2> DirectTileValue(params Vector2[] tiles);

        /// <summary>
        /// Checks if an item matches the target.
        /// </summary>
        /// <param name="potential">Potential match.</param>
        /// <returns>Whether this item is targetable.</returns>
        bool ValidateTile(
            GameLocation location,
            Vector2 potential
        );

        /// <summary>
        /// Direct access to targeted <see cref="Warp"/>.
        /// </summary>
        /// <returns>Warp of target.</returns>
        IList<Warp> DirectWarpValue();

        /// <inheritdoc cref="DirectWarpValue()"/>
        IList<Warp> DirectWarpValue(params Warp[] warps);

        /// <summary>
        /// Checks if an item matches the target.
        /// </summary>
        /// <param name="potential">Potential match.</param>
        /// <returns>Whether this item is targetable.</returns>
        bool ValidateWarp(Warp potential);
    }
}
