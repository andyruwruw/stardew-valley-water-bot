using BotFramework.Enums;

namespace BotFramework.Queries
{
    public abstract class CharacterTarget : Target
    {
        public CharacterTarget() : base(TargetType.Character)
        {
        }
    }
}
