using BotFramework.Queries;

namespace WaterBot.Queries
{
    /// <summary>
    /// Seeks all waterable tiles.
    /// </summary>
    public class WaterableTerrainFeatureQuery : Query
    {
        /// <inheritdoc cref="Query.GetTarget"/>
        public override ITarget GetTarget()
        {
            return new WaterableTerrainFeatureTarget();
        }
    }
}
