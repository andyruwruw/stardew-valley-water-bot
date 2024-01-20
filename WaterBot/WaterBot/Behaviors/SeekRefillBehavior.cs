using BotFramework.Behaviors;
using BotFramework.Characters;
using BotFramework.Enums;
using BotFramework.Queries;
using StardewValley.Tools;
using StardewValley;
using System.Collections.Generic;
using WaterBot.Queries;

namespace WaterBot.Behaviors
{
    /// <summary>
    /// Seeks to refill watering can behavior.
    /// </summary>
    internal class SeekRefillBehavior : Behavior
    {
        /// <inheritdoc cref="Behavior.GetKey"/>
        public static string Key = "seek-refill-behavior";

        /// <inheritdoc cref="Behavior.GetKey"/>
        public override string GetKey()
        {
            return SeekRefillBehavior.Key;
        }

        /// <inheritdoc cref="Behavior.GetName"/>
        public override string GetName()
        {
            return "Seek Watering Can Refill Behavior";
        }

        /// <inheritdoc cref="Behavior.GetQuery"/>
        public override IQuery GetQuery()
        {
            return new RefillableWaterSourceQuery();
        }

        /// <inheritdoc cref="Behavior.GetActionType"/>
        public override ActionType GetActionType()
        {
            return ActionType.UseTool;
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
