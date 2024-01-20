using BotFramework.Config;

namespace WaterBot.Config
{
    internal class WaterBotStringResources
    {
        /// <summary>
        /// Retrieves the name of the bot.
        /// </summary>
        /// <returns>Name of the bot.</returns>
        public static string GetBotName()
        {
            return StringResources.Get("bot.name");
        }

        /// <summary>
        /// Retrieves the description of the bot.
        /// </summary>
        /// <returns>Description of the bot.</returns>
        public static string GetBotDescription()
        {
            return StringResources.Get("bot.description");
        }

        /// <summary>
        /// Retrieves the name of a bot state.
        /// </summary>
        /// <param name="key"><see cref="IState"/> key.</param>
        /// <returns>State name.</returns>
        public static string GetStateName(string key)
        {
            return StringResources.Get($"state.{key}.name");
        }

        /// <summary>
        /// Retrieves message for state activation.
        /// </summary>
        /// <param name="key"><see cref="IState"/> key.</param>
        /// <returns>State activation message.</returns>
        public static string GetStateActivationMessage(string key)
        {
            return StringResources.Get($"state.{key}.activation");
        }

        /// <summary>
        /// Retrieves message for state inaccessibility.
        /// </summary>
        /// <param name="key"><see cref="IState"/> key.</param>
        /// <returns>State inaccessible message.</returns>
        public static string GetStateInaccessibleMessage(string key)
        {
            return StringResources.Get($"state.{key}.inaccessible");
        }

        /// <summary>
        /// Retrieves message for state interrupt.
        /// </summary>
        /// <param name="key"><see cref="IState"/> key.</param>
        /// <returns>State interrupt message.</returns>
        public static string GetStateInterruptMessage(string key)
        {
            return StringResources.Get($"state.{key}.interrupt");
        }

        /// <summary>
        /// Retrieves message for state exhausted.
        /// </summary>
        /// <param name="key"><see cref="IState"/> key.</param>
        /// <returns>State exhausted message.</returns>
        public static string GetStateExhaustedMessage(string key)
        {
            return StringResources.Get($"state.{key}.exhausted");
        }
    }
}
