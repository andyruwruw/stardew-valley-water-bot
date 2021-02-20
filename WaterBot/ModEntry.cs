using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using WaterBot.Framework.Tools;

namespace WaterBot
{
    /// <summary>The mod entry point.</summary>
    public class WaterBot : Mod
    {
        public bool waterBotOn = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
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

            if (this.waterBotOn)
            {
                Game1.addHUDMessage(new HUDMessage("Tired of Watering", 1));
                this.waterBotOn = false;
            }

            if (e.Button == SButton.MouseLeft && Game1.currentLocation is Farm && Game1.player.CurrentItem is WateringCan)
            {
                Vector2 toolLocation = Game1.player.GetToolLocation(false);
                List<Vector2> source = ((UnprotectedWateringCan)Game1.player.CurrentItem).useTilesAffected(new Vector2((float)(toolLocation.X / Game1.tileSize), (float)(toolLocation.Y / Game1.tileSize)), 1, Game1.player);

                Game1.addHUDMessage(new HUDMessage("Watering Plants", 1));
                this.Monitor.Log($"{Game1.player.GetToolLocation(true)}", LogLevel.Debug);
                this.waterBotOn = true;
            }
        }
    }
}