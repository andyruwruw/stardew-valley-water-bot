using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBot.Framework
{
    /// <summary>
    /// Defines the process of the Bot being active.
    /// </summary>
    class WaterBotControler
    {
        public bool active;

        public Map map;

        public PlayerControler controler;

        public WaterBotControler()
        {
            this.active = false;
            this.map = new Map();
            this.controler = new PlayerControler();
        }

        /// <summary>
        /// Starts the bot up.
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        public void start(console console)
        {
            this.active = true;

            this.displayMessage("Time to start watering!", 2);

            // Load map data
            this.map.loadMap();
            this.map.displayMap(console);

            console($"{Player.getPosition().X} {Player.getPosition().Y}");

            // Group waterable tiles
            this.map.findGroupings(console);

            List<Group> path = this.map.findGroupPath(console);
        }

        /// <summary>
        /// Cancels the bot's progress.
        /// </summary>
        public void stop()
        {
            this.active = false;

            this.displayMessage("Tired of watering", 3);
        }

        /// <summary>
        /// Displays banner message.
        /// </summary>
        /// 
        /// <param name="message">Banner text.</param>
        /// <param name="type">Banner type.</param>
        public void displayMessage(string message, int type)
        {
            Game1.addHUDMessage(new HUDMessage(message, type));
        }
    }
}
