using BotFramework.Enums;

namespace BotFramework.Queries
{
    public abstract class ObjectTarget : Target
    {
        public ObjectTarget() : base(TargetType.Object)
        {
        }
    }
}
