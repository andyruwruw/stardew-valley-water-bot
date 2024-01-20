using StardewValley;
using StardewValley.Tools;
using StardewModdingAPI.Events;
using BotFramework.Behaviors;
using WaterBot.Config;
using System.Collections.Generic;

namespace WaterBot.Behaviors
{
    /// <summary>
    /// State when watering can is empty during watering.
    /// </summary>
    internal class EmptyWateringCanState : State
    {
        /// <inheritdoc cref="State.GetKey"/>
        public static string Key = "waterbot-empty-watering-can";

        /// <inheritdoc cref="State.GetKey"/>
        public override string GetKey()
        {
            return Key;
        }

        /// <inheritdoc cref="State.GetName"/>
        public override string GetName()
        {
            return WaterBotStringResources.GetStateName(EmptyWateringCanState.Key);
        }

        /// <inheritdoc cref="State.GetBehaviors"/>
        public override IList<IBehavior> GetBehaviors()
        {
            return base.GetBehaviors(new SeekRefillBehavior());
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
            // If we're actively looking for refilling.
            if (active == this)
            {
                // If we stop on any key press.
                if (WaterBotConfig.Config.StopOnAnyKeyPress)
                    return 0;

                // If the player pressed a specified stop key.
                if (WaterBotConfig.Config.OverrideStopKey.JustPressed())
                    return 0;

                // Key Watering.
                return 1;
            }

            return 0;
        }

        /// <inheritdoc cref="IState.ShouldCheckAfterAction"/>
        public override bool ShouldCheckAfterAction(IState active)
        {
            return true;
        }

        /// <inheritdoc cref="IState.CheckAfterAction"/>
        public override int CheckAfterAction(IState active)
        {
            // If the can is empty during watering.
            if (active.GetKey() == WateringState.Key
                && Game1.player.CurrentItem is WateringCan
                && (Game1.player.CurrentItem as WateringCan).WaterLeft <= 0)
                return 1;

            return 0;
        }
    }
}
