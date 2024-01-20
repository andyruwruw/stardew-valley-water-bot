using BotFramework.Enums;
using System.Collections.Generic;

namespace BotFramework.Queries
{
    /// <summary>
    /// Method for targeting select objects.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Designated <see cref="ITarget"/> for this query.
        /// </summary>
        /// <returns>Query <see cref="ITarget"/></returns>
        ITarget GetTarget();

        /// <summary>
        /// How many objects to recieve.
        /// </summary>
        /// <returns>How many objects to recieve.</returns>
        QueryLimit GetQueryLimit();

        /// <summary>
        /// Where to look for targets.
        /// </summary>
        /// <returns>A scope for the query.</returns>
        QueryScope GetQueryScope();

        /// <summary>
        /// Bots ability to see targets.
        /// </summary>
        /// <returns>What targets the bot can see.</returns>
        BotAwareness GetBotAwareness();

        /// <summary>
        /// Post query selectors for altering targets.
        /// </summary>
        /// <returns>An <see cref="IList{T}"/> of <see cref="PostQuerySelector">PostQuerySelectors</see></returns>
        IList<PostQuerySelector> GetPostQuerySelectors();
    }
}
