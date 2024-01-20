using StardewValley;
using BotFramework.Behaviors;
using WaterBot.Config;

namespace WaterBot.Behaviors
{
    /// <summary>
    /// Awaiting input state.
    /// </summary>
    internal class IdleState : State
    {
        /// <inheritdoc cref="State.GetKey"/>
        public static string Key = "waterbot-idle";

        /// <inheritdoc cref="State.GetKey"/>
        public override string GetKey()
        {
            return Key;
        }


        /// <inheritdoc cref="State.GetName"/>
        public override string GetName()
        {
            return WaterBotStringResources.GetStateName(IdleState.Key);
        }

        /// <inheritdoc cref="IState.ShouldCheckAfterAction"/>
        public override bool ShouldCheckAfterAction(IState active)
        {
            return true;
        }

        /// <inheritdoc cref="IState.CheckAfterAction"/>
        public override int CheckAfterAction(IState active)
        {
            // If the user is about to run out of energy.
            if (active.GetKey() == WateringState.Key
                && Game1.player.Stamina <= 2f)
                return 1;

            return 0;
        }

        ///// <inheritdoc cref="State.ShouldCheckOnButtonPress"/>
        //public override bool ShouldCheckOnButtonPress(IState active)
        //{
        //    return active == this;
        //}

        ///// <inheritdoc cref="State.CheckButtonPressed"/>
        //public override int CheckButtonPressed(
        //    IState active,
        //    object sender,
        //    ButtonPressedEventArgs args
        //)
        //{
        //    return 0;
        //}
    }
}
