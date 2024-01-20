using BotFramework.Enums;

namespace BotFramework.Queries
{
    public abstract class ProjectileTarget : Target
    {
        public ProjectileTarget() : base(TargetType.Projectile)
        {
        }
    }
}
