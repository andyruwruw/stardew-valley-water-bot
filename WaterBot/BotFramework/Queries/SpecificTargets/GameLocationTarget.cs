using BotFramework.Enums;

namespace BotFramework.Queries
{
    public abstract class GameLocationTarget : Target
    {
        public GameLocationTarget() : base(TargetType.GameLocation)
        {
        }
    }
}
