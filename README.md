# Cirreum Secrets Provider

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.SecretsProvider.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.SecretsProvider/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.SecretsProvider.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.SecretsProvider/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.SecretsProvider?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.SecretsProvider/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.SecretsProvider?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.SecretsProvider/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Secrets provider abstractions and registration patterns for the Cirreum Framework**

## Overview

**Cirreum.SecretsProvider** is the foundational library for implementing secrets providers within the Cirreum ecosystem. It provides the core abstractions, registration patterns, and configuration management infrastructure needed to build pluggable secrets management solutions that integrate seamlessly with .NET's configuration system.

### Key Features

#### 🔐 Provider Registration Pattern
A standardized approach to implementing secrets providers through the `SecretsProviderRegistrar<TSettings, TInstanceSettings>` abstract base class:

- **Multi-instance support** - Configure multiple instances of the same provider type with different endpoints
- **Duplicate detection** - Automatic prevention of duplicate endpoint registrations using SHA256 hashing
- **Validation framework** - Provider-specific settings validation before registration
- **Configuration integration** - Direct integration with .NET's IConfiguration system

#### 🔍 Endpoint Validation System
Robust validation and duplicate detection for provider endpoints:

- **Endpoint uniqueness** - SHA256-based hashing prevents duplicate endpoint registrations
- **Connection string parsing** - Extensible endpoint parsing for provider-specific formats
- **Error handling** - Clear error messages for configuration issues

#### ⚙️ Configuration Abstractions
Flexible configuration models that support provider-specific settings while maintaining consistency:

- **Hierarchical settings** - Provider-level settings with instance-specific overrides
- **OpenTelemetry integration** - Built-in tracing support for observability
- **Configuration binding** - Seamless integration with .NET configuration system

### Usage Example

```csharp
// Implement a custom secrets provider
public class MySecretsProvider : SecretsProviderRegistrar<MyProviderSettings, MyInstanceSettings>
{
    public override ProviderType ProviderType => ProviderType.Secrets;
    public override string ProviderName => "MySecretsProvider";
    public override string[] ActivitySourceNames => ["MySecretsProvider"];

    public override void AddSecretsProviderInstance(
        IConfigurationBuilder builder,
        MyInstanceSettings settings)
    {
        // Add the secrets provider to the configuration pipeline
        builder.Add(new MySecretsConfigurationSource(settings));
    }

    public override void ValidateSettings(MyInstanceSettings settings)
    {
        // Provider-specific validation
        if (string.IsNullOrEmpty(settings.ApiKey))
            throw new InvalidOperationException("ApiKey is required");
    }
}

// Define your settings classes
public class MyProviderSettings : SecretsProviderSettings<MyInstanceSettings> { }

public class MyInstanceSettings : SecretsProviderInstanceSettings
{
    public string ApiKey { get; set; } = "";
    
    public override void ParseEndPoint()
    {
        // Parse provider-specific connection strings if needed
        // This method is called during registration
    }
}

// Register the provider
var providerSettings = configuration.GetSection("MySecretsProvider").Get<MyProviderSettings>();
var myProvider = new MySecretsProvider();
myProvider.Register(providerSettings, services, configurationBuilder);
```

### Architecture

The library follows a layered architecture:

```text
SecretsProviderRegistrar (Base Class)
├── Provider Type Identification
├── Instance Management & Validation
├── Endpoint Duplicate Detection
└── Configuration Integration Hooks

Validation System
├── Endpoint Uniqueness (SHA256 Hashing)
├── Provider-specific Validation
└── Configuration Parsing

Configuration Models
├── Provider Settings (Multiple Instances + Tracing)
└── Instance Settings (Endpoint + Provider-specific)
```

### Installation

```bash
dotnet add package Cirreum.SecretsProvider
```

### Basic Setup

```csharp
// In Program.cs or Startup.cs
var builder = WebApplication.CreateBuilder(args);

// Register your secrets providers during configuration building
var providerSettings = builder.Configuration.GetSection("MySecretsProvider").Get<MyProviderSettings>();
var myProvider = new MySecretsProvider();
myProvider.Register(providerSettings, builder.Services, builder.Configuration);

var app = builder.Build();

// Secrets are now available through IConfiguration
var secretValue = app.Configuration["MySecret"];
```

## Contribution Guidelines

1. **Be conservative with new abstractions**  
   The API surface must remain stable and meaningful.

2. **Limit dependency expansion**  
   Only add foundational, version-stable dependencies.

3. **Favor additive, non-breaking changes**  
   Breaking changes ripple through the entire ecosystem.

4. **Include thorough unit tests**  
   All primitives and patterns should be independently testable.

5. **Document architectural decisions**  
   Context and reasoning should be clear for future maintainers.

6. **Follow .NET conventions**  
   Use established patterns from Microsoft.Extensions.* libraries.

## Versioning

Cirreum.SecretsProvider follows [Semantic Versioning](https://semver.org/):

- **Major** - Breaking API changes
- **Minor** - New features, backward compatible
- **Patch** - Bug fixes, backward compatible

Given its foundational role, major version bumps are rare and carefully considered.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*