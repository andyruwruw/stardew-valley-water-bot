using System;
using StardewValley;
using Microsoft.Xna.Framework;
using BotFramework.Queries;

namespace WaterBot.Queries
{
    /// <summary>
    /// Targets tiles where the user can refill their watering can.
    /// </summary>
    public class RefillableSourceTileTarget : TileTarget
    {
        /// <inheritdoc cref="ITarget.ValidateTile"/>
        public override bool ValidateTile(
            GameLocation location,
            Vector2 potential
        ) {
            return location.CanRefillWateringCanOnTile(
                (int) Math.Round(potential.X),
                (int) Math.Round(potential.Y)
            );
        }
    }
}
