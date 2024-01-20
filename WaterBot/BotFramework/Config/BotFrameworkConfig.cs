namespace BotFramework.Config
{
    public class BotFrameworkConfig
    {
        /// <summary>
        /// Static reference to configuration.
        /// </summary>
        static public BotFrameworkConfig Config = new BotFrameworkConfig();

        /// <summary>
        /// Whether to spend the processing time finding the best path between groups.
        /// </summary>
        public bool FindBestPathBetweenGroups { get; set; }

        public BotFrameworkConfig()
        {
            this.FindBestPathBetweenGroups = true;
        }
    }
}
