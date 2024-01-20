using BotFramework.Behaviors;
using BotFramework.Characters;
using BotFramework.Queries;
using StardewValley.Tools;
using StardewValley;
using System.Collections.Generic;
using WaterBot.Queries;

namespace WaterBot.Behaviors
{
    /// <summary>
    /// Seeks to water all waterable crops behavior.
    /// </summary>
    internal class WaterCropsBehavior : Behavior
    {
        /// <inheritdoc cref="Behavior.GetKey"/>
        public static string Key = "watering-crops-behavior";

        /// <inheritdoc cref="Behavior.GetKey"/>
        public override string GetKey()
        {
            return WaterCropsBehavior.Key;
        }

        /// <inheritdoc cref="Behavior.GetName"/>
        public override string GetName()
        {
            return "Watering Crops Behavior";
        }

        /// <inheritdoc cref="Behavior.GetQuery"/>
        public override IQuery GetQuery()
        {
            return new WaterableTerrainFeatureQuery();
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
