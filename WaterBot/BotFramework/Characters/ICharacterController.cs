using StardewValley;
using System.Collections.Generic;

namespace BotFramework.Characters
{
    public interface ICharacterController
    {
        /// <summary>
        /// Retrieves the controlled <see cref="Character"/>.
        /// </summary>
        /// <returns>The controlled <see cref="Character"/></returns>
        Character GetCharacter();

        /// <summary>
        /// Retrieves the <see cref="Character">Character's</see> <see cref="GameLocation"/>.
        /// </summary>
        /// <returns></returns>
        GameLocation GetCurrentLocation();

        /// <summary>
        /// Whether this character is the farmer.
        /// </summary>
        bool IsFarmer();

        /// <summary>
        /// Retrieves the character's inventory.
        /// </summary>
        /// <returns>The character's inventory, or null if not a farmer.</returns>
        IList<Item> GetInventory();

        /// <summary>
        /// Retrieves the character's active item.
        /// </summary>
        /// <returns>Character active <see cref="Item"/>.</returns>
        Item GetActiveItem();

        /// <summary>
        /// Switches the character to hold a given item.
        /// </summary>
        /// <param name="item"><see cref="Item"/> to switch to.</param>
        void SwitchActiveItem(Item item);
    }
}
