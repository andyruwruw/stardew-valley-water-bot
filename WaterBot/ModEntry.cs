using StardewModdingAPI;
using BotFramework;
using BotFramework.Bots;
using BotFramework.Config;
using BotFramework.Helpers;
using WaterBot.Config;

namespace WaterBot
{
    /// <summary>
    /// The mod entry point.
    /// </summary>
    public class WaterBot : Mod
    {
        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.SetStaticReferences(helper);
            this.SetEventListeners(helper);

            // Attatches to manager.
            BotImplementation waterBot = new BotImplementation();
            BotManager.Attatch(waterBot);
        }

        /// <summary>
        /// Sets all required static references from <see cref="IModHelper"/>.
        /// </summary>
        /// <param name="helper"><see cref="IModHelper"/> from <see cref="Entry(IModHelper)"/>.</param>
        private void SetStaticReferences(IModHelper helper)
        {
            // Read config file.
            WaterBotConfig.Config = Helper.ReadConfig<WaterBotConfig>();
            // Set helper references.
            UserInput.Helper = Helper.Input;
            StringResources.Helper = Helper.Translation;
            // Set static reference to monitor for logging.
            Logger.SetMonitor(Monitor);
        }

        /// <summary>
        /// Sets all required event listeners from <see cref="IModHelper"/>.
        /// </summary>
        /// <param name="helper"><see cref="IModHelper"/> from <see cref="Entry(IModHelper)"/>.</param>
        private void SetEventListeners(IModHelper helper)
        {
            // Add event listeners.
            helper.Events.GameLoop.UpdateTicked += BotManager.UpdateTicked;
            helper.Events.GameLoop.DayStarted += BotManager.DayStarted;
            helper.Events.Input.ButtonPressed += BotManager.ButtonPressed;
            helper.Events.Player.Warped += BotManager.Warped;
        }
    }
}