using BotFramework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using WaterBot.Framework;

namespace WaterBot
{
    /// <summary>
    /// The mod entry point.
    /// </summary>
    public class WaterBot : Mod
    {
        internal static Config? config;
        private WaterBotController? bot;

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// 
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            Logger.SetMonitor(Monitor);
            this.bot = new WaterBotController(helper);
            // Set static reference to monitor for logging.

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += (s, e) => SetUpConfigMenu();
        }

        /// <summary>
        /// Raised after the player presses a button on the keyboard, controller, or mouse.
        /// </summary>
        /// 
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (this.bot?.active == true)
            {
                Logger.Log("Player provided interrupt signal. Process stopped.");
                this.bot.stop();
            }
            else if (e.Button.IsActionButton()) // SButton.MouseRight 
            {
                if (this.isWateringHoedDirt())
                {
                    Logger.Log("Player provided trigger to begin bot.");
                    this.bot?.start(this.console);
                }
            }
        }

        /// <summary>
        /// Determines if the event was watering a tile of hoed dirt
        /// </summary>
        private bool isWateringHoedDirt()
        {
            // Is the player using a Watering Can on their Farm?
            if (Game1.player?.CurrentItem is WateringCan)
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
                    if (Game1.currentLocation?.terrainFeatures.ContainsKey(rounded) == true &&
                        Game1.currentLocation.terrainFeatures[rounded] is HoeDirt dirt &&
                        dirt.crop != null &&
                        ((dirt.crop.fullyGrown.Value &&
                        dirt.crop.dayOfCurrentPhase.Value > 0) || 
                            (dirt.crop.currentPhase.Value < dirt.crop.phaseDays.Count - 1)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Debug messages
        /// </summary>
        /// 
        /// <param name="message">Message text.</param>
        public void console(string message)
        {
            Logger.Log(message, LogLevel.Debug);
        }

        public class Config
        {
            public bool UseSmallGrouping { get; set; } = false;
            public bool RefillOnFinish { get; set; } = false;
            public int RefillIfLower { get; set; } = 95;
            public bool RedoPathOnRefill { get; set; } = false;
        }

        private void SetUpConfigMenu()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => config = new Config(),
                save: () => this.Helper.WriteConfig<Config>(config)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => this.Helper.Translation.Get("config.use_small_grouping.name"),
                tooltip: () => this.Helper.Translation.Get("config.use_small_grouping.desc"),
                getValue: () => config.UseSmallGrouping,
                setValue: (bool b) => config.UseSmallGrouping = b
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => this.Helper.Translation.Get("config.refill_on_finish.name"),
                tooltip: () => this.Helper.Translation.Get("config.refill_on_finish.desc"),
                getValue: () => config.RefillOnFinish,
                setValue: (bool b) => config.RefillOnFinish = b
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => this.Helper.Translation.Get("config.refill_if_lower.name"),
                tooltip: () => this.Helper.Translation.Get("config.refill_if_lower.desc"),
                getValue: () => config.RefillIfLower,
                setValue: (int b) => config.RefillIfLower = b,
                min: 0,
                max: 100
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => this.Helper.Translation.Get("config.redo_path_on_refill.name"),
                tooltip: () => this.Helper.Translation.Get("config.redo_path_on_refill.desc"),
                getValue: () => config.RedoPathOnRefill,
                setValue: (bool b) => config.RedoPathOnRefill = b
            );
        }
    }
}