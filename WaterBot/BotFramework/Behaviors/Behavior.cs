using BotFramework.Characters;
using BotFramework.Enums;
using BotFramework.Queries;
using StardewValley;

namespace BotFramework.Behaviors
{
    /// <inheritdoc cref="IBehavior"/>
    public abstract class Behavior : IBehavior
    {
        /// <summary>
        /// Instantiates a new behavior.
        /// </summary>
        public Behavior() {}

        /// <inheritdoc cref="IBehavior.GetKey"/>
        public abstract string GetKey();

        /// <inheritdoc cref="IBehavior.GetName"/>
        public abstract string GetName();

        /// <inheritdoc cref="IBehavior.GetQuery"/>
        public abstract IQuery GetQuery();

        /// <inheritdoc cref="IBehavior.GetCallOrder"/>
        public virtual CallOrder GetCallOrder()
        {
            return CallOrder.BeforeAll;
        }

        /// <inheritdoc cref="IBehavior.GetBehaviorType"/>
        public virtual BehaviorType GetBehaviorType()
        {
            return BehaviorType.Task;
        }

        /// <inheritdoc cref="IBehavior.GetActionType"/>
        public virtual ActionType GetActionType()
        {
            return ActionType.Act;
        }

        /// <inheritdoc cref="IBehavior.PickInventoryItem"/>
        public virtual Item PickInventoryItem(CharacterController character)
        {
            return character.GetActiveItem();
        }
    }
}
