using System.Configuration;

namespace ModularisTest.Configuration
{
    public class LogConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("destinations")]
        public DestinationElementCollection Destinations
        {
            get { return (DestinationElementCollection)this["destinations"]; }
        }

        [ConfigurationProperty("messageTypes")]
        public MessageTypeElementCollection MessageTypes
        {
            get { return (MessageTypeElementCollection)this["messageTypes"]; }
        }

        [ConfigurationProperty("fileSettings")]
        public FileSettingsElementCollection FileSettings
        {
            get { return (FileSettingsElementCollection)this["fileSettings"]; }
        }
    }

    public class DestinationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)this["enabled"]; }
            set { this["enabled"] = value; }
        }

        [ConfigurationProperty("connectionString", IsRequired = false)]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }
    }

    public class DestinationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DestinationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DestinationElement)element).Name;
        }

        public DestinationElement this[int index]
        {
            get { return (DestinationElement)BaseGet(index); }
        }
    }

    public class MessageTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)this["enabled"]; }
            set { this["enabled"] = value; }
        }
    }

    public class MessageTypeElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MessageTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MessageTypeElement)element).Type;
        }

        public MessageTypeElement this[int index]
        {
            get { return (MessageTypeElement)BaseGet(index); }
        }
    }

    public class FileSettingElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }

    public class FileSettingsElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FileSettingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileSettingElement)element).Key;
        }

        public FileSettingElement this[int index]
        {
            get { return (FileSettingElement)BaseGet(index); }
        }

        public string GetValue(string key)
        {
            var element = this.Cast<FileSettingElement>().FirstOrDefault(e => e.Key == key);
            return element?.Value;
        }
    }
}
