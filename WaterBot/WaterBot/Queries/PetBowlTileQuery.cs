using BotFramework.Queries;

namespace WaterBot.Queries
{
    public class PetBowlTileQuery : Query
    {
        /// <inheritdoc cref="Query.GetTarget"/>
        public override ITarget GetTarget()
        {
            return new PetBowlTileTarget();
        }
    }
}
