//-----------------------------------------------------------------------
// <copyright file="CSVConfigItem.cs" company="CompanyName">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------

namespace HandyCSVConverter
{
    using System.Configuration;

    /// <summary>
    /// A class which contains the split configuration item properties
    /// </summary>
    /// <seealso cref="System.Configuration.ConfigurationElement" />
    public class CSVConfigItem : ConfigurationElement
    {
        /// <summary>
        /// The name configuration attribute
        /// </summary>
        private const string NameConfigAttribute = "name";

        /// <summary>
        /// The split character configuration attribute
        /// </summary>
        private const string SplitCharConfigAttribute = "split-char";

        /// <summary>
        /// Gets or sets the name value from the application configuration file.
        /// </summary>
        /// <value>
        /// The name to be returned from the app config file.
        /// </value>
        [ConfigurationProperty(NameConfigAttribute, IsRequired = true)]
        public string Name
        {
            get { return (string)this[NameConfigAttribute]; }
            set { this[NameConfigAttribute] = value; }
        }

        /// <summary>
        /// Gets or sets the split character based on which the input string needs to be Split
        /// </summary>
        /// <value>
        /// The split character based on which the input string needs to be Split and replaced
        /// "\r\n" are the default values 
        /// </value>
        [ConfigurationProperty("split-char", IsRequired = false, DefaultValue = "\r\n")]
        public string SplitChar
        {
            get
            {
                return (string)this["split-char"];
            }

            set
            {
                this["split-char"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the field delimiter to be replaced after the input string is split
        /// </summary>
        /// <value>
        /// The field delimiter.
        /// </value>
        [ConfigurationProperty("delimiter", IsRequired = true)]
        public string FieldDelimiter
        {
            get { return (string)this["delimiter"]; }
            set { this["delimiter"] = value; }
        }

        /// <summary>
        /// Gets or sets the enclose character.
        /// </summary>
        /// <value>
        /// The enclose character.
        /// </value>
        [ConfigurationProperty("enclose-char", IsRequired = false, DefaultValue = "")]
        public string EncloseCharacter
        {
            get { return (string)this["enclose-char"]; }
            set { this["enclose-char"] = value; }
        }

        /// <summary>
        /// Gets or sets the maximum items per line.
        /// </summary>
        /// <value>
        /// The maximum items per line.
        /// </value>
        [ConfigurationProperty("max-items-per-line", IsRequired = false, DefaultValue = -1)]
        public int MaxItemsPerLine
        {
            get { return (int)this["max-items-per-line"]; }
            set { this["max-items-per-line"] = value; }
        }
    }
}