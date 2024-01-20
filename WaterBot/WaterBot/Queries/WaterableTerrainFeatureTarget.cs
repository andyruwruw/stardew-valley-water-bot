using StardewValley;
using StardewValley.TerrainFeatures;
using BotFramework.Queries;

namespace WaterBot.Queries
{
    /// <summary>
    /// Targets all crops in need of watering.
    /// </summary>
    internal class WaterableTerrainFeatureTarget : TerrainFeatureTarget
    {
        /// <inheritdoc cref="ITarget.ValidateTerrainFeature"/>
        public override bool ValidateTerrainFeature(
            GameLocation location,
            TerrainFeature potential
        ) {
            return potential is HoeDirt
                && (potential as HoeDirt).crop != null
                && (((potential as HoeDirt).crop.fullyGrown.Value
                && (potential as HoeDirt).crop.dayOfCurrentPhase.Value > 0)
                || ((potential as HoeDirt).crop.currentPhase.Value < (potential as HoeDirt).crop.phaseDays.Count - 1));
        }
    }
}
