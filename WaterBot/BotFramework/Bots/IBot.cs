using StardewModdingAPI.Events;

namespace BotFramework.Bots
{
    /// <summary>
    /// Abstraction of Bots that interact with game events.
    /// </summary>
    public interface IBot
    {
        /// <summary>
        /// Generates a key for the bot.
        /// </summary>
        /// <returns>Bot key.</returns>
        string GetKey();

        /// <summary>
        /// Generates a name for the bot.
        /// </summary>
        /// <returns>Bot name.</returns>
        string GetName();

        /// <summary>
        /// Generates a description for the bot.
        /// </summary>
        /// <returns>Bot description.</returns>
        string GetDescription();

        /// <summary>
        /// Starts the Bot listening for <see cref="IState"/> changes.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the Bot from listening for <see cref="IState"/> changes.
        /// </summary>
        void Stop();

        /// <summary>
        /// Event handler for game tick update.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        void GameTick(
            object sender,
            UpdateTickedEventArgs args
        );

        /// <summary>
        /// Event handler for game tick update.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        void DayStarted(
            object sender,
            DayStartedEventArgs args
        );

        /// <summary>
        /// Event handler for game tick update.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        void ButtonPressed(
            object sender,
            ButtonPressedEventArgs args
        );

        /// <summary>
        /// Event handler for game tick update.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        void Warped(
            object sender,
            WarpedEventArgs args
        );

        /// <summary>
        /// Event handler for before actions.
        /// </summary>
        void BeforeAction();

        /// <summary>
        /// Event handler for after actions.
        /// </summary>
        void AfterAction();
    }
}
