using BotFramework.Behaviors;
using System.Collections.Generic;
using WaterBot.Config;

namespace WaterBot.Behaviors
{
    /// <summary>
    /// State concluding all tasks.
    /// </summary>
    internal class FinishedTasksState : State
    {
        /// <inheritdoc cref="State.GetKey"/>
        public static string Key = "waterbot-finished-state";

        /// <inheritdoc cref="State.GetKey"/>
        public override string GetKey()
        {
            return Key;
        }


        /// <inheritdoc cref="State.GetName"/>
        public override string GetName()
        {
            return WaterBotStringResources.GetStateName(FinishedTasksState.Key);
        }

        /// <inheritdoc cref="State.GetBehaviors"/>
        public override IList<IBehavior> GetBehaviors()
        {
            return base.GetBehaviors(new ReturnToStartBehavior());
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
