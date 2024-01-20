using BotFramework.Enums;
using BotFramework.Queries;

namespace WaterBot.Queries
{
    /// <summary>
    /// Seeks tiles to refill the watering can.
    /// </summary>
    public class RefillableWaterSourceQuery : Query
    {
        /// <inheritdoc cref="Query.GetTarget"/>
        public override ITarget GetTarget()
        {
            return new RefillableSourceTileTarget();
        }

        /// <inheritdoc cref="Query.GetQueryPriority"/>
        public override QueryLimit GetQueryLimit()
        {
            return QueryLimit.DoForClosest;
        }
    }
}
