using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBot.Framework
{
    class Player
    {
        public static Point getPosition()
        {
            return new Point(Game1.player.getTileX(), Game1.player.getTileY());
        }
    }
}
