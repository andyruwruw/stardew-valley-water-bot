using BotFramework.Enums;
using System.Collections.Generic;

namespace BotFramework.Queries
{
    /// <inheritdoc cref="IQuery"/>
    public abstract class Query : IQuery
    {
        /// <summary>
        /// Instantiates a new query.
        /// </summary>
        public Query()
        {
        }

        /// <inheritdoc cref="IQuery.GetTarget"/>
        public abstract ITarget GetTarget();

        /// <inheritdoc cref="IQuery.GetQueryLimit"/>
        public virtual QueryLimit GetQueryLimit()
        {
            return QueryLimit.DoForAll;
        }

        /// <inheritdoc cref="IQuery.GetQueryScope"/>
        public virtual QueryScope GetQueryScope()
        {
            return QueryScope.CurrentLocation;
        }

        /// <inheritdoc cref="IQuery.GetBotAwareness"/>
        public virtual BotAwareness GetBotAwareness()
        {
            return BotAwareness.Omniscient;
        }

        /// <inheritdoc cref="IQuery.GetPostQuerySelectors"/>
        public virtual IList<PostQuerySelector> GetPostQuerySelectors()
        {
            return new List<PostQuerySelector>();
        }

        /// <inheritdoc cref="IQuery.GetPostQuerySelectors"/>
        public virtual IList<PostQuerySelector> GetPostQuerySelectors(params PostQuerySelector[] selectors)
        {
            IList<PostQuerySelector> list = new List<PostQuerySelector>();

            foreach (PostQuerySelector selector in selectors)
            {
                list.Add(selector);
            }

            return list;
        }
    }
}
