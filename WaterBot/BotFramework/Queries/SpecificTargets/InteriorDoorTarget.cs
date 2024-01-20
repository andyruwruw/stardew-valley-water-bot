using BotFramework.Enums;

namespace BotFramework.Queries
{
    public abstract class InteriorDoorTarget : Target
    {
        public InteriorDoorTarget() : base(TargetType.InteriorDoor)
        {
        }
    }
}
