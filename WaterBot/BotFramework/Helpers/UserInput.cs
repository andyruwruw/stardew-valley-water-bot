using StardewValley;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;

namespace BotFramework.Helpers
{
    /// <summary>
    /// Helper class for user input parsing.
    /// </summary>
    public class UserInput
    {
        /// <summary>
        /// References to input helper.
        /// </summary>
        public static IInputHelper Helper;

        /// <summary>
        /// Button press event arguments.
        /// </summary>
        private ButtonPressedEventArgs _args;

        /// <summary>
        /// Logged cursor position.
        /// </summary>
        private ICursorPosition _cursor;

        /// <summary>
        /// Player position at event.
        /// </summary>
        private Point _position;

        /// <summary>
        /// Location when clicked.
        /// </summary>
        private GameLocation _location;

        /// <summary>
        /// Tool location at click.
        /// </summary>
        private Vector2 _toolLocation;

        /// <summary>
        /// Instantiates a new user input parser.
        /// </summary>
        /// <param name="args">Button press arguments.</param>
        public UserInput(ButtonPressedEventArgs args)
        {
            this._args = args;
            this._cursor = UserInput.Helper.GetCursorPosition();
            this._position = Game1.player.getTileLocationPoint();
            this._location = Game1.currentLocation;
            this._toolLocation = Game1.player.GetToolLocation();
        }

        /// <summary>
        /// Retrieves the tile under the cursor.
        /// </summary>
        /// <returns><see cref="Vector2"/> of the tile under the user's cursor.</returns>
        public Vector2 GetCursorTileAtPress()
        {
            return this._cursor.Tile;
        }

        /// <summary>
        /// Retrieves the tile under the cursor.
        /// </summary>
        /// <returns><see cref="Vector2"/> of the tile under the user's cursor.</returns>
        public Vector2 GetCursorToolLocationAtPress()
        {
            return this._toolLocation;
        }

        /// <summary>
        /// Retrieves the tile under the cursor.
        /// </summary>
        /// <returns><see cref="Vector2"/> of the tile under the user's cursor.</returns>
        public TerrainFeature GetCursorTerrainFeatureAtPress()
        {
            if (this._location.terrainFeatures.ContainsKey(this.GetCursorTileAtPress()))
                return this._location.terrainFeatures[this.GetCursorTileAtPress()];
            return null;
        }

        /// <summary>
        /// Retrieves game location at time of press.
        /// </summary>
        /// <returns><see cref="GameLocation"/> when butten pressed.</returns>
        public GameLocation GetGameLocationAtPress()
        {
            return this._location;
        }
    }
}
