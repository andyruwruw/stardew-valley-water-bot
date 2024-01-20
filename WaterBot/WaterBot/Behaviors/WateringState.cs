using StardewValley;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using BotFramework.Behaviors;
using BotFramework.Helpers;
using WaterBot.Config;
using System.Collections.Generic;

namespace WaterBot.Behaviors
{
    /// <summary>
    /// State in which bot is watering crops.
    /// </summary>
    internal class WateringState : State
    {
        /// <inheritdoc cref="State.GetKey"/>
        public static string Key = "waterbot-watering-crops";

        /// <inheritdoc cref="State.GetKey"/>
        public override string GetKey()
        {
            return Key;
        }

        /// <inheritdoc cref="State.GetName"/>
        public override string GetName()
        {
            return WaterBotStringResources.GetStateName(WateringState.Key);
        }

        /// <inheritdoc cref="State.GetBehaviors"/>
        public override IList<IBehavior> GetBehaviors()
        {
            return base.GetBehaviors(
                new WaterCropsBehavior(),
                new WaterPetBowlBehavior()
            );
        }

        /// <inheritdoc cref="State.ShowsActivationMessage"/>
        public override bool ShowsActivationMessage()
        {
            return true;
        }

        /// <inheritdoc cref="State.GetActivationMessage"/>
        public override string GetActivationMessage()
        {
            return WaterBotStringResources.GetStateActivationMessage(WateringState.Key);
        }

        /// <inheritdoc cref="State.ShouldCheckOnButtonPress"/>
        public override bool ShouldCheckOnButtonPress(IState active)
        {
            return true;
        }

        /// <inheritdoc cref="State.CheckButtonPressed"/>
        public override int CheckButtonPressed(
            IState active,
            object sender,
            ButtonPressedEventArgs args
        )
        {
            // If we are not currently watering.
            if (active != this)
            {
                // If the player isn't holding a watering can.
                if (WaterBotConfig.Config.StartOnlyIfHoldingWateringCan
                    && !(Game1.player.CurrentItem is WateringCan))
                    return 0;

                // If the player didn't press the action key.
                if (WaterBotConfig.Config.StartOnActionKey
                    && !args.Button.IsActionButton())
                    return 0;

                // If the player didn't press another specified key.
                if (!WaterBotConfig.Config.StartOnActionKey
                    && !WaterBotConfig.Config.OverrideStartKey.JustPressed())
                    return 0;

                // If the tile must be waterable.
                if (WaterBotConfig.Config.StartAfterKeyPressOnWaterableTitle)
                {
                    UserInput input = new UserInput(args);
                    TerrainFeature feature = input.GetCursorTerrainFeatureAtPress();

                    // Feature is present.
                    if (feature != null)
                        return 0;

                    // Contains a crop that needs to be watered.
                    if (feature is HoeDirt
                        && (feature as HoeDirt).crop != null
                        && (feature as HoeDirt).crop.fullyGrown.Value
                        && (feature as HoeDirt).crop.dayOfCurrentPhase.Value > 0
                        || (feature as HoeDirt).crop.currentPhase.Value < (feature as HoeDirt).crop.phaseDays.Count - 1)
                        return 1;
                }

                // Start Watering.
                return 1;
            }

            // If we stop on any key press.
            if (WaterBotConfig.Config.StopOnAnyKeyPress)
                return 0;

            // If the player pressed a specified stop key.
            if (WaterBotConfig.Config.OverrideStopKey.JustPressed())
                return 0;

            // Key Watering.
            return 1;
        }

        /// <inheritdoc cref="IState.ShouldCheckAfterAction"/>
        public override bool ShouldCheckAfterAction(IState active)
        {
            return true;
        }

        /// <inheritdoc cref="IState.CheckAfterAction"/>
        public override int CheckAfterAction(IState active)
        {
            return 0;
        }
    }
}
