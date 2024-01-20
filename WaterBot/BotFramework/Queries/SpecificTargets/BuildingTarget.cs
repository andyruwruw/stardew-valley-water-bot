using BotFramework.Enums;

namespace BotFramework.Queries
{
    public abstract class BuildingTarget : Target
    {
        public BuildingTarget() : base(TargetType.Building)
        {
        }
    }
}
