using StardewValley;
using System.Collections.Generic;

namespace BotFramework.Characters
{
    public class CharacterController : ICharacterController
    {
        /// <inheritdoc cref="ICharacterController.GetCharacter"/>
        private Character _who;

        /// <summary>
        /// Instantiates a new <see cref="Character"/> controller.
        /// </summary>
        /// <param name="who">The <see cref="Character"/> to control.</param>
        public CharacterController(Character who)
        {
            this._who = who;
        }

        /// <inheritdoc cref="ICharacterController.GetCharacter"/>
        public virtual Character GetCharacter()
        {
            return this._who;
        }

        /// <inheritdoc cref="ICharacterController.GetCurrentLocation"/>
        public virtual GameLocation GetCurrentLocation()
        {
            return this._who.currentLocation;
        }

        /// <inheritdoc cref="ICharacterController.IsFarmer"/>
        public virtual bool IsFarmer()
        {
            return this._who is Farmer;
        }

        /// <inheritdoc cref="ICharacterController.GetInventory"/>
        public virtual IList<Item> GetInventory()
        {
            if (this.IsFarmer())
                return (this._who as Farmer).Items;
            return null;
        }

        /// <inheritdoc cref="ICharacterController.GetActiveItem"/>
        public virtual Item GetActiveItem()
        {
            if (this.IsFarmer())
                return (this._who as Farmer).ActiveObject;
            return null;
        }

        /// <inheritdoc cref="ICharacterController.SwitchActiveItem"/>
        public virtual void SwitchActiveItem(Item item)
        {
            /// shiftToolbar
        }
    }
}
