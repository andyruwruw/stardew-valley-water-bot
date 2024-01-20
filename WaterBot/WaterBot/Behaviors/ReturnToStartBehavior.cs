using BotFramework.Behaviors;
using BotFramework.Queries;
using WaterBot.Queries;

namespace WaterBot.Behaviors
{
    /// <summary>
    /// Behavior to return to starting place.
    /// </summary>
    public class ReturnToStartBehavior : Behavior
    {
        /// <inheritdoc cref="Behavior.GetKey"/>
        public static string Key = "return-to-starting-position-behavior";

        /// <inheritdoc cref="Behavior.GetKey"/>
        public override string GetKey()
        {
            return ReturnToStartBehavior.Key;
        }

        /// <inheritdoc cref="Behavior.GetName"/>
        public override string GetName()
        {
            return "Return to Starting Position Behavior";
        }

        /// <inheritdoc cref="Behavior.GetQuery"/>
        public override IQuery GetQuery()
        {
            return new FarmerStartingPositionQuery();
        }
    }
}
