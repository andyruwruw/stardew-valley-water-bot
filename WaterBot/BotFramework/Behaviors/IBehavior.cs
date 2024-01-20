using BotFramework.Characters;
using BotFramework.Enums;
using BotFramework.Queries;
using StardewValley;
using System.Collections.Generic;

namespace BotFramework.Behaviors
{
    /// <summary>
    /// Behaviors of the bot given a state.
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// Retrieves the key of the state.
        /// </summary>
        /// <returns>Key of the state.</returns>
        string GetKey();

        /// <summary>
        /// Retrieves the name of the state.
        /// </summary>
        /// <returns>Name of the state.</returns>
        string GetName();

        /// <summary>
        /// Designated query for this behavior. Targets to act on.
        /// </summary>
        /// <returns><see cref="IQuery"/> to find targets.</returns>
        IQuery GetQuery();

        /// <summary>
        /// Call order of behavior in state compared to other behaviors.
        /// </summary>
        /// <returns><see cref="CallOrder"/> of behavior.</returns>
        CallOrder GetCallOrder();

        /// <summary>
        /// What kind of act this behavior takes on targets.
        /// </summary>
        /// <returns>Behavior type for targets.</returns>
        BehaviorType GetBehaviorType();

        /// <summary>
        /// What kind of action to take on the target.
        /// </summary>
        /// <returns>Action type.</returns>
        ActionType GetActionType();

        /// <summary>
        /// Select an item from the user's inventory to use for behavior.
        /// </summary>
        /// <param name="character">Character doing the action.</param>
        /// <returns><see cref="Item"/> to use.</returns>
        Item PickInventoryItem(CharacterController character);
    }
}
