using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using BotFramework.Enums;

namespace BotFramework.Behaviors
{
    /// <inheritdoc cref="IActionable"/>
    public class Actionable : IActionable
    {
        /// <summary>
        /// Type of actionable item.
        /// </summary>
        private TargetType _type;

        /// <summary>
        /// The actionable item.
        /// </summary>
        private dynamic _item;

        /// <summary>
        /// The game location the actionable item is in.
        /// </summary>
        private GameLocation _location;

        /// <summary>
        /// Instantiates a new actionable item.
        /// </summary>
        /// <param name="type">Type of actionable item.</param>
        /// <param name="item">The item itself.</param>
        /// <param name="location">Location of the item if available.</param>
        public Actionable(
            TargetType type,
            dynamic item,
            GameLocation location = null
        ) {
            this._type = type;
            this._item = item;
            this._location = location;
        }

        /// <inheritdoc cref="IActionable.GetType"/>
        public virtual TargetType GetActionType()
        {
            return this._type;
        }

        /// <inheritdoc cref="IActionable.GetGameLocation"/>
        public virtual GameLocation GetGameLocation()
        {
            if (this._type == TargetType.Character)
            {
                return (this._item as Character).currentLocation;
            }
            if (this._type == TargetType.GameLocation)
            {
                return (this._item as GameLocation);
            }
            if (this._type == TargetType.InteriorDoor)
            {
                return (this._item as InteriorDoor).Location;
            }
            if (this._type == TargetType.TerrainFeature)
            {
                return (this._item as TerrainFeature).currentLocation;
            }
            // Building, Object, Projectile, Tile, Warp
            return this._location;
        }

        /// <inheritdoc cref="IActionable.GetTile"/>
        public virtual Vector2 GetTile()
        {
            if (this._type == TargetType.Building)
            {
                return new Vector2(
                    (this._item as Building).tileX.Value + (this._item as Building).humanDoor.X,
                    (this._item as Building).tileY.Value + (this._item as Building).humanDoor.Y
                );
            }
            if (this._type == TargetType.Character)
            {
                return (this._item as Character).getTileLocation();
            }
            if (this._type == TargetType.InteriorDoor)
            {
                return (this._item as InteriorDoor).Position.ToVector2();
            }
            if (this._type == TargetType.Object)
            {
                return (this._item as StardewValley.Object).TileLocation;
            }
            if (this._type == TargetType.Projectile)
            {
                Point center = (this._item as Projectile).getBoundingBox().Center;
                return new Vector2(
                    center.X,
                    center.Y
                );
            }
            if (this._type == TargetType.TerrainFeature)
            {
                return (this._item as TerrainFeature).currentTileLocation;
            }
            if (this._type == TargetType.Tile)
            {
                int x = this._item.X;
                int y = this._item.Y;

                return new Vector2(
                    x,
                    y
                );
            }
            if (this._type == TargetType.Warp)
            {
                return new Vector2(
                    (this._item as Warp).X,
                    (this._item as Warp).Y
                );
            }
            // GameLocation
            return new Vector2(
                -1,
                -1
            );
        }
    }
}
