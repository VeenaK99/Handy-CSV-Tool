using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyCSVConverter
{
    public class StringSplitConfigDataSection : ConfigurationSection
    {
        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "StringSplitConfigDataSection";

        private const string EndpointCollectionName = "CsvConfigCollection";

        [ConfigurationProperty(EndpointCollectionName)]
        [ConfigurationCollection(typeof(CSVConfigurationCollection), AddItemName = "add")]
        public CSVConfigurationCollection CsvConfigCollection { get { return (CSVConfigurationCollection)base[EndpointCollectionName]; } }
    }

    public class CSVConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CSVConfigItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CSVConfigItem)element).Name;
        }
    }

    public class CSVConfigItem : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("delimiter", IsRequired = true)]
        public string FieldDelimiter
        {
            get { return (string)this["delimiter"]; }
            set { this["delimiter"] = value; }
        }

        [ConfigurationProperty("enclose-char", IsRequired = false, DefaultValue = "")]
        public string EncloseCharacter
        {
            get { return (string)this["enclose-char"]; }
            set { this["enclose-char"] = value; }
        }

        [ConfigurationProperty("max-items-per-line", IsRequired = false, DefaultValue = -1)]
        public int MaxItemsPerLine
        {
            get { return (int)this["max-items-per-line"]; }
            set { this["max-items-per-line"] = value; }
        }
    }
}
