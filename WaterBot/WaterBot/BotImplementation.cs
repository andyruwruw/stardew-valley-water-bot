using System.Collections.Generic;
using BotFramework.Behaviors;
using BotFramework.Bots;
using WaterBot.Behaviors;
using WaterBot.Config;


namespace WaterBot
{
    /// <summary>
    /// WaterBot implementation.
    /// </summary>
    internal class BotImplementation : Bot
    {
        /// <inheritdoc cref="Bot.GetKey"/>
        public static string Key = "andyruwruw-waterbot";

        /// <inheritdoc cref="Bot.GetKey"/>
        public override string GetKey()
        {
            return BotImplementation.Key;
        }

        /// <inheritdoc cref="Bot.GetName"/>
        public override string GetName()
        {
            return WaterBotStringResources.GetBotName();
        }

        /// <inheritdoc cref="Bot.GetDescription"/>
        public override string GetDescription()
        {
            return WaterBotStringResources.GetBotDescription();
        }

        /// <inheritdoc cref="Bot.IniticalStates"/>
        protected override IDictionary<string, IState> IniticalStates()
        {
            return base.IniticalStates(
                new IdleState(),
                new FinishedTasksState(),
                new EmptyWateringCanState(),
                new WateringState()
            );
        }

        /// <inheritdoc cref="Bot.InitialDefaultState"/>
        protected override IState InitialDefaultState()
        {
            return this._states[IdleState.Key];
        }
    }
}
