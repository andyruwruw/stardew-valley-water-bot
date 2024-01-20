using BotFramework.Enums;
using Microsoft.Xna.Framework;
using StardewValley;

namespace BotFramework.Behaviors
{
    /// <summary>
    /// A wrapper and shared API for various target types once queried.
    /// </summary>
    public interface IActionable
    {
        /// <summary>
        /// The type of target.
        /// </summary>
        /// <returns><see cref="TargetType"/> of object.</returns>
        TargetType GetType();

        /// <summary>
        /// Resolves the target into it's game location.
        /// </summary>
        /// <returns><see cref="GameLocation"/> the target is in.</returns>
        GameLocation GetGameLocation();

        /// <summary>
        /// Resolves the target into a tile position.
        /// </summary>
        /// <returns><see cref="Vector2"/> of target position.</returns>
        Vector2 GetTile();
    }
}
