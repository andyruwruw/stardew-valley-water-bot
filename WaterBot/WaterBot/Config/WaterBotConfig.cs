using StardewModdingAPI.Utilities;
using BotFramework.Config;

namespace WaterBot.Config
{
    public class WaterBotConfig : BotFrameworkConfig
    {
        /// <summary>
        /// Static reference to configuration.
        /// </summary>
        new static public WaterBotConfig Config = new WaterBotConfig();

        /// <summary>
        /// Whether the process of watering should take energy.
        /// </summary>
        public bool WateringConsumesEnergy { get; set; }

        /// <summary>
        /// Whether to use water when watering.
        /// </summary>
        public bool WateringEmptiesWateringCan { get; set; }

        /// <summary>
        /// Whether to skip the watering and simply set all plants as watered.
        /// </summary>
        public bool WateringHappensInstantly { get; set; }

        /// <summary>
        /// Whether to use animation cancels.
        /// </summary>
        public bool UseAnimationCancels { get; set; }

        /// <summary>
        /// Whether to use upgraded watering cans.
        /// </summary>
        public bool UseUpgradedWateringCans { get; set; }

        /// <summary>
        /// Whether to stop before the farmer passes out.
        /// </summary>
        public bool StopBeforeExhaustion { get; set; }

        /// <summary>
        /// Whether to stop the bot given any key press.
        /// </summary>
        public bool StopOnAnyKeyPress { get; set; }

        /// <summary>
        /// What button starts the bot.
        /// </summary>
        public KeybindList OverrideStopKey { get; set; }

        /// <summary>
        /// Whether the user must be holding a watering can to start.
        /// </summary>
        public bool StartOnlyIfHoldingWateringCan { get; set; } = true;

        /// <summary>
        /// Whether the user must be using the toggle key on a waterable item.
        /// </summary>
        public bool StartAfterKeyPressOnWaterableTitle { get; set; } = true;

        /// <summary>
        /// Whether the user must be using the toggle key on a waterable item.
        /// </summary>
        public bool StartOnActionKey { get; set; } = true;

        /// <summary>
        /// What button starts the bot.
        /// </summary>
        public KeybindList OverrideStartKey { get; set; }

        /// <summary>
        /// Instantiates a new config.
        /// </summary>
        public WaterBotConfig() : base()
        {
            this.WateringConsumesEnergy = true;
            this.WateringEmptiesWateringCan = false;
            this.WateringHappensInstantly = false;
            this.UseAnimationCancels = false;
            this.UseUpgradedWateringCans = true;
            this.StopBeforeExhaustion = true;
            this.StopOnAnyKeyPress = true;
            this.OverrideStopKey = KeybindList.Parse("MouseLeft");
            this.StartOnlyIfHoldingWateringCan = true;
            this.StartAfterKeyPressOnWaterableTitle = true;
            this.StartOnActionKey = true;
            this.OverrideStartKey = KeybindList.Parse("MouseRight");
        }
    }
}
