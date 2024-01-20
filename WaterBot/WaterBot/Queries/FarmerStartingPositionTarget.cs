using BotFramework.Queries;
using StardewValley;
using System.Collections.Generic;

namespace WaterBot.Queries
{
    public class FarmerStartingPositionTarget : CharacterTarget
    {
        /// <inheritdoc cref="CharacterTarget.HasDirectValue"/>
        public override bool HasDirectValue()
        {
            return true;
        }

        /// <inheritdoc cref="CharacterTarget.DirectCharacterValue"/>
        public override IList<Character> DirectCharacterValue()
        {
            return base.DirectCharacterValue(Game1.player);
        }
    }
}
