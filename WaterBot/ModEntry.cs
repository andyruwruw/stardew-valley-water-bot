using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using WaterBot.Framework;

namespace WaterBot
{
    /// <summary>The mod entry point.</summary>
    public class WaterBot : Mod
    {
        public bool active = false;

        Map map;

        Player player;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.map = new Map();

            this.player = new Player();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (this.active)
            {
                this.stopBot();
            } else if (e.Button == SButton.MouseLeft)
            {
                if (this.isWateringHoedDirt())
                {
                    this.console("Starting");
                    this.startBot();
                }
            }
        }

        /// <summary>Determines if the event was watering a tile of hoed dirt</summary>
        private bool isWateringHoedDirt()
        {
            // Is the player using a Watering Can on their Farm?
            if (Game1.currentLocation is Farm && Game1.player.CurrentItem is WateringCan)
            {
                // Find action tiles
                Vector2 mousePosition = Utility.PointToVector2(Game1.getMousePosition()) + new Vector2(Game1.viewport.X, Game1.viewport.Y);
                Vector2 toolLocation = Game1.player.GetToolLocation(mousePosition);
                Vector2 tile = Utility.clampToTile(toolLocation);

                List<Vector2> tileLocations = this.Helper.Reflection
                    .GetMethod(Game1.player.CurrentItem, "tilesAffected")
                    .Invoke<List<Vector2>>(new Vector2(tile.X / 64, tile.Y / 64), 0, Game1.player);

                foreach (Vector2 tileLocation in tileLocations)
                {
                    Vector2 rounded = new Vector2((float)Math.Round(tileLocation.X), (float)Math.Round(tileLocation.Y));

                    // If they just watered Hoe Dirt, return true
                    if (Game1.currentLocation.terrainFeatures.ContainsKey(rounded) && Game1.currentLocation.terrainFeatures[rounded] is HoeDirt)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>Start water bot</summary>
        private void startBot()
        {
            this.displayMessage("Time to start watering!", 2);

            this.map.loadMap(Game1.currentLocation);
            this.map.displayMap(this.console);

            this.map.findGroupings(this.console);

            this.active = true;
            this.console("DONE");
        }

        /// <summary>Stop water bot</summary>
        private void stopBot()
        {
            this.displayMessage("Tired of watering", 3);
            this.active = false;
        }

        /// <summary>Displays banner message.</summary>
        /// <param name="message">Banner text.</param>
        /// <param name="type">Banner type.</param>
        public void displayMessage(string message, int type)
        {
            Game1.addHUDMessage(new HUDMessage(message, type));
        }

        /// <summary>Debug messages</summary>
        /// /// <param name="message">Message text.</param>
        public void console(string message)
        {
            this.Monitor.Log(message, LogLevel.Debug);
        }
    }
}