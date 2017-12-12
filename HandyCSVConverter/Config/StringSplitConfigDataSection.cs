//-----------------------------------------------------------------------
// <copyright file="StringSplitConfigDataSection.cs" company="CompanyName">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------

namespace HandyCSVConverter
{
    using System.Configuration;
    using HandyCSVConverter.Config;

    /// <summary>
    /// Represents a config section in the app.config file from which the configuration details can be read
    /// </summary>
    /// <seealso cref="System.Configuration.ConfigurationSection" />
    public class StringSplitConfigDataSection : ConfigurationSection
    {
        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "StringSplitConfigDataSection";

        /// <summary>
        /// The Config collection name to be used in app.config
        /// </summary>
        private const string ConfigCollection = "CsvConfigCollection";

        /// <summary>
        /// Gets the CSV configuration collection which would contain all the config collections
        /// </summary>
        /// <value>
        /// The CSV configuration collection.
        /// </value>
        [ConfigurationProperty(ConfigCollection)]
        [ConfigurationCollection(typeof(CSVConfigurationCollection), AddItemName = "add")]
        public CSVConfigurationCollection CsvConfigCollection
        {
            get
            {
                return (CSVConfigurationCollection)base[ConfigCollection];
            }
        }
    }
}