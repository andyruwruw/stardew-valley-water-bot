using BotFramework.Enums;
using BotFramework.Queries;
using System.Collections.Generic;

namespace WaterBot.Queries
{
    public class FarmerStartingPositionQuery : Query
    {
        /// <inheritdoc cref="Query.GetTarget"/>
        public override ITarget GetTarget()
        {
            return new FarmerStartingPositionTarget();
        }

        /// <inheritdoc cref="IQuery.GetPostQuerySelectors"/>
        public override IList<PostQuerySelector> GetPostQuerySelectors()
        {
            return base.GetPostQuerySelectors(PostQuerySelector.TileOf);
        }
    }
}
