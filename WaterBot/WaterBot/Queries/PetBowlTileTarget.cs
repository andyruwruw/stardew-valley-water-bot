using StardewValley;
using Microsoft.Xna.Framework;
using BotFramework.Queries;
using System.Collections.Generic;

namespace WaterBot.Queries
{
    /// <summary>
    /// Targets the farm pet bowl if pet bowl is unwatered.
    /// </summary>
    public class PetBowlTileTarget : TileTarget
    {
        /// <inheritdoc cref="TileTarget.HasDirectValue"/>
        public override bool HasDirectValue()
        {
            return true;
        }

        /// <inheritdoc cref="TileTarget.DirectTileValue"/>
        public override IList<Vector2> DirectTileValue()
        {
            if (Game1.getFarm().petBowlWatered.Value)
            {
                return base.DirectTileValue();
            }

            return base.DirectTileValue(new Vector2(
                Game1.getFarm().petBowlPosition.X,
                Game1.getFarm().petBowlPosition.Y
            ));
        }
    }
}
