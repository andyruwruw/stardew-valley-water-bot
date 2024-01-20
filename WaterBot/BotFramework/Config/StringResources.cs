using StardewModdingAPI;

namespace BotFramework.Config
{
    /// <summary>
    /// Manages string resources for different languages.
    /// </summary>
    public class StringResources
    {
        /// <summary>
        /// References to input helper.
        /// </summary>
        public static ITranslationHelper Helper;

        /// <summary>
        /// Retrieves a translation for a given key.
        /// </summary>
        /// <param name="key">Key of string.</param>
        /// <returns>String value.</returns>
        public static string Get(string key)
        {
            return Helper.Get(key);
        }

        /// <summary>
        /// Retrieves a translation for a given key.
        /// </summary>
        /// <param name="key">Key of string.</param>
        /// <param name="tokens">Tokens to embed.</param>
        /// <returns>String value.</returns>
        public static string Get(
            string key,
            object tokens
        )
        {
            return Helper.Get(
                key,
                tokens
            );
        }
    }
}
