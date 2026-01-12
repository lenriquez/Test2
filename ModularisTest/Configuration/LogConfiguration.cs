using ModularisTest.Enums;
using System.Configuration;
using System.Reflection;

namespace ModularisTest.Configuration
{
    public class LogConfiguration
    {
        private readonly LogConfigurationSection _configSection;

        public LogConfiguration()
        {
            // Try multiple approaches to load configuration in .NET 6
            try
            {
                // First, try the standard method
                _configSection = ConfigurationManager.GetSection("logging") as LogConfigurationSection;
                
                if (_configSection == null)
                {
                    // Try opening exe configuration explicitly
                    var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                    var configPath = assemblyLocation + ".config";
                    
                    if (File.Exists(configPath))
                    {
                        var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configPath };
                        var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                        _configSection = config.GetSection("logging") as LogConfigurationSection;
                    }
                }
            }
            catch
            {
                // Final fallback
                _configSection = ConfigurationManager.GetSection("logging") as LogConfigurationSection;
            }

            if (_configSection == null)
            {
                throw new ConfigurationErrorsException("Logging configuration section not found in App.config");
            }
        }

        public IEnumerable<string> GetEnabledDestinations()
        {
            if (_configSection?.Destinations == null)
            {
                return Enumerable.Empty<string>();
            }

            return _configSection.Destinations
                .Cast<DestinationElement>()
                .Where(d => d.Enabled)
                .Select(d => d.Name);
        }

        public string GetDestinationConnectionString(string destinationName)
        {
            if (_configSection?.Destinations == null)
            {
                return null;
            }

            var destination = _configSection.Destinations
                .Cast<DestinationElement>()
                .FirstOrDefault(d => d.Name == destinationName);

            return destination?.ConnectionString;
        }

        public IEnumerable<LogMessageType> GetEnabledMessageTypes()
        {
            if (_configSection?.MessageTypes == null)
            {
                return Enumerable.Empty<LogMessageType>();
            }

            var enabledTypes = _configSection.MessageTypes
                .Cast<MessageTypeElement>()
                .Where(mt => mt.Enabled)
                .Select(mt => ParseMessageType(mt.Type))
                .Where(mt => mt.HasValue)
                .Select(mt => mt.Value);

            return enabledTypes;
        }

        public string GetFileSetting(string key)
        {
            if (_configSection?.FileSettings == null)
            {
                return null;
            }

            return _configSection.FileSettings.GetValue(key);
        }

        private LogMessageType? ParseMessageType(string typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString))
            {
                return null;
            }

            if (System.Enum.TryParse<LogMessageType>(typeString, true, out var messageType))
            {
                return messageType;
            }

            return null;
        }
    }
}
