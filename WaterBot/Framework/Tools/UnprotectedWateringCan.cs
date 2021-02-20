using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace WaterBot.Framework.Tools
{
    class UnprotectedWateringCan : StardewValley.Tools.WateringCan
    {
        public List<Vector2> useTilesAffected(Vector2 tileLocation, int power, Farmer who)
        {
            return this.tilesAffected(tileLocation, power, who);
        }
    }
}
