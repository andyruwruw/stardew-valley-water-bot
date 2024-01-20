using BotFramework.Enums;

namespace BotFramework.Queries
{
    public abstract class WarpTarget : Target
    {
        public WarpTarget() : base(TargetType.Warp)
        {
        }
    }
}
