# JobLogger

A flexible, extensible logging system for .NET 6 applications that supports multiple logging destinations, message type classification, and configuration-driven setup.

## Features

- **Multiple Logging Destinations**: Log messages to files, console, and databases simultaneously
- **Message Type Classification**: Support for Message, Warning, and Error types
- **Configuration-Driven**: All settings stored in App.config
- **SOLID Principles**: Clean architecture following SOLID principles
- **Design Patterns**: Implements Singleton and Strategy patterns
- **Dependency Injection**: Full support for dependency injection using Microsoft.Extensions.DependencyInjection

## Architecture

<img width="626" height="363" alt="image" src="https://github.com/user-attachments/assets/bfc05714-811c-45ff-92dd-41eaa6142a5e" />


### Design Patterns

- **Singleton Pattern**: `JobLogger` provides a single instance for application-wide logging
- **Strategy Pattern**: Different logging destinations (`ILogDestination`) can be swapped without changing client code
- **Factory Pattern**: `LogDestinationFactory` creates destination instances from configuration

### SOLID Principles

1. **Single Responsibility**: Each destination class handles only one logging mechanism
2. **Open/Closed**: New destinations can be added by implementing `ILogDestination` without modifying existing code
3. **Liskov Substitution**: All destinations are interchangeable via `ILogDestination`
4. **Interface Segregation**: `ILogDestination` contains only logging-related methods
5. **Dependency Inversion**: `JobLogger` depends on `ILogDestination` abstraction, not concrete implementations

## Project Structure

```
ModularisTest/
├── Configuration/
│   ├── LogConfiguration.cs          # Reads settings from App.config
│   └── LogConfigurationSection.cs   # Custom configuration section handler
├── Destinations/
│   ├── ConsoleLogDestination.cs     # Logs to console with color coding
│   ├── DatabaseLogDestination.cs    # Logs to database
│   └── FileLogDestination.cs        # Logs to text files
├── Enums/
│   └── LogMessageType.cs            # Message type enumeration
├── Exceptions/
│   └── JobLoggerNotInitializedException.cs
├── Extensions/
│   └── LoggingServiceCollectionExtensions.cs  # DI registration
├── Factories/
│   └── LogDestinationFactory.cs     # Creates destinations from config
├── Interfaces/
│   └── ILogDestination.cs           # Strategy pattern interface
└── JobLogger.cs                      # Main logger class (Singleton)
```

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022, Rider, or VS Code

### Installation

1. Clone the repository
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
3. Build the solution:
   ```bash
   dotnet build
   ```

### Configuration

Configure logging in `App.config`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="logging" type="ModularisTest.Configuration.LogConfigurationSection, ModularisTest" />
  </configSections>
  <logging>
    <destinations>
      <add name="File" enabled="true" />
      <add name="Console" enabled="true" />
      <add name="Database" enabled="false" connectionString="Server=localhost;Database=Logs;Integrated Security=true;" />
    </destinations>
    <messageTypes>
      <add type="Message" enabled="true" />
      <add type="Warning" enabled="true" />
      <add type="Error" enabled="true" />
    </messageTypes>
    <fileSettings>
      <add key="LogFileDirectory" value="" />
    </fileSettings>
  </logging>
</configuration>
```

### Usage

#### Basic Usage

```csharp
using ModularisTest;
using ModularisTest.Enums;
using ModularisTest.Extensions;
using Microsoft.Extensions.DependencyInjection;

// Set up dependency injection
var services = new ServiceCollection();
services.AddLogging();
var serviceProvider = services.BuildServiceProvider();

// Get dependencies
var configuration = serviceProvider.GetRequiredService<LogConfiguration>();
var destinations = serviceProvider.GetRequiredService<IEnumerable<ILogDestination>>();
var enabledMessageTypes = configuration.GetEnabledMessageTypes();

// Initialize JobLogger
JobLogger.Initialize(destinations, enabledMessageTypes);

// Log messages
JobLogger.Instance.LogMessage("This is a message", LogMessageType.Message);
JobLogger.Instance.LogMessage("This is a warning", LogMessageType.Warning);
JobLogger.Instance.LogMessage("This is an error", LogMessageType.Error);
```

#### Logging Destinations

**File Logging**
- Logs are written to text files
- File format: `LogFile{Date}.txt` (e.g., `LogFile1.11.2026.txt`)
- Directory can be configured via `LogFileDirectory` setting
- Messages are appended to existing files

**Console Logging**
- Messages are displayed in the console
- Color coding:
  - **Red**: Error messages
  - **Yellow**: Warning messages
  - **White**: Regular messages

**Database Logging**
- Requires SQL Server database
- Table structure:
  ```sql
  CREATE TABLE Log (
      Message NVARCHAR(MAX),
      Type INT,
      Date DATETIME
  )
  ```
- Connection string must be provided in configuration

## Running Tests

Run all tests:
```bash
dotnet test
```

Run specific test project:
```bash
dotnet test ModularisTestUnitTests/ModularisTestUnitTests.csproj
```

### Test Coverage

The test suite includes:
- Basic logging functionality
- File destination verification
- Console destination verification
- Multiple message types
- Message appending
- Directory auto-creation
- Empty message handling
- Initialization validation

## Adding Custom Destinations

To add a new logging destination:

1. Implement `ILogDestination`:
   ```csharp
   public class CustomLogDestination : ILogDestination
   {
       public void LogMessage(string message, LogMessageType messageType)
       {
           // Your logging logic here
       }
   }
   ```

2. Update `LogDestinationFactory` to create instances of your destination

3. Add configuration in `App.config`

## Dependencies

- **Microsoft.Extensions.DependencyInjection** (8.0.0) - Dependency injection container
- **System.Configuration.ConfigurationManager** (8.0.0) - Configuration file support
- **Microsoft.Data.SqlClient** (5.1.5) - SQL Server database connectivity

## License

[Specify your license here]

## Contributing

[Add contribution guidelines if applicable]
