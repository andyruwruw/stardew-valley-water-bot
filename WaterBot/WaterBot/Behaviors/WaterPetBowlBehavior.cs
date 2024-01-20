using System.Collections.Generic;
using BotFramework.Behaviors;
using BotFramework.Characters;
using BotFramework.Queries;
using StardewValley;
using StardewValley.Tools;
using WaterBot.Queries;

namespace WaterBot.Behaviors
{
    /// <summary>
    /// Seeks to water pet bowl behavior.
    /// </summary>
    internal class WaterPetBowlBehavior : Behavior
    {
        /// <inheritdoc cref="Behavior.GetKey"/>
        public static string Key = "watering-pet-bowl-behavior";

        /// <inheritdoc cref="Behavior.GetKey"/>
        public override string GetKey()
        {
            return WaterPetBowlBehavior.Key;
        }

        /// <inheritdoc cref="Behavior.GetName"/>
        public override string GetName()
        {
            return "Watering Pet Bowl Behavior";
        }

        /// <inheritdoc cref="Behavior.GetQuery"/>
        public override IQuery GetQuery()
        {
            return new PetBowlTileQuery();
        }

        /// <inheritdoc cref="Behavior.PickInventoryItem"/>
        public override Item PickInventoryItem(CharacterController character)
        {
            IList<Item> inventory = character.GetInventory();
            Item bestItem = null;

            if (inventory != null)
            {
                foreach (Item item in inventory)
                {
                    if (bestItem == null)
                    {
                        bestItem = item;
                    }

                    if (item is WateringCan)
                    {
                        bestItem = item;
                        break;
                    }
                }
            }

            return bestItem;
        }
    }
}
