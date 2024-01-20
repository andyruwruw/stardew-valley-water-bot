using BotFramework.Enums;

namespace BotFramework.Queries
{
    public abstract class TerrainFeatureTarget : Target
    {
        public TerrainFeatureTarget() : base(TargetType.TerrainFeature)
        {
        }
    }
}
