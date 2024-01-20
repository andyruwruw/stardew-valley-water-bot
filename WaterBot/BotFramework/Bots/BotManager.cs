using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace BotFramework.Bots
{
    /// <summary>
    /// Static manager for <see cref="IBot">Bots</see> which informs them of game events.
    /// </summary>
    public class BotManager
    {
        /// <summary>
        /// List of active <see cref="IBot"/>.
        /// </summary>
        protected static IList<IBot> _bots;

        /// <summary>
        /// Attatches a new <see cref="IBot"/> to manager.
        /// </summary>
        /// <param name="bot"><see cref="IBot"/> to be added.</param>
        public static void Attatch(IBot bot)
        {
            if (_bots == null)
                _bots = new List<IBot>();

            _bots.Add(bot);
        }

        /// <summary>
        /// Removes a <see cref="IBot"/>.
        /// </summary>
        /// <param name="bot"></param>
        public static void Detatch(IBot bot)
        {
            if (_bots.Contains(bot))
                _bots.RemoveAt(_bots.IndexOf(bot));
        }

        /// <summary>
        /// Keeps all <see cref="IBot">Bots</see> updated with new game ticks.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        public static void UpdateTicked(
            object sender,
            UpdateTickedEventArgs args
        )
        {
            foreach (IBot bot in _bots)
                bot.GameTick(
                    sender,
                    args
                );
        }

        /// <summary>
        /// Keeps all <see cref="IBot">Bots</see> updated with day events.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        public static void DayStarted(
            object sender,
            DayStartedEventArgs args
        )
        {
            foreach (IBot bot in _bots)
                bot.DayStarted(
                    sender,
                    args
                );
        }

        /// <summary>
        /// Keeps all <see cref="IBot">Bots</see> updated with button presses.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        public static void ButtonPressed(
            object sender,
            ButtonPressedEventArgs args
        )
        {
            foreach (IBot bot in _bots)
                bot.ButtonPressed(
                    sender,
                    args
                );
        }

        /// <summary>
        /// Keeps all <see cref="IBot">Bots</see> updated with warps.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        public static void Warped(
            object sender,
            WarpedEventArgs args
        )
        {
            foreach (IBot bot in _bots)
                bot.Warped(
                    sender,
                    args
                );
        }
    }
}
