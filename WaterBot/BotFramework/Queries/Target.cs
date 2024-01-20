using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using BotFramework.Enums;

namespace BotFramework.Queries
{
    /// <inheritdoc cref="ITarget"/>
    public abstract class Target : ITarget
    {
        /// <summary>
        /// Type of target.
        /// </summary>
        protected TargetType _type;

        /// <summary>
        /// Instantiates a new target type.
        /// </summary>
        /// <param name="type">Type of target.</param>
        public Target(TargetType type)
        {
            this._type = type;
        }

        /// <inheritdoc cref="ITarget.GetType"/>
        public TargetType GetTargetType()
        {
            return this._type;
        }

        /// <inheritdoc cref="ITarget.HasDirectValue"/>
        public virtual bool HasDirectValue()
        {
            return false;
        }

        /// <inheritdoc cref="ITarget.DirectBuildingValue"/>
        public virtual IList<Building> DirectBuildingValue()
        {
            return new List<Building>();
        }

        /// <inheritdoc cref="ITarget.DirectBuildingValue"/>
        public virtual IList<Building> DirectBuildingValue(params Building[] buildings)
        {
            IList<Building> list = new List<Building>();

            foreach (Building building in buildings)
            {
                list.Add(building);
            }

            return list;
        }

        /// <inheritdoc cref="ITarget.ValidateBuilding"/>
        public virtual bool ValidateBuilding(Building potential)
        {
            return false;
        }

        /// <inheritdoc cref="ITarget.DirectBuildingValue"/>
        public virtual IList<Character> DirectCharacterValue()
        {
            return new List<Character>();
        }

        /// <inheritdoc cref="ITarget.DirectCharacterValue"/>
        public virtual IList<Character> DirectCharacterValue(params Character[] characters)
        {
            IList<Character> list = new List<Character>();

            foreach (Character character in characters)
            {
                list.Add(character);
            }

            return list;
        }

        /// <inheritdoc cref="ITarget.ValidateCharacter"/>
        public virtual bool ValidateCharacter(Character potential)
        {
            return false;
        }

        /// <inheritdoc cref="ITarget.DirectGameLocationValue"/>
        public virtual IList<GameLocation> DirectGameLocationValue()
        {
            return new List<GameLocation>();
        }

        /// <inheritdoc cref="ITarget.DirectGameLocationValue"/>
        public virtual IList<GameLocation> DirectGameLocationValue(params GameLocation[] gameLocations)
        {
            IList<GameLocation> list = new List<GameLocation>();

            foreach (GameLocation gameLocation in gameLocations)
            {
                list.Add(gameLocation);
            }

            return list;
        }

        /// <inheritdoc cref="ITarget.ValidateGameLocation"/>
        public virtual bool ValidateGameLocation(GameLocation potential)
        {
            return false;
        }

        /// <inheritdoc cref="ITarget.DirectInteriorDoorValue"/>
        public virtual IList<InteriorDoor> DirectInteriorDoorValue()
        {
            return new List<InteriorDoor>();
        }

        /// <inheritdoc cref="ITarget.DirectInteriorDoorValue"/>
        public virtual IList<InteriorDoor> DirectInteriorDoorValue(params InteriorDoor[] interiorDoors)
        {
            IList<InteriorDoor> list = new List<InteriorDoor>();

            foreach (InteriorDoor interiorDoor in interiorDoors)
            {
                list.Add(interiorDoor);
            }

            return list;
        }

        /// <inheritdoc cref="ITarget.ValidateInteriorDoor"/>
        public virtual bool ValidateInteriorDoor(InteriorDoor potential)
        {
            return false;
        }

        /// <inheritdoc cref="ITarget.DirectObjectValue"/>
        public virtual IList<StardewValley.Object> DirectObjectValue()
        {
            return new List<StardewValley.Object>();
        }

        /// <inheritdoc cref="ITarget.DirectObjectValue"/>
        public virtual IList<StardewValley.Object> DirectObjectValue(params StardewValley.Object[] stardewObjects)
        {
            IList<StardewValley.Object> list = new List<StardewValley.Object>();

            foreach (StardewValley.Object stardewObject in stardewObjects)
            {
                list.Add(stardewObject);
            }

            return list;
        }

        /// <inheritdoc cref="ITarget.ValidateObject"/>
        public virtual bool ValidateObject(StardewValley.Object potential)
        {
            return false;
        }

        /// <inheritdoc cref="ITarget.DirectProjectileValue"/>
        public virtual IList<Projectile> DirectProjectileValue()
        {
            return new List<Projectile>();
        }

        /// <inheritdoc cref="ITarget.DirectProjectileValue"/>
        public virtual IList<Projectile> DirectProjectileValue(params Projectile[] projectiles)
        {
            IList<Projectile> list = new List<Projectile>();

            foreach (Projectile projectile in projectiles)
            {
                list.Add(projectile);
            }

            return list;
        }

        /// <inheritdoc cref="ITarget.ValidateProjectile"/>
        public virtual bool ValidateProjectile(Projectile potential)
        {
            return false;
        }

        /// <inheritdoc cref="ITarget.DirectTerrainFeatureValue"/>
        public virtual IList<TerrainFeature> DirectTerrainFeatureValue()
        {
            return new List<TerrainFeature>();
        }

        /// <inheritdoc cref="ITarget.DirectTerrainFeatureValue"/>
        public virtual IList<TerrainFeature> DirectTerrainFeatureValue(params TerrainFeature[] terrainFeatures)
        {
            IList<TerrainFeature> list = new List<TerrainFeature>();

            foreach (TerrainFeature terrainFeature in terrainFeatures)
            {
                list.Add(terrainFeature);
            }

            return list;
        }

        /// <inheritdoc cref="ITarget.ValidateTerrainFeature"/>
        public virtual bool ValidateTerrainFeature(
            GameLocation location,
            TerrainFeature potential
        ) {
            return false;
        }

        /// <inheritdoc cref="ITarget.DirectTileValue"/>
        public virtual IList<Vector2> DirectTileValue()
        {
            return new List<Vector2>();
        }

        /// <inheritdoc cref="ITarget.DirectTileValue"/>
        public virtual IList<Vector2> DirectTileValue(params Vector2[] tiles)
        {
            IList<Vector2> list = new List<Vector2>();

            foreach (Vector2 tile in tiles)
            {
                list.Add(tile);
            }

            return list;
        }

        /// <inheritdoc cref="ITarget.ValidateTile"/>
        public virtual bool ValidateTile(
            GameLocation location,
            Vector2 potential
        ) {
            return false;
        }

        /// <inheritdoc cref="ITarget.DirectWarpValue"/>
        public virtual IList<Warp> DirectWarpValue()
        {
            return new List<Warp>();
        }

        /// <inheritdoc cref="ITarget.DirectWarpValue"/>
        public virtual IList<Warp> DirectWarpValue(params Warp[] warps)
        {
            IList<Warp> list = new List<Warp>();

            foreach (Warp warp in warps)
            {
                list.Add(warp);
            }

            return list;
        }

        /// <inheritdoc cref="ITarget.ValidateWarp"/>
        public virtual bool ValidateWarp(Warp potential)
        {
            return false;
        }
    }
}
