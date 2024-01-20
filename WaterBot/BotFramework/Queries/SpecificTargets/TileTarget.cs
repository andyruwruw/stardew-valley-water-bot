using BotFramework.Enums;

namespace BotFramework.Queries
{
    public abstract class TileTarget : Target
    {
        public TileTarget() : base(TargetType.Tile)
        {
        }
    }
}
